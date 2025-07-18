using DTOs;
using System.Collections.Generic;
using DataAccess.DAO.DataAccess.DAO;
using DataAccess.DAO;

namespace DataAccess.CRUD
{
    public class UserEntityCrudFactory : CrudFactory
    {
        public UserEntityCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO dto)
        {
            var entity = (UserEntity)dto;
            var op = new SqlOperation { ProcedureName = "CREATE_USER_FINANCIAL_ENTITY_LINK_PR" };
            op.AddIntParam("@P_UserID", entity.UserID);
            op.AddIntParam("@P_FinancialEntityID", entity.FinancialEntityID);
            _sqlDao.ExecuteProcedure(op);
        }

        public override void Delete(BaseDTO dto)
        {
            var entity = (UserEntity)dto;
            var op = new SqlOperation { ProcedureName = "DELETE_USER_FINANCIAL_ENTITY_LINK_PR" };
            op.AddIntParam("@P_UserID", entity.UserID);
            op.AddIntParam("@P_FinancialEntityID", entity.FinancialEntityID);
            _sqlDao.ExecuteProcedure(op);
        }

        public override List<T> RetrieveAll<T>()
        {
            var op = new SqlOperation { ProcedureName = "RET_ALL_USER_FINANCIAL_ENTITY_LINKS_PR" };
            var results = _sqlDao.ExecuteQueryProcedure(op);
            var list = new List<T>();
            foreach (var row in results)
            {
                var entity = new UserEntity
                {
                    UserID = (int)row["UserID"],
                    FinancialEntityID = (int)row["FinancialEntityID"]
                };
                list.Add((T)(object)entity);
            }
            return list;
        }

        public override T RetrieveById<T>(int id)
        {
            // No es común buscar por ID en tablas N:N, pero se implementa para cumplir la interfaz
            return default(T);
        }

        public override void Update(BaseDTO dto)
        {
            // No suele ser necesario para tablas de relación N:N
        }

        // Nuevo: obtener entidades financieras completas por usuario
        public List<FinancialEntity> RetrieveFinancialEntitiesByUserId(int userId)
        {
            var op = new SqlOperation { ProcedureName = "RET_FINANCIAL_ENTITIES_BY_USER_ID_PR" };
            op.AddIntParam("@P_UserID", userId);
            var results = _sqlDao.ExecuteQueryProcedure(op);
            var list = new List<FinancialEntity>();
            foreach (var row in results)
            {
                var entity = new FinancialEntity
                {
                    ID = (int)row["FinancialEntityID"],
                    EntityName = row["EntityName"].ToString(),
                    TaxID = row["TaxID"].ToString(),
                    Latitude = Convert.ToDouble(row["Latitude"]),
                    Longitude = Convert.ToDouble(row["Longitude"]),
                    ContactPhone = row["ContactPhone"].ToString(),
                    Email = row["Email"].ToString(),
                    CommissionPercentage = Convert.ToDouble(row["CommissionPercentage"]),
                    ValidationStatus = row["ValidationStatus"].ToString()
                };
                list.Add(entity);
            }
            return list;
        }
    }
}
