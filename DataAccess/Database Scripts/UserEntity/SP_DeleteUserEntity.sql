CREATE PROCEDURE DELETE_USER_FINANCIAL_ENTITY_LINK_PR
    @P_UserID INT,
    @P_FinancialEntityID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM UserFinancialEntity
    WHERE UserID = @P_UserID AND FinancialEntityID = @P_FinancialEntityID;
END;
GO