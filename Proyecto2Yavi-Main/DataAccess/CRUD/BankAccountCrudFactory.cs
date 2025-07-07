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
    internal class BankAccountCrudFactory : CrudFactory
    {
        public override void Create(BaseDTO baseDTO)
        {
            var bankAccount = baseDTO as BankAccount;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_BANK_ACCOUNT_SP" };

            sqlOperation.AddIntParam("@P_UserID", bankAccount.UserID);
            sqlOperation.AddStringParameter("@P_IBAN", bankAccount.IBAN);
            sqlOperation.AddIntParam("@P_FinancialEntityID", bankAccount.FinancialEntityID);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var bankAccount = baseDTO as BankAccount;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_BANK_ACCOUNT_PR" };
            sqlOperation.AddIntParam("@P_BankAccountID", bankAccount.ID);

            _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }

        public override List<T> RetrieveAll<T>()
        {
            var lstBankAccounts = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_BANK_ACCOUNTS_PR" };
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            foreach (var row in lstResults)
            {
                var bankAccount = BuildBankAccount(row);
                lstBankAccounts.Add((T)Convert.ChangeType(bankAccount, typeof(T)));
            }

            return lstBankAccounts;
        }

        public override T RetrieveById<T>(int id)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_BANK_ACCOUNT_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_BankAccountID", id);
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                var bankAccount = BuildBankAccount(row);
                return (T)Convert.ChangeType(bankAccount, typeof(T));
            }

            return default;
        }

        private BankAccount BuildBankAccount(Dictionary<string, object> row)
        {
            return new BankAccount
            {
                ID = (int)row["BankAccountID"],
                UserID = (int)row["UserID"],
                IBAN = (string)row["IBAN"],
                FinancialEntityID = (int)row["FinancialEntityID"],
                Status = (bool)row["Status"],
                RegisteredAt = Convert.ToDateTime(row["RegisteredAt"])
            };
        }

        public override void Update(BaseDTO baseDTO)
        {
            var bankAccount = baseDTO as BankAccount;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_BANK_ACCOUNT_PR" };

            sqlOperation.AddIntParam("@P_BankAccountID", bankAccount.ID);
            sqlOperation.AddIntParam("@P_UserID", bankAccount.UserID);
            sqlOperation.AddStringParameter("@P_IBAN", bankAccount.IBAN);
            sqlOperation.AddIntParam("@P_FinancialEntityID", bankAccount.FinancialEntityID);
            sqlOperation.AddBoolParam("@P_Status", bankAccount.Status);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
    }
}