CREATE OR ALTER PROCEDURE RET_USER_BY_EMAIL_PR
    @P_Email VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserID, CreatedAt, EmailVerified, MobileVerified, BiometricVerified, ValidationStatus,
		SMSNotificaction, PushNotification, EmailNotification,
        FirstName, LastName,
        IDNumber, BirthDate, Email, MobilePhone, ProfilePhoto, IDPhotoFront, IDPhotoBack,
        Latitude, Longitude, Password, Role
    FROM
        Users
    WHERE
        Email = @P_Email
END
GO