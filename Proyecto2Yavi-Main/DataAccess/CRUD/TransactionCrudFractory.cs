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
    public class TransactionCrudFactory : CrudFactory
    {
        public TransactionCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO baseDTO)
        {
            var transaction = baseDTO as Transaction;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_TRANSACTION_PR" };

            sqlOperation.AddIntParam("@P_UserID", transaction.UserID);
            sqlOperation.AddIntParam("@P_MerchantID", transaction.MerchantID);
            sqlOperation.AddIntParam("@P_BankAccountID", transaction.BankAccountID);
            sqlOperation.AddDoubleParam("@P_GrossAmount", transaction.GrossAmount);
            sqlOperation.AddDoubleParam("@P_NetAmount", transaction.NetAmount);
            sqlOperation.AddDoubleParam("@P_DiscountApplied", transaction.DiscountApplied);
            sqlOperation.AddDoubleParam("@P_CommissionApplied", transaction.CommissionApplied);
            sqlOperation.AddStringParameter("@P_TransactionStatus", transaction.TransactionStatus);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var transaction = baseDTO as Transaction;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_TRANSACTION_PR" };
            sqlOperation.AddIntParam("@P_ID", transaction.ID);

            _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }

        public override List<T> RetrieveAll<T>()
        {
            var lstTransactions = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_TRANSACTIONS_PR" };
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            foreach (var row in lstResults)
            {
                var transaction = BuildTransaction(row);
                lstTransactions.Add((T)Convert.ChangeType(transaction, typeof(T)));
            }

            return lstTransactions;
        }

        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_TRANSACTION_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_TransactionID", ID);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var transaction = BuildTransaction(lstResults[0]);
                return (T)Convert.ChangeType(transaction, typeof(T));
            }

            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var transaction = baseDTO as Transaction;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_TRANSACTION_pr" };

            sqlOperation.AddIntParam("@P_TransactionID", transaction.ID);
            sqlOperation.AddIntParam("@P_UserID", transaction.UserID);
            sqlOperation.AddIntParam("@P_MerchantID", transaction.MerchantID);
            sqlOperation.AddIntParam("@P_BankAccountID", transaction.BankAccountID);
            sqlOperation.AddDoubleParam("@P_GrossAmount", transaction.GrossAmount);
            sqlOperation.AddDoubleParam("@P_NetAmount", transaction.NetAmount);
            sqlOperation.AddDoubleParam("@P_DiscountApplied", transaction.DiscountApplied);
            sqlOperation.AddDoubleParam("@P_CommissionApplied", transaction.CommissionApplied);
            sqlOperation.AddStringParameter("@P_TransactionStatus", transaction.TransactionStatus);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private Transaction BuildTransaction(Dictionary<string, object> row)
        {
            return new Transaction
            {
                ID = (int)row["TransactionID"],
                UserID = (int)row["UserID"],
                MerchantID = (int)row["MerchantID"],
                BankAccountID = (int)row["BankAccountID"],
                GrossAmount = Convert.ToDouble(row["GrossAmount"]),
                NetAmount = Convert.ToDouble(row["NetAmount"]),
                DiscountApplied = Convert.ToDouble(row["DiscountApplied"]),
                CommissionApplied = Convert.ToDouble(row["CommissionApplied"]),
                TransactionStatus = (string)row["TransactionStatus"]
            };
        }
    }
}