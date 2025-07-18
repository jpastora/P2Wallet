CREATE OR ALTER PROCEDURE UPDATE_USER_PR
    @P_UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET
        BiometricVerified = 'Active',
        ValidationStatus = 'Active',
        UpdatedAt = GETDATE()
    WHERE
        UserID = @P_UserID;
END;
GO
