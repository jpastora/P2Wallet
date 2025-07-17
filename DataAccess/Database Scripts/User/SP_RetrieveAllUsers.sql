CREATE OR ALTER PROCEDURE RET_ALL_USERS_PR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, FirstName, LastName, Role, IDNumber, Email, BirthDate, MobilePhone, ProfilePhoto, 
           IDPhotoFront, IDPhotoBack, Latitude, Longitude, Password, EmailVerified, MobileVerified, 
           BiometricVerified, ValidationStatus, SMSNotificaction, PushNotification, EmailNotification, 
           CreatedAt, UpdatedAt
    FROM Users;
END;
GO