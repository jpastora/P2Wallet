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
        public override void Create(BaseDTO baseDTO)
        {
            var merchant = baseDTO as Merchant;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_MERCHANT_SP" };

            sqlOperation.AddStringParameter("@P_MerchantName", merchant.MerchantName);
            sqlOperation.AddStringParameter("@P_TaxID", merchant.TaxID);
            sqlOperation.AddStringParameter("@P_LogoImage", merchant.LogoImage);
            sqlOperation.AddDoubleParam("@P_Latitude", merchant.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", merchant.Longitude);
            sqlOperation.AddStringParameter("@P_Phone", merchant.Phone);
            sqlOperation.AddStringParameter("@P_Email", merchant.Email);
            sqlOperation.AddDoubleParam("@P_CommissionPercentage", merchant.CommissionPercentage);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        public override void Delete(BaseDTO baseDTO)
        {
            var merchant = baseDTO as Merchant;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_MERCHANT_SP" };
            sqlOperation.Parameters.Add(new SqlParameter("@P_ID", merchant.ID));

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }
        public override List<T> RetrieveAll<T>()
        {
            throw new NotImplementedException();
        }
        public override T RetrieveById<T>(int iD)
        {
            throw new NotImplementedException();
        }
        public override void Update(BaseDTO baseDTO)
        {
            throw new NotImplementedException();
        }
    }
}
