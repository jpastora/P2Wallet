CREATE OR ALTER PROCEDURE EXECUTE_PAYMENT_WITH_PROMOTION_PR
    @P_PaymentRequestCode NVARCHAR(50),
    @P_UserID INT,
    @P_BankAccountID INT,
    @P_SelectedPromotionID INT = NULL,
    @P_SelectedPromotionType NVARCHAR(20) = NULL -- 'Merchant' o 'Financial'
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Variables
        DECLARE @TransactionID INT, @MerchantID INT, @FinancialEntityID INT;
        DECLARE @OriginalNetAmount DECIMAL(18,2), @DiscountApplied DECIMAL(18,2) = 0;
        DECLARE @TaxRate DECIMAL(5,2);
        DECLARE @FinalNetAmount DECIMAL(18,2), @FinalSalesTaxAmount DECIMAL(18,2), @FinalGrossAmount DECIMAL(18,2), @FinalCommissionApplied DECIMAL(18,2);
        DECLARE @UserBalance DECIMAL(18,2);

        -- 1. Obtener y validar datos de la solicitud de pago
        SELECT 
            @TransactionID = T.TransactionID, 
            @MerchantID = T.MerchantID, 
            @OriginalNetAmount = T.NetAmount, -- El monto de venta original antes de impuestos
            @TaxRate = T.TaxRateApplied,
            @FinancialEntityID = BA.FinancialEntityID
        FROM Transactions T
        INNER JOIN BankAccounts BA ON BA.BankAccountID = @P_BankAccountID
        WHERE T.PaymentRequestCode = @P_PaymentRequestCode 
          AND T.TransactionStatus = 'PendingUserApproval' 
          AND T.ExpiresAt > GETDATE()
          AND BA.UserID = @P_UserID; -- Validar que la cuenta pertenezca al usuario

        IF @TransactionID IS NULL
        BEGIN
            RAISERROR('Solicitud de pago no válida, expirada, o la cuenta bancaria no pertenece al usuario.', 16, 1);
            RETURN;
        END

        -- 2. Aplicar promoción si se seleccionó una válida
        IF @P_SelectedPromotionID IS NOT NULL AND @P_SelectedPromotionType IS NOT NULL
        BEGIN
            DECLARE @DiscountPercentage DECIMAL(5,2), @MaxRefund DECIMAL(18,2);
            
            IF @P_SelectedPromotionType = 'Merchant'
            BEGIN
                SELECT @DiscountPercentage = DiscountPercentage, @MaxRefund = MaxRefund
                FROM MerchantPromotions 
                WHERE PromotionID = @P_SelectedPromotionID AND MerchantID = @MerchantID
                  AND ValidationStatus = 'Active' AND AvailableQuantity > 0 AND GETDATE() BETWEEN StartDate AND EndDate;
            END
            ELSE IF @P_SelectedPromotionType = 'Financial'
            BEGIN
                SELECT @DiscountPercentage = DiscountPercentage, @MaxRefund = MaxRefund
                FROM FinancialPromotions
                WHERE PromotionID = @P_SelectedPromotionID AND FinancialEntityID = @FinancialEntityID
                  AND ValidationStatus = 'Active' AND AvailableQuantity > 0 AND GETDATE() BETWEEN StartDate AND EndDate;
            END
            
            -- Calcular descuento si se encontró una promoción válida
            IF @DiscountPercentage IS NOT NULL
            BEGIN
                SET @DiscountApplied = @OriginalNetAmount * (@DiscountPercentage / 100.0);
                IF @DiscountApplied > @MaxRefund
                    SET @DiscountApplied = @MaxRefund;

                -- Actualizar cantidad disponible de la promoción
                IF @P_SelectedPromotionType = 'Merchant'
                    UPDATE MerchantPromotions SET AvailableQuantity = AvailableQuantity - 1 WHERE PromotionID = @P_SelectedPromotionID;
                ELSE IF @P_SelectedPromotionType = 'Financial'
                    UPDATE FinancialPromotions SET AvailableQuantity = AvailableQuantity - 1 WHERE PromotionID = @P_SelectedPromotionID;
            END
        END

        -- 3. Recalcular montos finales
        SET @FinalNetAmount = @OriginalNetAmount - @DiscountApplied;
        SET @FinalSalesTaxAmount = @FinalNetAmount * (@TaxRate / 100.0);
        SET @FinalGrossAmount = @FinalNetAmount + @FinalSalesTaxAmount;

        -- 4. Verificar fondos del usuario
        SELECT @UserBalance = Balance FROM BankAccounts WHERE BankAccountID = @P_BankAccountID;
        
        IF @UserBalance < @FinalGrossAmount
        BEGIN
            RAISERROR('Fondos insuficientes.', 16, 1);
            RETURN;
        END

        -- 5. Calcular comisión final (basada en nuestra lógica de negocio)
        DECLARE @EntityComm DECIMAL(5, 2), @MerchantComm DECIMAL(5, 2), @TotalCommRate DECIMAL(10, 4);
        SELECT @EntityComm = CommissionPercentage FROM FinancialEntities WHERE FinancialEntityID = @FinancialEntityID;
        SELECT @MerchantComm = CommissionPercentage FROM Merchants WHERE MerchantID = @MerchantID;
        SET @TotalCommRate = (@EntityComm / 100.0) + (@MerchantComm / 100.0);
        SET @FinalCommissionApplied = @FinalNetAmount * @TotalCommRate;

        -- 6. Actualizar balances y registrar movimientos
        UPDATE BankAccounts SET Balance = Balance - ROUND(@FinalGrossAmount, 2) WHERE BankAccountID = @P_BankAccountID;
        -- Aquí irían los UPDATE a MerchantBankAccounts y SystemAccounts, y los INSERT en AccountMovements

        -- 7. Actualizar la transacción a estado 'Completed' con los datos finales
        UPDATE Transactions SET 
            UserID = @P_UserID,
            BankAccountID = @P_BankAccountID,
            GrossAmount = ROUND(@FinalGrossAmount, 2),
            NetAmount = ROUND(@FinalNetAmount, 2),
            SalesTaxAmount = ROUND(@FinalSalesTaxAmount, 2),
            CommissionApplied = ROUND(@FinalCommissionApplied, 2),
            TransactionStatus = 'Completed',
            MerchantPromotionID = CASE WHEN @P_SelectedPromotionType = 'Merchant' THEN @P_SelectedPromotionID ELSE NULL END,
            FinancialPromotionID = CASE WHEN @P_SelectedPromotionType = 'Financial' THEN @P_SelectedPromotionID ELSE NULL END,
            Timestamp = GETDATE()
        WHERE TransactionID = @TransactionID;
        
        COMMIT TRANSACTION;
        
        SELECT @TransactionID AS TransactionID, @DiscountApplied AS DiscountApplied, @FinalGrossAmount AS FinalAmountPaid;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO