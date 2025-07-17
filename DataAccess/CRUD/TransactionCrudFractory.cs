using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

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
            sqlOperation.AddDoubleParam("@P_CommissionApplied", transaction.CommissionApplied);
            sqlOperation.AddDoubleParam("@P_SalesTaxAmount", transaction.SalesTaxAmount);
            sqlOperation.AddDoubleParam("@P_TaxRateApplied", transaction.TaxRateApplied);
            sqlOperation.Parameters.Add(new SqlParameter("@P_Timestamp", transaction.Timestamp));
            sqlOperation.AddStringParameter("@P_TransactionStatus", transaction.TransactionStatus);
            sqlOperation.Parameters.Add(new SqlParameter("@P_FinancialPromotionID", (object?)transaction.FinancialPromotionID ?? DBNull.Value));
            sqlOperation.Parameters.Add(new SqlParameter("@P_MerchantPromotionID", (object?)transaction.MerchantPromotionID ?? DBNull.Value));

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
            sqlOperation.AddDoubleParam("@P_CommissionApplied", transaction.CommissionApplied);
            sqlOperation.AddDoubleParam("@P_SalesTaxAmount", transaction.SalesTaxAmount);
            sqlOperation.AddDoubleParam("@P_TaxRateApplied", transaction.TaxRateApplied);
            sqlOperation.Parameters.Add(new SqlParameter("@P_Timestamp", transaction.Timestamp));
            sqlOperation.AddStringParameter("@P_TransactionStatus", transaction.TransactionStatus);
            sqlOperation.Parameters.Add(new SqlParameter("@P_FinancialPromotionID", (object?)transaction.FinancialPromotionID ?? DBNull.Value));
            sqlOperation.Parameters.Add(new SqlParameter("@P_MerchantPromotionID", (object?)transaction.MerchantPromotionID ?? DBNull.Value));

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private Transaction BuildTransaction(Dictionary<string, object> row)
        {
            double GetDouble(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? Convert.ToDouble(row[key]) : 0.0;
            }
            int? GetNullableInt(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? (int?)Convert.ToInt32(row[key]) : null;
            }
            string GetString(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? row[key].ToString() : string.Empty;
            }
            DateTime GetDateTime(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? Convert.ToDateTime(row[key]) : DateTime.MinValue;
            }

            return new Transaction
            {
                ID = row.ContainsKey("TransactionID") ? Convert.ToInt32(row["TransactionID"]) : 0,
                UserID = row.ContainsKey("UserID") ? Convert.ToInt32(row["UserID"]) : 0,
                MerchantID = row.ContainsKey("MerchantID") ? Convert.ToInt32(row["MerchantID"]) : 0,
                BankAccountID = row.ContainsKey("BankAccountID") ? Convert.ToInt32(row["BankAccountID"]) : 0,
                GrossAmount = GetDouble("GrossAmount"),
                NetAmount = GetDouble("NetAmount"),
                CommissionApplied = GetDouble("CommissionApplied"),
                SalesTaxAmount = GetDouble("SalesTaxAmount"),
                TaxRateApplied = GetDouble("TaxRateApplied"),
                Timestamp = GetDateTime("Timestamp"),
                TransactionStatus = GetString("TransactionStatus"),
                FinancialPromotionID = GetNullableInt("FinancialPromotionID"),
                MerchantPromotionID = GetNullableInt("MerchantPromotionID")
            };
        }
    }
}