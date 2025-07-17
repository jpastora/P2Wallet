CREATE PROCEDURE CREATE_USER_FINANCIAL_ENTITY_LINK_PR
    @P_UserID INT,
    @P_FinancialEntityID INT
AS
BEGIN
    SET NOCOUNT ON;
--Se inserta el enlace solo si no existe previamente para evitar errores de clave primaria
    IF NOT EXISTS (SELECT 1 FROM UserFinancialEntity WHERE UserID = @P_UserID AND FinancialEntityID = @P_FinancialEntityID)
    BEGIN
        INSERT INTO UserFinancialEntity (UserID, FinancialEntityID)
        VALUES (@P_UserID, @P_FinancialEntityID);
END
END;
GO