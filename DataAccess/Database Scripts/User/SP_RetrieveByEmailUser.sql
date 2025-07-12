CREATE PROCEDURE RET_USER_BY_EMAIL_PR
    @P_Email VARCHAR(100)
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
        Email = @P_Email
END
GO