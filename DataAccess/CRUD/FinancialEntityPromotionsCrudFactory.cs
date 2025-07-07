using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using System;
using System.Collections.Generic;

namespace DataAccess.CRUD
{
    public class FinancialEntityPromotionCrudFactory : CrudFactory
    {
        public FinancialEntityPromotionCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO baseDTO)
        {
            var promo = baseDTO as FinancialEntityPromotions;
            var op = new SqlOperation { ProcedureName = "CREATE_FINANCIAL_PROMOTION_PR" };

            op.AddIntParam("@P_FINANCIAL_ENTITYID", promo.FinancialEntityID);
            op.AddStringParameter("@P_PROMOTION_TYPE", promo.PromotionType);
            op.AddDoubleParam("@P_DISCOUNT_PERCENTAGE", Convert.ToDouble(promo.DiscountPercentage));
            op.AddDoubleParam("@P_MAX_REFUND", Convert.ToDouble(promo.MaxRefund));
            op.AddDateTimeParam("@P_START_DATE", promo.StartDate ?? DateTime.Now);
            op.AddDateTimeParam("@P_END_DATE", promo.EndDate ?? DateTime.Now);
            op.AddIntParam("@P_AVAILABLE_QUANTITY", promo.AvailableQuantity ?? 0);
            op.AddBoolParam("@P_STATUS", promo.Status);

            _sqlDao.ExecuteProcedure(op);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var promo = baseDTO as FinancialEntityPromotions;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_FINANCIAL_PROMOTION_PR" };

            sqlOperation.AddIntParam("@P_PROMOTIONID", promo.ID);
            sqlOperation.AddIntParam("@P_FINANCIAL_ENTITYID", promo.FinancialEntityID);
            sqlOperation.AddStringParameter("@P_PROMOTION_TYPE", promo.PromotionType);
            sqlOperation.AddDoubleParam("@P_DISCOUNT_PERCENTAGE", Convert.ToDouble(promo.DiscountPercentage));
            sqlOperation.AddDoubleParam("@P_MAX_REFUND", Convert.ToDouble(promo.MaxRefund));
            sqlOperation.AddDateTimeParam("@P_START_DATE", promo.StartDate ?? DateTime.Now);
            sqlOperation.AddDateTimeParam("@P_END_DATE", promo.EndDate ?? DateTime.Now);
            sqlOperation.AddIntParam("@P_AVAILABLE_QUANTITY", promo.AvailableQuantity ?? 0);
            sqlOperation.AddBoolParam("@P_STATUS", promo.Status);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var promo = baseDTO as FinancialEntityPromotions;
            var op = new SqlOperation { ProcedureName = "DELETE_FINANCIAL_PROMOTION_PR" };
            op.AddIntParam("@P_PROMOTIONID", promo.PromotionID);

            _sqlDao.ExecuteProcedure(op);
        }

        public override FinancialEntityPromotion RetrieveById<FinancialEntityPromotion>(int id)
        {
            var op = new SqlOperation { ProcedureName = "RETRIEVE_FINANCIAL_PROMOTION_BY_ID_PR" };
            op.AddIntParam("@P_PROMOTIONID", id);

            var result = _sqlDao.ExecuteQueryProcedure(op);
            if (result.Count == 0) return default;

            return (FinancialEntityPromotion)(object)BuildEntity(result[0]);
        }

        public override List<FinancialEntityPromotion> RetrieveAll<FinancialEntityPromotion>()
        {
            var op = new SqlOperation { ProcedureName = "RETRIEVE_ALL_FINANCIAL_PROMOTIONS_PR" };
            var result = _sqlDao.ExecuteQueryProcedure(op);
            var list = new List<FinancialEntityPromotion>();

            foreach (var row in result)
            {
                list.Add((FinancialEntityPromotion)(object)BuildEntity(row));
            }

            return list;
        }

        private DTOs.FinancialEntityPromotions BuildEntity(Dictionary<string, object> row)
        {
            return new DTOs.FinancialEntityPromotions
            {
                PromotionID = Convert.ToInt32(row["PromotionID"]),
                FinancialEntityID = Convert.ToInt32(row["FinancialEntityID"]),
                PromotionType = row["PromotionType"].ToString(),
                DiscountPercentage = Convert.ToDecimal(row["DiscountPercentage"]),
                MaxRefund = Convert.ToDecimal(row["MaxRefund"]),
                StartDate = row["StartDate"] as DateTime?,
                EndDate = row["EndDate"] as DateTime?,
                AvailableQuantity = row["AvailableQuantity"] as int?,
                Status = Convert.ToBoolean(row["Status"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }
    }
}
