CREATE OR ALTER PROCEDURE UPDATE_FINANCIAL_PROMOTION_PR
    @P_PromotionID INT,
    @P_FinancialEntityID INT,
    @P_PromotionType VARCHAR(20),
    @P_DiscountPercentage DECIMAL(5,2),
    @P_MaxRefund DECIMAL(12,2),
    @P_StartDate DATETIME,
    @P_EndDate DATETIME,
    @P_AvailableQuantity INT,
    @P_ValidationStatus VARCHAR(15),
    @P_FinancialPromotionName VARCHAR(100),
    @P_FinancialPromotionDescription VARCHAR(255),
    @P_FinancialPromotionTerms VARCHAR(MAX),
    @P_FinancialPromotionImage VARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE FinancialPromotions
    SET
        FinancialEntityID = @P_FinancialEntityID,
        PromotionType = @P_PromotionType,
        DiscountPercentage = @P_DiscountPercentage,
        MaxRefund = @P_MaxRefund,
        StartDate = @P_StartDate,
        EndDate = @P_EndDate,
        AvailableQuantity = @P_AvailableQuantity,
        ValidationStatus = @P_ValidationStatus,
        FinancialPromotionName = @P_FinancialPromotionName,
        FinancialPromotionDescription = @P_FinancialPromotionDescription,
        FinancialPromotionTerms = @P_FinancialPromotionTerms,
        FinancialPromotionImage = @P_FinancialPromotionImage
    WHERE
        PromotionID = @P_PromotionID;
END;
GO