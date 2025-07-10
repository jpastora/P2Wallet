CREATE PROCEDURE CREATE_USER_PR
    @P_FullName VARCHAR(100),
    @P_Email VARCHAR(100),
    @P_MobilePhone VARCHAR(15),
    @P_IDPhotoFront VARCHAR(300),
    @P_IDPhotoBack VARCHAR(300),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_Password VARCHAR(255)
AS
BEGIN
    INSERT INTO Users (
        CreatedAt, FullName, Email, MobilePhone,
        IDPhotoFront, IDPhotoBack, Latitude, Longitude, Password
    )
    VALUES (
        GETDATE(), @P_FullName, @P_Email, @P_MobilePhone,
        @P_IDPhotoFront, @P_IDPhotoBack, @P_Latitude, @P_Longitude, @P_Password
    )
END
GO