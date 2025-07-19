CREATE OR ALTER PROCEDURE UPDATE_USER_BIOMETRIC_PR
    @P_UserID INT,
    @P_BirthDate DATE,
    @P_IDNumber VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET
        BiometricVerified = 'Active',
        ValidationStatus = 'Active',
        UpdatedAt = GETDATE(),
        BirthDate = @P_BirthDate,
        IDNumber = @P_IDNumber
    WHERE
        UserID = @P_UserID;
END;
GO
