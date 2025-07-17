CREATE OR ALTER PROCEDURE CREATE_USER_PR
    -- Datos Personales
    @P_FirstName VARCHAR(50),
    @P_LastName VARCHAR(50),
    @P_IDNumber VARCHAR(50),
    @P_BirthDate DATETIME,
    
    -- Contacto y Credenciales
    @P_Email VARCHAR(100),
    @P_MobilePhone VARCHAR(15),
    @P_Password VARCHAR(255),

    -- Fotos y Geolocalización
    @P_ProfilePhoto VARCHAR(300) = NULL,
    @P_IDPhotoFront VARCHAR(300),
    @P_IDPhotoBack VARCHAR(300),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    
    -- Rol y Estados (Opcionales, con valor por defecto 'Inactive')
    @P_Role VARCHAR(35) = 'User',
    @P_EmailVerified VARCHAR(15) = 'Inactive',
    @P_MobileVerified VARCHAR(15) = 'Inactive',
    @P_BiometricVerified VARCHAR(15) = 'Inactive',
    @P_ValidationStatus VARCHAR(15) = 'Inactive',
    @P_SMSNotification VARCHAR(15) = 'Inactive',
    @P_PushNotification VARCHAR(15) = 'Inactive',
    @P_EmailNotification VARCHAR(15) = 'Inactive'
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users (
        FirstName, LastName, IDNumber, BirthDate, Email, MobilePhone,
        ProfilePhoto, IDPhotoFront, IDPhotoBack, Latitude, Longitude, Password,
        EmailVerified, MobileVerified, BiometricVerified, ValidationStatus,
        SMSNotificaction, PushNotification, EmailNotification,
        CreatedAt, Role
    )
    VALUES (
        @P_FirstName, @P_LastName, @P_IDNumber, @P_BirthDate, @P_Email, @P_MobilePhone,
        @P_ProfilePhoto, @P_IDPhotoFront, @P_IDPhotoBack, @P_Latitude, @P_Longitude, @P_Password,
        @P_EmailVerified, @P_MobileVerified, @P_BiometricVerified, @P_ValidationStatus,
        @P_SMSNotificaction, @P_PushNotification, @P_EmailNotification,
        GETDATE(), @P_Role
    );
END;
GO