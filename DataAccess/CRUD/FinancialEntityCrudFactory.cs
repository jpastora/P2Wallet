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
        public FinancialEntityCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }
        public override void Create(BaseDTO baseDTO)
        {
            var entity = baseDTO as FinancialEntity;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_FINANCIAL_ENTITY_PR" };

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
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_FINANCIAL_ENTITY_PR" };
            sqlOperation.Parameters.Add(new SqlParameter("@P_ID", entity.ID));

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }
        public override List<T> RetrieveAll<T>()
        {
            var entities = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_FINANCIAL_ENTITIES_PR" };
            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            foreach (var row in results)
            {
                var entity = BuildFinancialEntity(row);
                entities.Add((T)Convert.ChangeType(entity, typeof(T)));
            }

            return entities;
        }
        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_FINANCIAL_ENTITY_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_FinancialEntityID", ID);

            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (results.Count > 0)
            {
                var entity = BuildFinancialEntity(results[0]);
                return (T)Convert.ChangeType(entity, typeof(T));
            }

            return default(T);
        }
        public override void Update(BaseDTO baseDTO)
        {
            var entity = baseDTO as FinancialEntity;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_FINANCIAL_ENTITY_PR" };

            sqlOperation.AddIntParam("@P_FinancialEntityID", entity.ID);
            sqlOperation.AddStringParameter("@P_EntityName", entity.EntityName);
            sqlOperation.AddStringParameter("@P_TaxID", entity.TaxID);
            sqlOperation.AddDoubleParam("@P_Latitude", entity.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", entity.Longitude);
            sqlOperation.AddStringParameter("@P_ContactPhone", entity.ContactPhone);
            sqlOperation.AddStringParameter("@P_Email", entity.Email);
            sqlOperation.AddDoubleParam("@P_CommissionPercentage", entity.CommissionPercentage);
            sqlOperation.AddStringParameter("@P_ValidationStatus", entity.ValidationStatus);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        private FinancialEntity BuildFinancialEntity(Dictionary<string, object> row)
        {
            return new FinancialEntity
            {
                ID = (int)row["FinancialEntityID"],
                EntityName = (string)row["EntityName"],
                TaxID = (string)row["TaxID"],
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
