CREATE OR ALTER PROCEDURE UPDATE_TRANSACTION_PR
    @P_TransactionID INT,
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

    UPDATE Transactions
    SET
        UserID = @P_UserID,
        MerchantID = @P_MerchantID,
        BankAccountID = @P_BankAccountID,
        GrossAmount = @P_GrossAmount,
        NetAmount = @P_NetAmount,
        CommissionApplied = @P_CommissionApplied,
        SalesTaxAmount = @P_SalesTaxAmount,
        TaxRateApplied = @P_TaxRateApplied,
        TransactionStatus = @P_TransactionStatus,
        Timestamp = GETDATE(),
        FinancialPromotionID = @P_FinancialPromotionID,
        MerchantPromotionID = @P_MerchantPromotionID
    WHERE
        TransactionID = @P_TransactionID;
END;
GO