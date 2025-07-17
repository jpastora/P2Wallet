CREATE PROCEDURE SP_RetByUserIdUserEntity
@UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM UserEntity
    WHERE UserId = @UserId;
END;
