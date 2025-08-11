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
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_TRANSACTION_PR" };

            sqlOperation.AddIntParam("@P_TransactionID", transaction.ID);
            sqlOperation.AddIntParam("@P_UserID", transaction.UserID);
            sqlOperation.AddIntParam("@P_MerchantID", transaction.MerchantID);
            sqlOperation.AddIntParam("@P_BankAccountID", transaction.BankAccountID);
            sqlOperation.AddDoubleParam("@P_GrossAmount", transaction.GrossAmount);
            sqlOperation.AddDoubleParam("@P_NetAmount", transaction.NetAmount);
            sqlOperation.AddDoubleParam("@P_CommissionApplied", transaction.CommissionApplied);
            sqlOperation.AddDoubleParam("@P_SalesTaxAmount", transaction.SalesTaxAmount);
            sqlOperation.AddDoubleParam("@P_TaxRateApplied", transaction.TaxRateApplied);
            sqlOperation.AddStringParameter("@P_TransactionStatus", transaction.TransactionStatus);
            sqlOperation.Parameters.Add(new SqlParameter("@P_FinancialPromotionID", (object?)transaction.FinancialPromotionID ?? DBNull.Value));
            sqlOperation.Parameters.Add(new SqlParameter("@P_MerchantPromotionID", (object?)transaction.MerchantPromotionID ?? DBNull.Value));

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        // Crea una solicitud de pago que genera un código QR para que el usuario pueda pagar
        public Transaction CreatePaymentRequest(int merchantId, decimal saleAmount, string description = "", int expirationMinutes = 30, DateTime? expiresAt = null)
        {
                var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_PAYMENT_REQUEST_PR" };

                sqlOperation.AddIntParam("@P_MerchantID", merchantId);
                sqlOperation.AddDoubleParam("@P_SaleAmount", (double)saleAmount);
                sqlOperation.AddStringParameter("@P_Description", description);
                sqlOperation.AddIntParam("@P_ExpirationMinutes", expirationMinutes);

                // CORRECCIÓN: Si se proporciona fecha de expiración específica, usarla
                if (expiresAt.HasValue)
                {
                    sqlOperation.Parameters.Add(new SqlParameter("@P_ExpiresAt", expiresAt.Value));
                }

                // Parámetros de salida
                sqlOperation.Parameters.Add(new SqlParameter("@P_TransactionID", SqlDbType.Int) { Direction = ParameterDirection.Output });
                sqlOperation.Parameters.Add(new SqlParameter("@P_PaymentRequestCode", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output });

                var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

                if (results.Count > 0)
                {
                    return BuildTransaction(results[0]);
                }

                return null;
            }
        

        // Obtiene una solicitud de pago activa por su código QR

        public Transaction RetrieveByPaymentCode(string paymentRequestCode)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_TRANSACTION_BY_PAYMENT_CODE_PR" };
            sqlOperation.AddStringParameter("@P_PaymentRequestCode", paymentRequestCode);

            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (results.Count > 0)
            {
                return BuildTransaction(results[0]);
            }

            return null;
        }


        // Ejecuta el pago de una solicitud aplicando promoción opcional

        public bool ExecutePaymentWithPromotion(string paymentRequestCode, int userId, int bankAccountId, int? promotionId = null, string promotionType = null)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "EXECUTE_PAYMENT_WITH_PROMOTION_PR" };
            
            sqlOperation.AddStringParameter("@P_PaymentRequestCode", paymentRequestCode);
            sqlOperation.AddIntParam("@P_UserID", userId);
            sqlOperation.AddIntParam("@P_BankAccountID", bankAccountId);
            sqlOperation.Parameters.Add(new SqlParameter("@P_SelectedPromotionID", (object?)promotionId ?? DBNull.Value));
            sqlOperation.AddStringParameter("@P_SelectedPromotionType", promotionType ?? "");

            try
            {
                var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);
                return results.Count > 0;
            }
            catch
            {
                return false;
            }
        }


        // Obtiene las promociones aplicables para un comercio y entidad financiera específicos

        public List<object> GetApplicablePromotions(int merchantId, int? financialEntityId = null)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "GET_APPLICABLE_PROMOTIONS_PR" };
            sqlOperation.AddIntParam("@P_MerchantID", merchantId);
            
            if (financialEntityId.HasValue)
            {
                sqlOperation.AddIntParam("@P_FinancialEntityID", financialEntityId.Value);
            }

            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);
            var promotions = new List<object>();

            foreach (var row in results)
            {
                promotions.Add(new
                {
                    PromotionID = Convert.ToInt32(row["PromotionID"]),
                    PromotionType = row["PromotionType"].ToString(),
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString(),
                    DiscountPercentage = Convert.ToDecimal(row["DiscountPercentage"]),
                    MaxRefund = Convert.ToDecimal(row["MaxRefund"]),
                    AvailableQuantity = Convert.ToInt32(row["AvailableQuantity"]),
                    SourceName = row["SourceName"].ToString()
                });
            }

            return promotions;
        }

        public void CancelPaymentRequest(int transactionId)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "CANCEL_PAYMENT_REQUEST_PR" };
            sqlOperation.AddIntParam("@P_TransactionID", transactionId);
            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private Transaction BuildTransaction(Dictionary<string, object> row)
        {
            double GetDouble(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? Convert.ToDouble(row[key]) : 0.0;
            }
            
            int GetInt(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? Convert.ToInt32(row[key]) : 0;
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
            
            DateTime? GetNullableDateTime(string key)
            {
                return row.ContainsKey(key) && row[key] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row[key]) : null;
            }

            return new Transaction
            {
                ID = GetInt("TransactionID"),
                UserID = GetInt("UserID"),
                MerchantID = GetInt("MerchantID"),
                BankAccountID = GetInt("BankAccountID"),
                GrossAmount = GetDouble("GrossAmount"),
                NetAmount = GetDouble("NetAmount"),
                CommissionApplied = GetDouble("CommissionApplied"),
                SalesTaxAmount = GetDouble("SalesTaxAmount"),
                TaxRateApplied = GetDouble("TaxRateApplied"),
                Timestamp = GetDateTime("Timestamp"),
                TransactionStatus = GetString("TransactionStatus"),
                FinancialPromotionID = GetNullableInt("FinancialPromotionID"),
                MerchantPromotionID = GetNullableInt("MerchantPromotionID"),
                
                // Nuevos campos para solicitudes de pago QR
                PaymentRequestCode = GetString("PaymentRequestCode"),
                Description = GetString("Description"),
                ExpiresAt = GetNullableDateTime("ExpiresAt")
            };
        }
    }
}