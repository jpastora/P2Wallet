CREATE PROCEDURE RET_ALL_TRANSACTIONS_PR
AS
BEGIN
    SELECT 
        TransactionID, UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, DiscountApplied, CommissionApplied,
        Timestamp, TransactionStatus
    FROM Transactions
END
GO
