using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public class MerchantCrudFactory : CrudFactory
    {
        public MerchantCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }
        public override void Create(BaseDTO baseDTO)
        {
            var merchant = baseDTO as Merchant;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_MERCHANT_PR" };

            sqlOperation.AddStringParameter("@P_MerchantName", merchant.MerchantName);
            sqlOperation.AddStringParameter("@P_TaxID", merchant.TaxID);
            sqlOperation.AddDoubleParam("@P_Latitude", merchant.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", merchant.Longitude);
            sqlOperation.AddStringParameter("@P_ContactPhone", merchant.ContactPhone);
            sqlOperation.AddStringParameter("@P_Email", merchant.Email);
            sqlOperation.AddDoubleParam("@P_CommissionPercentage", merchant.CommissionPercentage);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        public override void Delete(BaseDTO baseDTO)
        {
            var merchant = baseDTO as Merchant;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_MERCHANT_PR" };
            sqlOperation.Parameters.Add(new SqlParameter("@P_ID", merchant.ID));

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }
        public override List<T> RetrieveAll<T>()
        {
            var lstMerchants = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_MERCHANTS_PR" };
            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            foreach (var row in results)
            {
                var merchant = BuildMerchant(row);
                lstMerchants.Add((T)Convert.ChangeType(merchant, typeof(T)));
            }

            return lstMerchants;
        }
        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_MERCHANT_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_MerchantID", ID);

            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (results.Count > 0)
            {
                var merchant = BuildMerchant(results[0]);
                return (T)Convert.ChangeType(merchant, typeof(T));
            }

            return default(T);
        }
        public override void Update(BaseDTO baseDTO)
        {
            var merchant = baseDTO as Merchant;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_MERCHANT_PR" };

            sqlOperation.AddIntParam("@P_MerchantID", merchant.ID);
            sqlOperation.AddStringParameter("@P_MerchantName", merchant.MerchantName);
            sqlOperation.AddStringParameter("@P_TaxID", merchant.TaxID);
            sqlOperation.AddStringParameter("@P_LogoImage", merchant.LogoImage);
            sqlOperation.AddDoubleParam("@P_Latitude", merchant.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", merchant.Longitude);
            sqlOperation.AddStringParameter("@P_ContactPhone", merchant.ContactPhone);
            sqlOperation.AddStringParameter("@P_Email", merchant.Email);
            sqlOperation.AddDoubleParam("@P_CommissionPercentage", merchant.CommissionPercentage);
            sqlOperation.AddStringParameter("@P_ValidationStatus", merchant.ValidationStatus);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private Merchant BuildMerchant(Dictionary<string, object> row)
        {
            return new Merchant
            {
                ID = (int)row["MerchantID"],
                MerchantName = (string)row["MerchantName"],
                TaxID = (string)row["TaxID"],
                LogoImage = (string)row["LogoImage"],
                Latitude = Convert.ToDouble(row["Latitude"]),
                Longitude = Convert.ToDouble(row["Longitude"]),
                ContactPhone = (string)row["ContactPhone"],
                Email = (string)row["Email"],
                CommissionPercentage = Convert.ToDouble(row["CommissionPercentage"]),
                ValidationStatus = (string)row["ValidationStatus"]
            };
        }
    }
}
