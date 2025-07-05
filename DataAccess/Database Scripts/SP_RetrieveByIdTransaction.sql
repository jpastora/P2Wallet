CREATE PROCEDURE RET_TRANSACTION_BY_ID_PR
    @P_TransactionID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        TransactionID, UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, DiscountApplied, CommissionApplied,
        Timestamp, TransactionStatus
    FROM 
        Transactions
    WHERE 
        TransactionID = @P_TransactionID
END
GO