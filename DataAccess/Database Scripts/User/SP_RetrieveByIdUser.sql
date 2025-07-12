CREATE PROCEDURE RET_USER_BY_ID_PR
    @P_UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        UserID, CreatedAt, EmailVerified, MobileVerified, BiometricVerified, ValidationStatus,
		SMSNotificaction, PushNotification, EmailNotification,
        FullName, IDNumber, BirthDate, Email, MobilePhone, ProfilePhoto, IDPhotoFront, IDPhotoBack,
        Latitude, Longitude, Password
    FROM 
        Users
    WHERE 
        UserID = @P_UserID
END
GO

