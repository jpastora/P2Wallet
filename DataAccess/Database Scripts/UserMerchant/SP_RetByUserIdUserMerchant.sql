CREATE PROCEDURE RET_MERCHANTS_BY_USER_ID_PR
    @P_UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.*
    FROM Merchants m
    INNER JOIN UserMerchant um ON m.MerchantID = um.MerchantID
    WHERE um.UserID = @P_UserID;
END;
GO