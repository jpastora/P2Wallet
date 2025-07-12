CREATE PROCEDURE RET_ADMINISTRATOR_BY_ID_PR
    @P_AdminID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        AdminID, 
        Name, 
        AccessUsername, 
        AccessPassword, 
        ValidationStatus, 
        CreatedAt, 
        UpdatedAt
    FROM 
        Administrators
    WHERE 
        AdminID = @P_AdminID
END
GO