CREATE PROCEDURE CREATE_FINANCIAL_PROMOTION_PR
    @P_FinancialEntityID INT,
    @P_PromotionType VARCHAR(20),
    @P_DiscountPercentage DECIMAL(5,2),
    @P_MaxRefund DECIMAL(12,2),
    @P_StartDate DATETIME,
    @P_EndDate DATETIME,
    @P_AvailableQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO FinancialPromotions (
        FinancialEntityID,
        PromotionType,
        DiscountPercentage,
        MaxRefund,
        StartDate,
        EndDate,
        AvailableQuantity,
        CreatedAt
    )
    VALUES (
        @P_FinancialEntityID,
        @P_PromotionType,
        @P_DiscountPercentage,
        @P_MaxRefund,
        @P_StartDate,
        @P_EndDate,
        @P_AvailableQuantity,
        GETDATE()
    );
END;
GO
