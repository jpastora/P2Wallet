CREATE PROCEDURE UPDATE_TRANSACTION_PR
    @P_TransactionID INT,
    @P_UserID INT,
    @P_MerchantID INT,
    @P_BankAccountID INT,
    @P_GrossAmount DECIMAL(12,2),
    @P_NetAmount DECIMAL(12,2),
    @P_DiscountApplied DECIMAL(12,2),
    @P_CommissionApplied DECIMAL(12,2),
    @P_TransactionStatus VARCHAR(20)
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
        DiscountApplied = @P_DiscountApplied,
        CommissionApplied = @P_CommissionApplied,
        TransactionStatus = @P_TransactionStatus,
        Timestamp = GETDATE()
    WHERE
        TransactionID = @P_TransactionID
END
GO
