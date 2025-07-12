CREATE PROCEDURE RET_ALL_ADMINISTRATORS_PR
AS
BEGIN
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
END
GO