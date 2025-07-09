CREATE PROCEDURE RET_USER_BY_ID_PR
    @P_UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        UserID, CreatedAt, EmailVerified, MobileVerified, BiometricVerified, UserStatus,
        FullName, Email, MobilePhone, ProfilePhoto, IDPhotoFront, IDPhotoBack,
        Latitude, Longitude, Password, UpdatedAt
    FROM 
        Users
    WHERE 
        UserID = @P_UserID
END
GO