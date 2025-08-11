CREATE OR ALTER PROCEDURE CREATE_PAYMENT_REQUEST_PR
    @P_MerchantID INT,
    @P_SaleAmount DECIMAL(18,2),
    @P_Description NVARCHAR(255) = '',
    @P_ExpirationMinutes INT = 30,
    @P_ExpiresAt DATETIME = NULL,  -- Nuevo parámetro opcional
    @P_TransactionID INT OUTPUT,
    @P_PaymentRequestCode NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        BEGIN TRANSACTION;
        
        -- 1. Generar código único para la solicitud
        SET @P_PaymentRequestCode = 'PAY' + FORMAT(GETDATE(), 'yyyyMMddHHmmss') + RIGHT('000' + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR), 4);
        
        -- 2. Calcular montos preliminares (13% IVA estándar Costa Rica)
        DECLARE @TaxRate DECIMAL(5,2) = 13.0;
        DECLARE @SalesTaxAmount DECIMAL(18,2) = @P_SaleAmount * (@TaxRate / 100.0);
        DECLARE @GrossAmount DECIMAL(18,2) = @P_SaleAmount + @SalesTaxAmount;
        
        -- 3. Determinar fecha de expiración
        DECLARE @ExpirationDate DATETIME;
        IF @P_ExpiresAt IS NOT NULL
            SET @ExpirationDate = @P_ExpiresAt;  -- Usar fecha proporcionada por el servidor de aplicación
        ELSE
            SET @ExpirationDate = DATEADD(MINUTE, @P_ExpirationMinutes, GETDATE());  -- Fallback
        
        -- 4. Crear la transacción en estado PendingUserApproval
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
            @ExpirationDate,                      -- ExpiresAt (usa fecha del servidor de aplicación)
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
            @ExpirationDate AS ExpiresAt;
            
    END TRY
    BEGIN CATCH
        -- Si hay una transacción abierta, revertirla
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Re-lanzar el error para que la aplicación lo reciba
        THROW;
    END CATCH;
END;
GO