using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public class BankAccountCrudFactory : CrudFactory
    {
        public BankAccountCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO baseDTO)
        {
            var account = baseDTO as BankAccount;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_BANK_ACCOUNT_PR" };

            sqlOperation.AddIntParam("@P_UserID", account.UserID);
            sqlOperation.AddStringParameter("@P_IBAN", account.IBAN);
            sqlOperation.AddIntParam("@P_FinancialEntityID", account.FinancialEntityID);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var account = baseDTO as BankAccount;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_BANK_ACCOUNT_PR" };
            sqlOperation.AddIntParam("@P_BankAccountID", account.ID);

            _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }

        public override List<T> RetrieveAll<T>()
        {
            var accounts = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_BANK_ACCOUNTS_PR" };
            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            foreach (var row in results)
            {
                var account = BuildBankAccount(row);
                accounts.Add((T)Convert.ChangeType(account, typeof(T)));
            }

            return accounts;
        }

        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_BANK_ACCOUNT_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_BankAccountID", ID);

            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (results.Count > 0)
            {
                var account = BuildBankAccount(results[0]);
                return (T)Convert.ChangeType(account, typeof(T));
            }

            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var account = baseDTO as BankAccount;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_BANK_ACCOUNT_PR" };

            sqlOperation.AddIntParam("@P_BankAccountID", account.ID);
            sqlOperation.AddIntParam("@P_UserID", account.UserID);
            sqlOperation.AddStringParameter("@P_IBAN", account.IBAN);
            sqlOperation.AddIntParam("@P_FinancialEntityID", account.FinancialEntityID);
            sqlOperation.AddStringParameter("@P_ValidationStatus", account.ValidationStatus);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private BankAccount BuildBankAccount(Dictionary<string, object> row)
        {
            return new BankAccount
            {
                ID = (int)row["BankAccountID"],
                UserID = (int)row["UserID"],
                IBAN = (string)row["IBAN"],
                FinancialEntityID = (int)row["FinancialEntityID"],
                ValidationStatus = (string)row["Status"]
            };
        }
    }
}
