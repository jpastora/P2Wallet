CREATE PROCEDURE CREATE_TRANSACTION_PR
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
    INSERT INTO Transactions (
        UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, DiscountApplied, CommissionApplied,
        Timestamp, TransactionStatus
    )
    VALUES (
        @P_UserID, @P_MerchantID, @P_BankAccountID,
        @P_GrossAmount, @P_NetAmount, @P_DiscountApplied, @P_CommissionApplied,
        GETDATE(), @P_TransactionStatus
    )
END
GO