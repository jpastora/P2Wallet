CREATE PROCEDURE RET_USER_BY_PHONE_PR
    @P_MobilePhone VARCHAR(15)
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
        MobilePhone = @P_MobilePhone
END
GO

