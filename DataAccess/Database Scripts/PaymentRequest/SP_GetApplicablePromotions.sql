CREATE OR ALTER PROCEDURE GET_APPLICABLE_PROMOTIONS_PR
    @P_MerchantID INT,
    @P_FinancialEntityID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Promociones del comercio
    SELECT 
        mp.PromotionID,
        'Merchant' as PromotionType,
        mp.MerchantPromotionName as Name,
        mp.MerchantPromotionDescription as Description,
        mp.DiscountPercentage,
        mp.MaxRefund,
        mp.AvailableQuantity,
        m.MerchantName as SourceName
    FROM MerchantPromotions mp
    INNER JOIN Merchants m ON mp.MerchantID = m.MerchantID -- Usando JOIN
    WHERE mp.MerchantID = @P_MerchantID
      AND mp.ValidationStatus = 'Active'
      AND mp.AvailableQuantity > 0
      AND GETDATE() BETWEEN mp.StartDate AND mp.EndDate

    UNION ALL

    -- Promociones de la entidad financiera (si se especifica)
    SELECT 
        fp.PromotionID,
        'Financial' as PromotionType,
        fp.FinancialPromotionName as Name,
        fp.FinancialPromotionDescription as Description,
        fp.DiscountPercentage,
        fp.MaxRefund,
        fp.AvailableQuantity,
        fe.EntityName as SourceName
    FROM FinancialPromotions fp
    INNER JOIN FinancialEntities fe ON fp.FinancialEntityID = fe.FinancialEntityID -- Usando JOIN
    WHERE fp.FinancialEntityID = @P_FinancialEntityID
      AND fp.ValidationStatus = 'Active'
      AND fp.AvailableQuantity > 0
      AND GETDATE() BETWEEN fp.StartDate AND fp.EndDate
      AND @P_FinancialEntityID IS NOT NULL; -- El filtro para el parámetro opcional
END;
GO