using DTOs;
using System.Collections.Generic;
using DataAccess.DAO.DataAccess.DAO;
using DataAccess.DAO;

namespace DataAccess.CRUD
{
    public class UserMerchantCrudFactory : CrudFactory
    {
        public UserMerchantCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO dto)
        {
            var entity = (UserMerchant)dto;
            var op = new SqlOperation { ProcedureName = "CREATE_USER_MERCHANT_LINK_PR" };
            op.AddIntParam("@P_UserID", entity.UserID);
            op.AddIntParam("@P_MerchantID", entity.MerchantID);
            _sqlDao.ExecuteProcedure(op);
        }

        public override void Delete(BaseDTO dto)
        {
            var entity = (UserMerchant)dto;
            var op = new SqlOperation { ProcedureName = "DELETE_USER_MERCHANT_LINK_PR" };
            op.AddIntParam("@P_UserID", entity.UserID);
            op.AddIntParam("@P_MerchantID", entity.MerchantID);
            _sqlDao.ExecuteProcedure(op);
        }

        public override List<T> RetrieveAll<T>()
        {
            var op = new SqlOperation { ProcedureName = "RET_ALL_USER_MERCHANT_LINKS_PR" };
            var results = _sqlDao.ExecuteQueryProcedure(op);
            var list = new List<T>();
            foreach (var row in results)
            {
                var entity = new UserMerchant
                {
                    UserID = (int)row["UserID"],
                    MerchantID = (int)row["MerchantID"]
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

        // Nuevo: obtener comercios completos por usuario
        public List<Merchant> RetrieveMerchantsByUserId(int userId)
        {
            var op = new SqlOperation { ProcedureName = "RET_MERCHANTS_BY_USER_ID_PR" };
            op.AddIntParam("@P_UserID", userId);
            var results = _sqlDao.ExecuteQueryProcedure(op);
            var list = new List<Merchant>();
            foreach (var row in results)
            {
                var merchant = new Merchant
                {
                    ID = (int)row["MerchantID"],
                    MerchantName = row["MerchantName"].ToString(),
                    TaxID = row["TaxID"].ToString(),
                    LogoImage = row["LogoImage"].ToString(),
                    Latitude = Convert.ToDouble(row["Latitude"]),
                    Longitude = Convert.ToDouble(row["Longitude"]),
                    ContactPhone = row["ContactPhone"].ToString(),
                    Email = row["Email"].ToString(),
                    CommissionPercentage = Convert.ToDouble(row["CommissionPercentage"]),
                    ValidationStatus = row["ValidationStatus"].ToString()
                };
                list.Add(merchant);
            }
            return list;
        }
    }
}
