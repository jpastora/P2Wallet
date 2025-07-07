using DataAccess.CRUD;
using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

public class MerchantPromotionCrudFactory : CrudFactory
{
    public MerchantPromotionCrudFactory()
    {
        _sqlDao = SqlDAO.GetInstance();
    }

    public override void Create(BaseDTO baseDTO)
    {
        var promo = (MerchantPromotions)baseDTO;
        var sqlOperation = new SqlOperation { ProcedureName = "CREATE_MERCHANT_PROMOTION_PR" };

        sqlOperation.AddIntParam("P_MERCHANT_ID", promo.MerchantID);
        sqlOperation.AddStringParameter("P_PROMOTION_TYPE", promo.PromotionType);
        sqlOperation.AddDoubleParam("P_DISCOUNT_PERCENTAGE", (double)promo.DiscountPercentage);
        sqlOperation.Parameters.Add(new SqlParameter("P_MAX_REFUND", promo.MaxRefund.HasValue ? (object)promo.MaxRefund : DBNull.Value));
        sqlOperation.AddDateTimeParam("P_START_DATE", promo.StartDate);
        sqlOperation.AddDateTimeParam("P_END_DATE", promo.EndDate);
        sqlOperation.AddIntParam("P_AVAILABLE_QUANTITY", promo.AvailableQuantity);
        sqlOperation.AddStringParameter("P_STATUS", promo.Status);
        sqlOperation.Parameters.Add(new SqlParameter
        {
            ParameterName = "OUT_PROMOTION_ID",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        });

        _sqlDao.ExecuteProcedure(sqlOperation);
        promo.PromotionID = (int)(sqlOperation.Parameters.FirstOrDefault(p => p.ParameterName == "OUT_PROMOTION_ID")?.Value ?? 0);
    }

    public override void Update(BaseDTO baseDTO)
    {
        var promo = (MerchantPromotions)baseDTO;
        var sqlOperation = new SqlOperation { ProcedureName = "UPDATE_MERCHANT_PROMOTION_PR" };

        sqlOperation.AddIntParam("P_PROMOTION_ID", promo.PromotionID);
        sqlOperation.AddIntParam("P_MERCHANT_ID", promo.MerchantID);
        sqlOperation.AddStringParameter("P_PROMOTION_TYPE", promo.PromotionType);
        sqlOperation.AddDoubleParam("P_DISCOUNT_PERCENTAGE", (double)promo.DiscountPercentage);
        sqlOperation.Parameters.Add(new SqlParameter("P_MAX_REFUND", promo.MaxRefund.HasValue ? (object)promo.MaxRefund : DBNull.Value));
        sqlOperation.AddDateTimeParam("P_START_DATE", promo.StartDate);
        sqlOperation.AddDateTimeParam("P_END_DATE", promo.EndDate);
        sqlOperation.AddIntParam("P_AVAILABLE_QUANTITY", promo.AvailableQuantity);
        sqlOperation.AddStringParameter("P_STATUS", promo.Status);

        _sqlDao.ExecuteProcedure(sqlOperation);
    }

    public override void Delete(BaseDTO baseDTO)
    {
        var promo = (MerchantPromotions)baseDTO;
        var sqlOperation = new SqlOperation { ProcedureName = "DELETE_MERCHANT_PROMOTION_PR" };
        sqlOperation.AddIntParam("P_PROMOTION_ID", promo.PromotionID);
        _sqlDao.ExecuteProcedure(sqlOperation);
    }

    public override T RetrieveById<T>(int id)
    {
        var sqlOperation = new SqlOperation { ProcedureName = "RETRIEVE_MERCHANT_PROMOTION_BY_ID_PR" };
        sqlOperation.AddIntParam("P_PROMOTION_ID", id);

        var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);
        if (result.Count > 0)
        {
            var row = result[0];
            var promo = MapRow(row);
            return (T)Convert.ChangeType(promo, typeof(T));
        }

        return default;
    }

    public override List<T> RetrieveAll<T>()
    {
        var sqlOperation = new SqlOperation { ProcedureName = "RETRIEVE_ALL_MERCHANT_PROMOTIONS_PR" };
        var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);

        return result.Select(row => (T)Convert.ChangeType(MapRow(row), typeof(T))).ToList();
    }

    private MerchantPromotions MapRow(Dictionary<string, object> row)
    {
        return new MerchantPromotions
        {
            PromotionID = Convert.ToInt32(row["PromotionID"]),
            MerchantID = Convert.ToInt32(row["MerchantID"]),
            PromotionType = row["PromotionType"].ToString(),
            DiscountPercentage = Convert.ToDecimal(row["DiscountPercentage"]),
            MaxRefund = row["MaxRefund"] != DBNull.Value ? Convert.ToDecimal(row["MaxRefund"]) : (decimal?)null,
            StartDate = Convert.ToDateTime(row["StartDate"]),
            EndDate = Convert.ToDateTime(row["EndDate"]),
            AvailableQuantity = Convert.ToInt32(row["AvailableQuantity"]),
            Status = row["Status"].ToString(),
            CreatedAt = Convert.ToDateTime(row["CreatedAt"])
        };
    }
}
