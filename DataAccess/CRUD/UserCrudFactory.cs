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
    public class UserCrudFactory : CrudFactory
    {
        public UserCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }
        public override void Create(BaseDTO baseDTO)
        {
            var user = baseDTO as User;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_USER_PR" };

            sqlOperation.AddStringParameter("@P_FullName", user.FullName);
            sqlOperation.AddStringParameter("@P_Email", user.Email);
            sqlOperation.AddStringParameter("@P_MobilePhone", user.MobilePhone);
            sqlOperation.AddStringParameter("@P_IDPhotoFront", user.IDPhotoFrontUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoBack", user.IDPhotoBackUrl);
            sqlOperation.AddDoubleParam("@P_Latitude", user.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", user.Longitude);
            sqlOperation.AddStringParameter("@P_Password", user.Password);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        public override void Delete(BaseDTO baseDTO)
        {
            var user = baseDTO as User;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_USER_PR" };
            sqlOperation.Parameters.Add(new SqlParameter("@P_ID", user.ID));

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }
        public override List<T> RetrieveAll<T>()
        {
            var lstUsers = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_USERS_PR" };
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                foreach (var row in lstResults)
                {
                    var user = BuildUser(row);
                    lstUsers.Add((T)Convert.ChangeType(user, typeof(T)));
                }
            }
            return lstUsers;
        }
        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_USER_BY_ID_PR" };
            sqlOperation.AddIntParam("P_UserID", ID);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                var user = BuildUser(row);

                return (T)Convert.ChangeType(user, typeof(T));
            }
            return default(T);
        }
        public override void Update(BaseDTO baseDTO)
        {
            var user = baseDTO as User;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_USER_PR" };
            sqlOperation.AddIntParam("@P_UserID", user.ID);
            sqlOperation.AddStringParameter("@P_FullName", user.FullName);
            sqlOperation.AddStringParameter("@P_Email", user.Email);
            sqlOperation.AddStringParameter("@P_MobilePhone", user.MobilePhone);
            sqlOperation.AddStringParameter("@P_ProfilePhoto", user.ProfilePhotoUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoFront", user.IDPhotoFrontUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoBack", user.IDPhotoBackUrl);
            sqlOperation.AddDoubleParam("@P_Latitude", user.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", user.Longitude);
            sqlOperation.AddStringParameter("@P_Password", user.Password);
            sqlOperation.AddBoolParam("@P_EmailVerified", user.EmailVerified);
            sqlOperation.AddBoolParam("@P_MobileVerified", user.MobileVerified);
            sqlOperation.AddBoolParam("@P_BiometricVerified", user.BiometricVerified);
            sqlOperation.AddBoolParam("@P_UserStatus", user.UserStatus);
            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        private User BuildUser(Dictionary<string, object> row)
        {
            var user = new User()
            {
                ID = (int)row["UserID"],
                FullName = (string)row["FullName"],
                Email = (string)row["Email"],
                MobilePhone = (string)row["MobilePhone"],
                ProfilePhotoUrl = (string)row["ProfilePhoto"],
                IDPhotoFrontUrl = (string)row["IDPhotoFront"],
                IDPhotoBackUrl = (string)row["IDPhotoBack"],
                Latitude = Convert.ToDouble(row["Latitude"]),
                Longitude = Convert.ToDouble(row["Longitude"]),
                Password = (string)row["Password"],
                UserStatus = (bool)row["UserStatus"]

            };
            return user;
        }
    }
}
