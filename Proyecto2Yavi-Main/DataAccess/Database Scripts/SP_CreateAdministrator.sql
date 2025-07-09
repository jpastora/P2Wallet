CREATE PROCEDURE CREATE_ADMINISTRATOR_PR
    @P_Name VARCHAR(20),
    @P_AccessUsername VARCHAR(50),
    @P_AccessPassword VARCHAR(255)
AS
BEGIN
    INSERT INTO Administrators (
        Name, 
        AccessUsername, 
        AccessPassword, 
        CreatedAt
    )
    VALUES (
        @P_Name, 
        @P_AccessUsername, 
        @P_AccessPassword, 
        GETDATE()
    )
END
GO