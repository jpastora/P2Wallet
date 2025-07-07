CREATE PROCEDURE CREATE_FINANCIAL_PROMOTION_PR
    @P_FINANCIAL_ENTITY_ID INT,
    @P_PROMOTION_TYPE NVARCHAR(100),
    @P_DISCOUNT_PERCENTAGE FLOAT,
    @P_MAX_REFUND FLOAT = NULL,
    @P_START_DATE DATETIME,
    @P_END_DATE DATETIME,
    @P_AVAILABLE_QUANTITY INT,
    @P_STATUS NVARCHAR(50),
    @OUT_PROMOTION_ID INT OUTPUT
AS
BEGIN
    INSERT INTO FinancialPromotions (
        FinancialEntityID,
        PromotionType,
        DiscountPercentage,
        MaxRefund,
        StartDate,
        EndDate,
        AvailableQuantity,
        Status,
        CreatedAt
    )
    VALUES (
        @P_FINANCIAL_ENTITY_ID,
        @P_PROMOTION_TYPE,
        @P_DISCOUNT_PERCENTAGE,
        @P_MAX_REFUND,
        @P_START_DATE,
        @P_END_DATE,
        @P_AVAILABLE_QUANTITY,
        @P_STATUS,
        GETDATE()
    );

    SET @OUT_PROMOTION_ID = SCOPE_IDENTITY();
END
