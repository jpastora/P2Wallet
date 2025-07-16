CREATE OR ALTER PROCEDURE RET_ALL_USERS_PR
AS
BEGIN
    SELECT
        UserID, CreatedAt, EmailVerified, MobileVerified, BiometricVerified, ValidationStatus,
		SMSNotificaction, PushNotification, EmailNotification,
        FirstName, LastName,
        IDNumber, BirthDate, Email, MobilePhone, ProfilePhoto, IDPhotoFront, IDPhotoBack,
        Latitude, Longitude, Password, Role
    FROM Users
END
GO