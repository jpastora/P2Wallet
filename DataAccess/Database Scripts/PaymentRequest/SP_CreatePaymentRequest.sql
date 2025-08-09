CREATE OR ALTER PROCEDURE CREATE_PAYMENT_REQUEST_PR
    @P_MerchantID INT,
    @P_SaleAmount DECIMAL(18,2),
    @P_Description NVARCHAR(255) = '',
    @P_ExpirationMinutes INT = 30,
    @P_TransactionID INT OUTPUT,
    @P_PaymentRequestCode NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY -- << CORRECCIÓN: Se añade BEGIN TRY

        BEGIN TRANSACTION;
        
        -- 1. Generar código único para la solicitud
        SET @P_PaymentRequestCode = 'PAY' + FORMAT(GETDATE(), 'yyyyMMddHHmmss') + RIGHT('000' + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR), 4);
        
        -- 2. Calcular montos preliminares (13% IVA estándar Costa Rica)
        DECLARE @TaxRate DECIMAL(5,2) = 13.0;
        DECLARE @SalesTaxAmount DECIMAL(18,2) = @P_SaleAmount * (@TaxRate / 100.0); -- Usar 100.0 para división decimal
        DECLARE @GrossAmount DECIMAL(18,2) = @P_SaleAmount + @SalesTaxAmount;
        
        -- 3. Crear la transacción en estado PendingUserApproval
        INSERT INTO Transactions (
            UserID, MerchantID, BankAccountID, GrossAmount, NetAmount, 
            CommissionApplied, SalesTaxAmount, TaxRateApplied, 
            Timestamp, TransactionStatus, PaymentRequestCode, Description, ExpiresAt,
            FinancialPromotionID, MerchantPromotionID
        ) VALUES (
            NULL,                                 -- UserID (se asigna cuando el usuario aprueba)
            @P_MerchantID,                        -- MerchantID
            NULL,                                 -- BankAccountID (se asigna cuando el usuario selecciona cuenta)
            @GrossAmount,                         -- GrossAmount
            @P_SaleAmount,                        -- NetAmount
            0,                                    -- CommissionApplied (se calcula al ejecutar)
            @SalesTaxAmount,                      -- SalesTaxAmount
            @TaxRate,                             -- TaxRateApplied
            GETDATE(),                            -- Timestamp
            'PendingUserApproval',                -- TransactionStatus
            @P_PaymentRequestCode,                -- PaymentRequestCode
            @P_Description,                       -- Description
            DATEADD(MINUTE, @P_ExpirationMinutes, GETDATE()), -- ExpiresAt
            NULL,                                 -- FinancialPromotionID
            NULL                                  -- MerchantPromotionID
        );
        
        SET @P_TransactionID = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        -- Devolver un resumen de la solicitud creada
        SELECT 
            @P_TransactionID AS TransactionID,
            @P_PaymentRequestCode AS PaymentRequestCode,
            @GrossAmount AS GrossAmount,
            @P_SaleAmount AS NetAmount,
            @SalesTaxAmount AS SalesTaxAmount,
            DATEADD(MINUTE, @P_ExpirationMinutes, GETDATE()) AS ExpiresAt;
            
    END TRY -- << CORRECCIÓN: Fin del bloque TRY
    BEGIN CATCH
        -- Si hay una transacción abierta, revertirla
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Re-lanzar el error para que la aplicación lo reciba
        THROW;
    END CATCH; -- << CORRECCIÓN: Se añade END CATCH
END;
GO