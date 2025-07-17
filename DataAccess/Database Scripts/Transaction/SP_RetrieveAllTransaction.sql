CREATE OR ALTER PROCEDURE RET_ALL_TRANSACTIONS_PR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TransactionID, UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, CommissionApplied, SalesTaxAmount, TaxRateApplied,
        Timestamp, TransactionStatus,
        FinancialPromotionID, MerchantPromotionID
    FROM Transactions;
END;
GO