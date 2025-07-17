CREATE OR ALTER PROCEDURE RET_TRANSACTION_BY_ID_PR
    @P_TransactionID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        TransactionID, UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, CommissionApplied, SalesTaxAmount, TaxRateApplied,
        Timestamp, TransactionStatus,
        FinancialPromotionID, MerchantPromotionID
    FROM 
        Transactions
    WHERE 
        TransactionID = @P_TransactionID;
END;
GO