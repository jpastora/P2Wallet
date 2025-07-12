CREATE PROCEDURE UPDATE_ADMINISTRATOR_PR
    @P_AdminID INT,
    @P_Name VARCHAR(20),
    @P_AccessUsername VARCHAR(50),
    @P_AccessPassword VARCHAR(255),
    @P_ValidationStatus VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Administrators
    SET
        Name = @P_Name,
        AccessUsername = @P_AccessUsername,
        AccessPassword = @P_AccessPassword,
        ValidationStatus = @P_ValidationStatus,
        UpdatedAt = GETDATE()
    WHERE
        AdminID = @P_AdminID
END
GO