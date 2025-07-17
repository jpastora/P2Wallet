CREATE OR ALTER PROCEDURE CREATE_TRANSACTION_PR
    @P_UserID INT,
    @P_MerchantID INT,
    @P_BankAccountID INT,
    @P_GrossAmount DECIMAL(12,2),
    @P_NetAmount DECIMAL(12,2),
    @P_CommissionApplied DECIMAL(12,2),
    @P_SalesTaxAmount DECIMAL(12,2),
    @P_TaxRateApplied DECIMAL(5, 2),
    @P_TransactionStatus VARCHAR(20),
    @P_FinancialPromotionID INT = NULL,
    @P_MerchantPromotionID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Transactions (
        UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, CommissionApplied, SalesTaxAmount, TaxRateApplied,
        Timestamp, TransactionStatus,
        FinancialPromotionID, MerchantPromotionID
    )
    VALUES (
        @P_UserID, @P_MerchantID, @P_BankAccountID,
        @P_GrossAmount, @P_NetAmount, @P_CommissionApplied, @P_SalesTaxAmount, @P_TaxRateApplied,
        GETDATE(), @P_TransactionStatus,
        @P_FinancialPromotionID, @P_MerchantPromotionID
    );
END;
GO