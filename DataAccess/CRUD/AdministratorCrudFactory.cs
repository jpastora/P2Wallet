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
    public class AdministratorCrudFactory : CrudFactory
    {
        public AdministratorCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }
        public override void Create(BaseDTO baseDTO)
        {
            var administrator = baseDTO as Administrator;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_ADMINISTRATOR_PR" };

            sqlOperation.AddStringParameter("@P_Name", administrator.Name);
            sqlOperation.AddStringParameter("@P_AccessUsername", administrator.AcessUsername);
            sqlOperation.AddStringParameter("@P_AccessPassword", administrator.AccessPassword);
            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var administrator = baseDTO as Administrator;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_ADMINISTRATOR_PR" };
            sqlOperation.Parameters.Add(new SqlParameter("@P_AdminID", administrator.ID));

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }

        public override List<T> RetrieveAll<T>()
        {
            var lstAdministrators = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_ADMINISTRATORS_PR" };
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                foreach (var row in lstResults)
                {
                    var administrator = BuildAdministrator(row);
                    lstAdministrators.Add((T)Convert.ChangeType(administrator, typeof(T)));
                }
            }
            return lstAdministrators;
        }

        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ADMINISTRATOR_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_AdminID", ID);
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);
            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                var administrator = BuildAdministrator(row);
                return (T)Convert.ChangeType(administrator, typeof(T));
            }
            return default;

        }

        public override void Update(BaseDTO baseDTO)
        {
            var administrator = baseDTO as Administrator;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_ADMINISTRATOR_PR" };
            sqlOperation.AddIntParam("@P_AdminID", administrator.ID);
            sqlOperation.AddStringParameter("@P_Name", administrator.Name);
            sqlOperation.AddStringParameter("@P_AccessUsername", administrator.AcessUsername);
            sqlOperation.AddStringParameter("@P_AccessPassword", administrator.AccessPassword);
            sqlOperation.AddStringParameter("@P_ValidationStatus", administrator.ValidationStatus);
            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private Administrator BuildAdministrator(Dictionary<string, object> row)
        {

            var administrator = new Administrator()
            {
                ID = (int)row["AdminID"],
                Name = (string)row["Name"],
                AcessUsername = (string)row["AccessUsername"],
                AccessPassword = (string)row["AccessPassword"],
                ValidationStatus = (string)row["AdminStatus"]
            };
            return administrator;
        }
    }
}
