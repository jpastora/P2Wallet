CREATE PROCEDURE RET_TRANSACTION_BY_PAYMENT_CODE_PR
    @P_PaymentRequestCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TransactionID, UserID, MerchantID, BankAccountID,
        GrossAmount, NetAmount, CommissionApplied, SalesTaxAmount, TaxRateApplied,
        Timestamp, TransactionStatus, FinancialPromotionID, MerchantPromotionID,
        PaymentRequestCode, Description, ExpiresAt
    FROM Transactions 
    WHERE PaymentRequestCode = @P_PaymentRequestCode
    AND ExpiresAt > GETDATE()
    AND TransactionStatus = 'PendingUserApproval'
END