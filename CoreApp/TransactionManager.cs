using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public class TransactionManager : BaseManager
    {
        // Crea una nueva transacción si el identificador no existe previamente.
        public void CreateTransaction(Transaction transaction)
        {
            try
            {
                var transactionCrud = new TransactionCrudFactory();
                if (transactionCrud.RetrieveById<Transaction>(transaction.ID) == null)
                {
                    transactionCrud.Create(transaction);
                }
                else
                {
                    throw new Exception("La transacción ya existe.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al crear la transacción: " + ex.Message);
            }
        }

        // Recupera la lista de todas las transacciones registradas.
        public List<Transaction> RetrieveAllTransactions()
        {
            var transactionCrud = new TransactionCrudFactory();
            return transactionCrud.RetrieveAll<Transaction>();
        }

        // Recupera una transacción por su identificador único.
        public Transaction RetrieveTransactionById(int id)
        {
            var transactionCrud = new TransactionCrudFactory();
            return transactionCrud.RetrieveById<Transaction>(id);
        }

        // Actualiza la información de una transacción existente.
        public void UpdateTransaction(Transaction transaction)
        {
            try
            {
                var transactionCrud = new TransactionCrudFactory();
                transactionCrud.Update(transaction);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Elimina una transacción.
        public void DeleteTransaction(Transaction transaction)
        {
            try
            {
                var transactionCrud = new TransactionCrudFactory();
                transactionCrud.Delete(transaction);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Crea una solicitud de pago que genera un código QR para que el usuario pueda pagar. Utilizado por comercios para solicitar pagos.
        // Devuelve la transacción con código QR generado
        public Transaction CreatePaymentRequest(int merchantId, decimal saleAmount, string description = "", int expirationMinutes = 30)
        {
            try
            {
                // Calcular la fecha de expiración en el servidor de aplicación para consistencia
                var expiresAt = DateTime.Now.AddMinutes(expirationMinutes);
                
                var transactionCrud = new TransactionCrudFactory();
                var paymentRequest = transactionCrud.CreatePaymentRequest(merchantId, saleAmount, description, expirationMinutes, expiresAt);
                
                if (paymentRequest == null)
                {
                    throw new Exception("No se pudo crear la solicitud de pago");
                }

                return paymentRequest;
            }
            catch (Exception ex)
            {
                ManageException(ex);
                throw new Exception("Error al crear solicitud de pago: " + ex.Message);
            }
        }

        // Obtiene una solicitud de pago activa por su código QR. Utilizado cuando un usuario escanea un código QR.
        // paymentRequestCode: Código único de la solicitud de pago
        // Devuelve la transacción correspondiente al código, o null si no se encuentra
        public Transaction GetPaymentRequestByCode(string paymentRequestCode)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentRequestCode))
                {
                    throw new ArgumentException("El código de solicitud de pago no puede estar vacío");
                }

                var transactionCrud = new TransactionCrudFactory();
                var paymentRequest = transactionCrud.RetrieveByPaymentCode(paymentRequestCode);
                
                return paymentRequest; // Puede ser null si no se encuentra o expiró
            }
            catch (Exception ex)
            {
                ManageException(ex);
                throw new Exception("Error al obtener solicitud de pago: " + ex.Message);
            }
        }

        // Ejecuta el pago de una solicitud aplicando promoción opcional.
        // Realiza toda la lógica de negocio: validación, descuentos, actualización de balances.
        // paymentRequestCode: Código de la solicitud de pago
        // userId: ID del usuario que va a pagar
        // bankAccountId: ID de la cuenta bancaria a usar
        // promotionId: ID de promoción a aplicar (opcional)
        // promotionType: "Merchant" o "Financial" (opcional)
        // Devuelve true si el pago se ejecutó exitosamente
        public bool ExecutePaymentWithPromotion(string paymentRequestCode, int userId, int bankAccountId, 
            int? promotionId = null, string promotionType = null)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrEmpty(paymentRequestCode))
                {
                    throw new ArgumentException("El código de solicitud de pago es requerido");
                }
                
                if (userId <= 0)
                {
                    throw new ArgumentException("ID de usuario inválido");
                }
                
                if (bankAccountId <= 0)
                {
                    throw new ArgumentException("ID de cuenta bancaria inválido");
                }

                // Validar tipo de promoción si se especificó
                if (promotionId.HasValue && !string.IsNullOrEmpty(promotionType))
                {
                    if (promotionType != "Merchant" && promotionType != "Financial")
                    {
                        throw new ArgumentException("Tipo de promoción debe ser 'Merchant' o 'Financial'");
                    }
                }

                var transactionCrud = new TransactionCrudFactory();
                bool success = transactionCrud.ExecutePaymentWithPromotion(
                    paymentRequestCode, userId, bankAccountId, promotionId, promotionType);

                return success;
            }
            catch (Exception ex)
            {
                ManageException(ex);
                return false;
            }
        }

        // Obtiene las promociones aplicables para una solicitud de pago específica.
        // Considera tanto promociones del comercio como de la entidad financiera.
        // merchantId: ID del comercio
        // financialEntityId: ID de la entidad financiera (opcional)
        // Devuelve la lista de promociones aplicables
        public List<object> GetApplicablePromotions(int merchantId, int? financialEntityId = null)
        {
            try
            {
                if (merchantId <= 0)
                {
                    throw new ArgumentException("ID de comercio inválido");
                }

                var transactionCrud = new TransactionCrudFactory();
                var promotions = transactionCrud.GetApplicablePromotions(merchantId, financialEntityId);

                return promotions ?? new List<object>();
            }
            catch (Exception ex)
            {
                ManageException(ex);
                return new List<object>();
            }
        }

        // Obtiene las promociones aplicables para una solicitud de pago usando el código QR.
        // Método de conveniencia que obtiene el comercio automáticamente.
        // paymentRequestCode: Código de la solicitud de pago
        // financialEntityId: ID de la entidad financiera (opcional)
        // Devuelve la lista de promociones aplicables
        public List<object> GetApplicablePromotionsForPayment(string paymentRequestCode, int? financialEntityId = null)
        {
            try
            {
                // Obtener la solicitud de pago para extraer el MerchantID
                var paymentRequest = GetPaymentRequestByCode(paymentRequestCode);
                
                if (paymentRequest == null)
                {
                    throw new Exception("Solicitud de pago no encontrada o expirada");
                }

                return GetApplicablePromotions(paymentRequest.MerchantID, financialEntityId);
            }
            catch (Exception ex)
            {
                ManageException(ex);
                return new List<object>();
            }
        }

        // Verifica si una solicitud de pago está vigente y disponible para uso.
        // paymentRequestCode: Código de la solicitud de pago
        // Devuelve true si la solicitud está vigente
        public bool IsPaymentRequestValid(string paymentRequestCode)
        {
            try
            {
                var paymentRequest = GetPaymentRequestByCode(paymentRequestCode);
                
                if (paymentRequest == null)
                {
                    return false;
                }

                // Verificar que esté en estado PendingUserApproval y no haya expirado
                bool isValidStatus = paymentRequest.TransactionStatus == "PendingUserApproval";
                bool isNotExpired = !paymentRequest.ExpiresAt.HasValue || paymentRequest.ExpiresAt.Value > DateTime.Now;

                return isValidStatus && isNotExpired;
            }
            catch (Exception ex)
            {
                ManageException(ex);
                return false;
            }
        }

        // Cancela una solicitud de pago pendiente. Útil para comercios que quieren cancelar una solicitud antes de que expire.
        // paymentRequestCode: Código de la solicitud de pago
        // Devuelve true si se canceló exitosamente
        public bool CancelPaymentRequest(string paymentRequestCode)
        {
            var paymentRequest = GetPaymentRequestByCode(paymentRequestCode);
            if (paymentRequest == null || paymentRequest.TransactionStatus != "PendingUserApproval")
                return false;

            var transactionCrud = new TransactionCrudFactory();
            transactionCrud.CancelPaymentRequest(paymentRequest.ID);
            return true;
        }
    }
}


