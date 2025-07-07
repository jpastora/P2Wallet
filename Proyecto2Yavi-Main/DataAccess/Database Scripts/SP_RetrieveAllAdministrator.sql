CREATE PROCEDURE RET_ALL_ADMINISTRATORS_PR
AS
BEGIN
    SELECT 
        AdminID, 
        Name, 
        AccessUsername, 
        AccessPassword, 
        AdminStatus, 
        CreatedAt, 
        UpdatedAt
    FROM 
        Administrators
END
GO