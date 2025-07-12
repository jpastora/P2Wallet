using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public class FinancialPromotionCrudFactory : CrudFactory
    {
        public FinancialPromotionCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO baseDTO)
        {
            var promo = baseDTO as FinancialPromotion;
            var operation = new SqlOperation { ProcedureName = "CREATE_FINANCIAL_PROMOTION_PR" };

            operation.AddIntParam("@P_FinancialEntityID", promo.FinancialEntityID);
            operation.AddStringParameter("@P_PromotionType", promo.PromotionType);
            operation.AddDoubleParam("@P_DiscountPercentage", promo.DiscountPercentage);
            operation.AddDoubleParam("@P_MaxRefund", promo.MaxRefund);
            operation.AddDateTimeParam("@P_StartDate", promo.StartDate);
            operation.AddDateTimeParam("@P_EndDate", promo.EndDate);
            operation.AddIntParam("@P_AvailableQuantity", promo.AvailableQuantity);
            operation.AddStringParameter("@P_ValidationStatus", promo.ValidationStatus);

            _sqlDao.ExecuteProcedure(operation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var promo = baseDTO as FinancialPromotion;
            var operation = new SqlOperation { ProcedureName = "DELETE_FINANCIAL_PROMOTION_PR" };
            operation.AddIntParam("@P_PromotionID", promo.ID);
            _sqlDao.ExecuteProcedure(operation);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var promo = baseDTO as FinancialPromotion;
            var operation = new SqlOperation { ProcedureName = "UPDATE_FINANCIAL_PROMOTION_PR" };

            operation.AddIntParam("@P_PromotionID", promo.ID);
            operation.AddIntParam("@P_FinancialEntityID", promo.FinancialEntityID);
            operation.AddStringParameter("@P_PromotionType", promo.PromotionType);
            operation.AddDoubleParam("@P_DiscountPercentage", promo.DiscountPercentage);
            operation.AddDoubleParam("@P_MaxRefund", promo.MaxRefund);
            operation.AddDateTimeParam("@P_StartDate", promo.StartDate);
            operation.AddDateTimeParam("@P_EndDate", promo.EndDate);
            operation.AddIntParam("@P_AvailableQuantity", promo.AvailableQuantity);
            operation.AddStringParameter("@P_ValidationStatus", promo.ValidationStatus);

            _sqlDao.ExecuteProcedure(operation);
        }

        public override T RetrieveById<T>(int id)
        {
            var operation = new SqlOperation { ProcedureName = "RET_FINANCIAL_PROMOTION_BY_ID_PR" };
            operation.AddIntParam("@P_PromotionID", id);

            var result = _sqlDao.ExecuteQueryProcedure(operation);
            if (result.Count > 0)
            {
                var row = result[0];
                var promo = BuildFinancialPromotion(row);
                return (T)Convert.ChangeType(promo, typeof(T));
            }
            return default(T);
        }

        public override List<T> RetrieveAll<T>()
        {
            var list = new List<T>();
            var operation = new SqlOperation { ProcedureName = "RET_ALL_FINANCIAL_PROMOTIONS_PR" };

            var result = _sqlDao.ExecuteQueryProcedure(operation);
            foreach (var row in result)
            {
                var promo = BuildFinancialPromotion(row);
                list.Add((T)Convert.ChangeType(promo, typeof(T)));
            }

            return list;
        }

        private FinancialPromotion BuildFinancialPromotion(Dictionary<string, object> row)
        {
            return new FinancialPromotion
            {
                ID = (int)row["PromotionID"],
                FinancialEntityID = (int)row["FinancialEntityID"],
                PromotionType = (string)row["PromotionType"],
                DiscountPercentage = Convert.ToDouble(row["DiscountPercentage"]),
                MaxRefund = Convert.ToDouble(row["MaxRefund"]),
                StartDate = Convert.ToDateTime(row["StartDate"]),
                EndDate = Convert.ToDateTime(row["EndDate"]),
                AvailableQuantity = Convert.ToInt32(row["AvailableQuantity"]),
                ValidationStatus = (string)row["ValidationStatus"]
            };
        }
    }

}
