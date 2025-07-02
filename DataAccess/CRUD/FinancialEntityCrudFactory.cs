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
    public class FinancialEntityCrudFactory : CrudFactory
    {
        public override void Create(BaseDTO baseDTO)
        {
            var entity = baseDTO as FinancialEntity;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_FINANCIAL_ENTITY_SP" };

            sqlOperation.AddStringParameter("@P_EntityName", entity.EntityName);
            sqlOperation.AddStringParameter("@P_TaxID", entity.TaxID);
            sqlOperation.AddDoubleParam("@P_Latitude", entity.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", entity.Longitude);
            sqlOperation.AddStringParameter("@P_ContactPhone", entity.ContactPhone);
            sqlOperation.AddStringParameter("@P_Email", entity.Email);
            sqlOperation.AddDoubleParam("@P_CommissionPercentage", entity.CommissionPercentage);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        public override void Delete(BaseDTO baseDTO)
        {
            var entity = baseDTO as FinancialEntity;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_FINANCIAL_ENTITY_SP" };
            sqlOperation.Parameters.Add(new SqlParameter("@P_ID", entity.ID));

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
