CREATE PROCEDURE RET_USERS_BY_MERCHANT_ID_PR
    @P_MerchantID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.*
    FROM Users u
    INNER JOIN UserMerchant um ON u.UserID = um.UserID
    WHERE um.MerchantID = @P_MerchantID;
END;
GO