CREATE PROCEDURE RET_ALL_USERS_PR
AS
BEGIN
    SELECT 
        UserID, CreatedAt, EmailVerified, MobileVerified, BiometricVerified, UserStatus,
        FullName, Email, MobilePhone, ProfilePhoto, IDPhotoFront, IDPhotoBack,
        Latitude, Longitude, Password
    FROM Users
END
GO
