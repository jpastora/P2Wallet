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

            sqlOperation.AddStringParameter("@P_FirstName", user.FirstName);
            sqlOperation.AddStringParameter("@P_LastName", user.LastName);
            sqlOperation.AddStringParameter("@P_IDNumber", user.IDNumber);
            sqlOperation.AddDateTimeParam("@P_BirthDate", user.BirthDate);
            sqlOperation.AddStringParameter("@P_Email", user.Email);
            sqlOperation.AddStringParameter("@P_MobilePhone", user.MobilePhone);
            sqlOperation.AddStringParameter("@P_ProfilePhoto", user.ProfilePhotoUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoFront", user.IDPhotoFrontUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoBack", user.IDPhotoBackUrl);
            sqlOperation.AddDoubleParam("@P_Latitude", user.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", user.Longitude);
            sqlOperation.AddStringParameter("@P_Password", user.Password);
            sqlOperation.AddStringParameter("@P_EmailVerified", "Active"); 
            sqlOperation.AddStringParameter("@P_MobileVerified", "Active"); 

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

        public T RetrieveByEmail<T>(User user)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_USER_BY_EMAIL_PR" };
            sqlOperation.AddStringParameter("@P_Email", user.Email);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                user = BuildUser(row);
                return (T)Convert.ChangeType(user, typeof(T));
            }
            return default(T);
        }

        public T RetrieveByMobilePhone<T>(User user)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_USER_BY_MOBILE_PHONE_PR" };
            sqlOperation.AddStringParameter("@P_MobilePhone", user.MobilePhone);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                user = BuildUser(row);
                return (T)Convert.ChangeType(user, typeof(T));
            }
            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var user = baseDTO as User;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_USER_PR" };
            sqlOperation.AddIntParam("@P_UserID", user.ID);
            sqlOperation.AddStringParameter("@P_FirstName", user.FirstName);
            sqlOperation.AddStringParameter("@P_LastName", user.LastName);
            sqlOperation.AddStringParameter("@P_IDNumber", user.IDNumber);
            sqlOperation.AddDateTimeParam("@P_BirthDate", user.BirthDate);
            sqlOperation.AddStringParameter("@P_Email", user.Email);
            sqlOperation.AddStringParameter("@P_MobilePhone", user.MobilePhone);
            sqlOperation.AddStringParameter("@P_ProfilePhoto", user.ProfilePhotoUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoFront", user.IDPhotoFrontUrl);
            sqlOperation.AddStringParameter("@P_IDPhotoBack", user.IDPhotoBackUrl);
            sqlOperation.AddDoubleParam("@P_Latitude", user.Latitude);
            sqlOperation.AddDoubleParam("@P_Longitude", user.Longitude);
            sqlOperation.AddStringParameter("@P_Password", user.Password);
            sqlOperation.AddStringParameter("@P_EmailVerified", user.EmailVerified);
            sqlOperation.AddStringParameter("@P_MobileVerified", user.MobileVerified);
            sqlOperation.AddStringParameter("@P_BiometricVerified", user.BiometricVerified);
            sqlOperation.AddStringParameter("@P_ValidationStatus", user.ValidationStatus);
            sqlOperation.AddStringParameter("@P_SMSNotification", user.SMSNotification);
            sqlOperation.AddStringParameter("@P_EmailNotification", user.EmailNotification);
            sqlOperation.AddStringParameter("@P_PushNotification", user.PushNotification);
            sqlOperation.AddStringParameter("@P_Role", user.Role);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        public void ChangePassword(int userId, string newPassword)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "CHANGE_USER_PASSWORD_PR" };
            sqlOperation.AddIntParam("@P_UserID", userId);
            sqlOperation.AddStringParameter("@P_NewPassword", newPassword);
            _sqlDao.ExecuteProcedure(sqlOperation);
        }
        private User BuildUser(Dictionary<string, object> row)
        {
            var user = new User()
            {
                ID = row.ContainsKey("UserID") ? (row["UserID"] is DBNull ? 0 : (int)row["UserID"]) : 0,
                FirstName = row.ContainsKey("FirstName") ? Convert.ToString(row["FirstName"]) : null,
                LastName = row.ContainsKey("LastName") ? Convert.ToString(row["LastName"]) : null,
                IDNumber = row.ContainsKey("IDNumber") ? Convert.ToString(row["IDNumber"]) : null,
                BirthDate = row.ContainsKey("BirthDate") ? (row["BirthDate"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["BirthDate"])) : DateTime.MinValue,
                Email = row.ContainsKey("Email") ? Convert.ToString(row["Email"]) : null,
                MobilePhone = row.ContainsKey("MobilePhone") ? Convert.ToString(row["MobilePhone"]) : null,
                ProfilePhotoUrl = row.ContainsKey("ProfilePhoto") ? Convert.ToString(row["ProfilePhoto"]) : null,
                IDPhotoFrontUrl = row.ContainsKey("IDPhotoFront") ? Convert.ToString(row["IDPhotoFront"]) : null,
                IDPhotoBackUrl = row.ContainsKey("IDPhotoBack") ? Convert.ToString(row["IDPhotoBack"]) : null,
                Latitude = row.ContainsKey("Latitude") ? (row["Latitude"] is DBNull ? 0 : Convert.ToDouble(row["Latitude"])) : 0,
                Longitude = row.ContainsKey("Longitude") ? (row["Longitude"] is DBNull ? 0 : Convert.ToDouble(row["Longitude"])) : 0,
                Password = row.ContainsKey("Password") ? Convert.ToString(row["Password"]) : null,
                EmailVerified = row.ContainsKey("EmailVerified") ? Convert.ToString(row["EmailVerified"]) : null,
                MobileVerified = row.ContainsKey("MobileVerified") ? Convert.ToString(row["MobileVerified"]) : null,
                BiometricVerified = row.ContainsKey("BiometricVerified") ? Convert.ToString(row["BiometricVerified"]) : null,
                ValidationStatus = row.ContainsKey("ValidationStatus") ? Convert.ToString(row["ValidationStatus"]) : null,
                SMSNotification = row.ContainsKey("SMSNotification") ? Convert.ToString(row["SMSNotification"]) : null,
                EmailNotification = row.ContainsKey("EmailNotification") ? Convert.ToString(row["EmailNotification"]) : null,
                PushNotification = row.ContainsKey("PushNotification") ? Convert.ToString(row["PushNotification"]) : null,
                Role = row.ContainsKey("Role") ? Convert.ToString(row["Role"]) : null,
            };
            return user;
        }
    }
}
