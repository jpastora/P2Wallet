using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            var promotion = (FinancialPromotion)baseDTO;
            var sqlOperation = new SqlOperation { ProcedureName = "CREATE_FINANCIAL_PROMOTION_SP" };

            sqlOperation.AddIntParam("P_FINANCIAL_ENTITY_ID", promotion.FinancialEntityID);
            sqlOperation.AddStringParameter("P_PROMOTION_TYPE", promotion.PromotionType);
            sqlOperation.AddDoubleParam("P_DISCOUNT_PERCENTAGE", (double)promotion.DiscountPercentage);

            if (promotion.MaxRefund.HasValue)
            {
                sqlOperation.AddDoubleParam("P_MAX_REFUND", (double)promotion.MaxRefund.Value);
            }
            else
            {
                sqlOperation.Parameters.Add(new SqlParameter("P_MAX_REFUND", DBNull.Value));
            }

            sqlOperation.AddDateTimeParam("P_START_DATE", promotion.StartDate);
            sqlOperation.AddDateTimeParam("P_END_DATE", promotion.EndDate);
            sqlOperation.AddIntParam("P_AVAILABLE_QUANTITY", promotion.AvailableQuantity);
            sqlOperation.AddStringParameter("P_STATUS", promotion.Status);

            // Add output parameter
            sqlOperation.Parameters.Add(new SqlParameter
            {
                ParameterName = "OUT_PROMOTION_ID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            });

            _sqlDao.ExecuteProcedure(sqlOperation);

            // Get output parameter value
            promotion.PromotionID = (int)(sqlOperation.Parameters.FirstOrDefault(p =>
                p.ParameterName == "OUT_PROMOTION_ID")?.Value ?? 0);
        }

        public override T RetrieveById<T>(int id)
        {
            var sqlOperation = new SqlOperation { ProcedureName = "RETRIEVE_FINANCIAL_PROMOTION_BY_ID_SP" };
            sqlOperation.AddIntParam("P_PROMOTION_ID", id);

            var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (result.Count > 0)
            {
                var firstRow = result[0];
                var promotion = new FinancialPromotion()
                {
                    PromotionID = Convert.ToInt32(firstRow["PromotionID"]),
                    FinancialEntityID = Convert.ToInt32(firstRow["FinancialEntityID"]),
                    PromotionType = firstRow["PromotionType"].ToString(),
                    DiscountPercentage = Convert.ToDecimal(firstRow["DiscountPercentage"]),
                    MaxRefund = firstRow["MaxRefund"] != DBNull.Value ? Convert.ToDecimal(firstRow["MaxRefund"]) : (decimal?)null,
                    StartDate = Convert.ToDateTime(firstRow["StartDate"]),
                    EndDate = Convert.ToDateTime(firstRow["EndDate"]),
                    AvailableQuantity = Convert.ToInt32(firstRow["AvailableQuantity"]),
                    Status = firstRow["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(firstRow["CreatedAt"])
                };

                return (T)Convert.ChangeType(promotion, typeof(T));
            }

            return default;
        }

        public override List<T> RetrieveAll<T>()
        {
            var sqlOperation = new SqlOperation { ProcedureName = "RETRIEVE_ALL_FINANCIAL_PROMOTIONS_SP" };

            var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);
            var promotions = new List<T>();

            foreach (var row in result)
            {
                var promotion = new FinancialPromotion()
                {
                    PromotionID = Convert.ToInt32(row["PromotionID"]),
                    FinancialEntityID = Convert.ToInt32(row["FinancialEntityID"]),
                    PromotionType = row["PromotionType"].ToString(),
                    DiscountPercentage = Convert.ToDecimal(row["DiscountPercentage"]),
                    MaxRefund = row["MaxRefund"] != DBNull.Value ? Convert.ToDecimal(row["MaxRefund"]) : (decimal?)null,
                    StartDate = Convert.ToDateTime(row["StartDate"]),
                    EndDate = Convert.ToDateTime(row["EndDate"]),
                    AvailableQuantity = Convert.ToInt32(row["AvailableQuantity"]),
                    Status = row["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                };

                promotions.Add((T)Convert.ChangeType(promotion, typeof(T)));
            }

            return promotions;
        }

        // Implement Update, Delete, and RetrieveActivePromotions similarly...
        // [Previous Update and Delete methods remain the same]

        public List<T> RetrieveActivePromotions<T>()
        {
            var sqlOperation = new SqlOperation { ProcedureName = "RETRIEVE_ACTIVE_FINANCIAL_PROMOTIONS_SP" };

            var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);
            var promotions = new List<T>();

            foreach (var row in result)
            {
                var promotion = new FinancialPromotion()
                {
                    PromotionID = Convert.ToInt32(row["PromotionID"]),
                    FinancialEntityID = Convert.ToInt32(row["FinancialEntityID"]),
                    PromotionType = row["PromotionType"].ToString(),
                    DiscountPercentage = Convert.ToDecimal(row["DiscountPercentage"]),
                    MaxRefund = row["MaxRefund"] != DBNull.Value ? Convert.ToDecimal(row["MaxRefund"]) : (decimal?)null,
                    StartDate = Convert.ToDateTime(row["StartDate"]),
                    EndDate = Convert.ToDateTime(row["EndDate"]),
                    AvailableQuantity = Convert.ToInt32(row["AvailableQuantity"]),
                    Status = row["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                };

                promotions.Add((T)Convert.ChangeType(promotion, typeof(T)));
            }

            return promotions;
        }

        public override void Update(BaseDTO baseDTO)
        {
            var promotion = (FinancialPromotion)baseDTO;
            var sqlOperation = new SqlOperation { ProcedureName = "UPDATE_FINANCIAL_PROMOTION_SP" };

            sqlOperation.AddIntParam("P_PROMOTION_ID", promotion.PromotionID);
            sqlOperation.AddIntParam("P_FINANCIAL_ENTITY_ID", promotion.FinancialEntityID);
            sqlOperation.AddStringParameter("P_PROMOTION_TYPE", promotion.PromotionType);
            sqlOperation.AddDoubleParam("P_DISCOUNT_PERCENTAGE", (double)promotion.DiscountPercentage);

            if (promotion.MaxRefund.HasValue)
            {
                sqlOperation.AddDoubleParam("P_MAX_REFUND", (double)promotion.MaxRefund.Value);
            }
            else
            {
                sqlOperation.Parameters.Add(new SqlParameter("P_MAX_REFUND", DBNull.Value));
            }

            sqlOperation.AddDateTimeParam("P_START_DATE", promotion.StartDate);
            sqlOperation.AddDateTimeParam("P_END_DATE", promotion.EndDate);
            sqlOperation.AddIntParam("P_AVAILABLE_QUANTITY", promotion.AvailableQuantity);
            sqlOperation.AddStringParameter("P_STATUS", promotion.Status);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }


        public override void Delete(BaseDTO baseDTO)
        {
            var promotion = (FinancialPromotion)baseDTO;
            var sqlOperation = new SqlOperation { ProcedureName = "DELETE_FINANCIAL_PROMOTION_SP" };

            sqlOperation.AddIntParam("P_PROMOTION_ID", promotion.PromotionID);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }



    }
}