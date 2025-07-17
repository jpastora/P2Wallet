CREATE OR ALTER PROCEDURE RET_USER_BY_PHONE_PR
    @P_MobilePhone VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, FirstName, LastName, Role, IDNumber, Email, BirthDate, MobilePhone, ProfilePhoto, 
           IDPhotoFront, IDPhotoBack, Latitude, Longitude, Password, EmailVerified, MobileVerified, 
           BiometricVerified, ValidationStatus, SMSNotificaction, PushNotification, EmailNotification, 
           CreatedAt, UpdatedAt
    FROM Users
    WHERE MobilePhone = @P_MobilePhone;
END;
GO