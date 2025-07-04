CREATE PROCEDURE UPDATE_USER_PR
    @P_UserID INT,
    @P_FullName VARCHAR(100),
    @P_Email VARCHAR(100),
    @P_MobilePhone VARCHAR(15),
    @P_ProfilePhoto VARCHAR(300),
    @P_IDPhotoFront VARCHAR(300),
    @P_IDPhotoBack VARCHAR(300),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_Password VARCHAR(255),
    @P_EmailVerified BIT,
    @P_MobileVerified BIT,
    @P_BiometricVerified BIT,
    @P_UserStatus BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET
        FullName = @P_FullName,
        Email = @P_Email,
        MobilePhone = @P_MobilePhone,
        ProfilePhoto = @P_ProfilePhoto,
        IDPhotoFront = @P_IDPhotoFront,
        IDPhotoBack = @P_IDPhotoBack,
        Latitude = @P_Latitude,
        Longitude = @P_Longitude,
        Password = @P_Password,
        EmailVerified = @P_EmailVerified,
        MobileVerified = @P_MobileVerified,
        BiometricVerified = @P_BiometricVerified,
        UserStatus = @P_UserStatus,
        UpdatedAt = GETDATE()
    WHERE
        UserID = @P_UserID
END
GO