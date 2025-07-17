CREATE PROCEDURE RET_USERS_BY_FINANCIAL_ENTITY_ID_PR
    @P_FinancialEntityID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.*
    FROM Users u
    INNER JOIN UserFinancialEntity ufe ON u.UserID = ufe.UserID
    WHERE ufe.FinancialEntityID = @P_FinancialEntityID;
END;
GO