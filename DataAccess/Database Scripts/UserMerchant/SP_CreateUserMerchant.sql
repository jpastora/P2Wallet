CREATE PROCEDURE CREATE_USER_MERCHANT_LINK_PR
    @P_UserID INT,
    @P_MerchantID INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Se inserta el enlace solo si no existe previamente
    IF NOT EXISTS (SELECT 1 FROM UserMerchant WHERE UserID = @P_UserID AND MerchantID = @P_MerchantID)
    BEGIN
        INSERT INTO UserMerchant (UserID, MerchantID)
        VALUES (@P_UserID, @P_MerchantID);
    END
END;
GO