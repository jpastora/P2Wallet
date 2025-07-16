CREATE OR ALTER PROCEDURE CREATE_USER_PR
    @P_FirstName VARCHAR(50),
    @P_LastName VARCHAR(50),
    @P_IDNumber VARCHAR(50),
    @P_Email VARCHAR(100),
    @P_BirthDate DATETIME,
    @P_MobilePhone VARCHAR(15),
    @P_IDPhotoFront VARCHAR(300),
    @P_IDPhotoBack VARCHAR(300),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_Password VARCHAR(255),
    @P_Role VARCHAR(35) = 'User'
AS
BEGIN
    INSERT INTO Users (
        FirstName, LastName,
        IDNumber, Email, BirthDate, MobilePhone,
        IDPhotoFront, IDPhotoBack, Latitude, Longitude, Password,
        Role, CreatedAt
    )
    VALUES (
        @P_FirstName, @P_LastName,
        @P_IDNumber, @P_Email, @P_BirthDate, @P_MobilePhone,
        @P_IDPhotoFront, @P_IDPhotoBack, @P_Latitude, @P_Longitude, @P_Password,
        @P_Role, GETDATE()
    )
END
GO