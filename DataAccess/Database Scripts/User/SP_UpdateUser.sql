CREATE OR ALTER PROCEDURE UPDATE_USER_PR
    @P_UserID INT,
    @P_FirstName VARCHAR(50),
    @P_LastName VARCHAR(50),
    @P_IDNumber VARCHAR(50),
    @P_BirthDate DATETIME,
    @P_Email VARCHAR(100),
    @P_MobilePhone VARCHAR(15),
    @P_ProfilePhoto VARCHAR(300),
    @P_IDPhotoFront VARCHAR(300),
    @P_IDPhotoBack VARCHAR(300),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_Password VARCHAR(255),
    @P_EmailVerified VARCHAR(15),
    @P_MobileVerified VARCHAR(15),
    @P_BiometricVerified VARCHAR(15),
    @P_ValidationStatus VARCHAR(15),
    @P_SMSNotification VARCHAR(15),
    @P_PushNotification VARCHAR(15),
    @P_EmailNotification VARCHAR(15),
    @P_Role VARCHAR(35)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET
        FirstName = @P_FirstName,
        LastName = @P_LastName,
        IDNumber = @P_IDNumber,
        BirthDate = @P_BirthDate,
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
        ValidationStatus = @P_ValidationStatus,
        SMSNotificaction = @P_SMSNotificaction,
        PushNotification = @P_PushNotification,
        EmailNotification = @P_EmailNotification,
        Role = @P_Role,
        UpdatedAt = GETDATE()
    WHERE
        UserID = @P_UserID;
END;
GO