using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentRequestController : ControllerBase
    {


        // DTO para crear una nueva solicitud de pago
        public class CreatePaymentRequestDto
        {
            public int MerchantId { get; set; }
            public decimal SaleAmount { get; set; }
            public string Description { get; set; } = string.Empty;
            public int ExpirationMinutes { get; set; } = 30;
        }

        // DTO para ejecutar un pago con promoción opcional
        public class ExecutePaymentDto
        {
            public string PaymentRequestCode { get; set; } = string.Empty;
            public int UserId { get; set; }
            public int BankAccountId { get; set; }
            public int? PromotionId { get; set; }
            public string? PromotionType { get; set; } // "Merchant" o "Financial"
        }

        // DTO para response de solicitud de pago creada
        public class PaymentRequestResponse
        {
            public int TransactionId { get; set; }
            public string PaymentRequestCode { get; set; } = string.Empty;
            public string QRCodeUrl { get; set; } = string.Empty;
            public string QRContent { get; set; } = string.Empty;
            public decimal GrossAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal SalesTaxAmount { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        // DTO para response de pago ejecutado
        public class PaymentExecutionResponse
        {
            public bool Success { get; set; }
            public int TransactionId { get; set; }
            public decimal FinalAmount { get; set; }
            public decimal DiscountApplied { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        ///Crea una nueva solicitud de pago con código QR
        // Solicitud creada con código QR
        [HttpPost]
        [Route("Create")]
        public ActionResult<PaymentRequestResponse> CreatePaymentRequest(CreatePaymentRequestDto request)
        {
            try
            {
                // Validar parámetros
                if (request.MerchantId <= 0)
                {
                    return BadRequest("ID de comercio inválido");
                }

                if (request.SaleAmount <= 0)
                {
                    return BadRequest("El monto de venta debe ser mayor a cero");
                }

                if (request.ExpirationMinutes < 5 || request.ExpirationMinutes > 120)
                {
                    return BadRequest("La expiración debe estar entre 5 y 120 minutos");
                }

                // Crear solicitud de pago
                var transactionManager = new TransactionManager();
                var paymentRequest = transactionManager.CreatePaymentRequest(
                    request.MerchantId, 
                    request.SaleAmount, 
                    request.Description ?? string.Empty
                );

                if (paymentRequest == null)
                {
                    return StatusCode(500, "No se pudo crear la solicitud de pago");
                }

                // Generar código QR
                var qrCodeUrl = QRCodeHelper.GeneratePaymentQR(paymentRequest.PaymentRequestCode);
                var qrContent = QRCodeHelper.GeneratePaymentQRContent(paymentRequest.PaymentRequestCode);

                // Crear response
                var response = new PaymentRequestResponse
                {
                    TransactionId = paymentRequest.ID,
                    PaymentRequestCode = paymentRequest.PaymentRequestCode,
                    QRCodeUrl = qrCodeUrl,
                    QRContent = qrContent,
                    GrossAmount = (decimal)paymentRequest.GrossAmount,
                    NetAmount = (decimal)paymentRequest.NetAmount,
                    SalesTaxAmount = (decimal)paymentRequest.SalesTaxAmount,
                    ExpiresAt = paymentRequest.ExpiresAt ?? DateTime.Now.AddMinutes(request.ExpirationMinutes),
                    Status = paymentRequest.TransactionStatus,
                    Description = paymentRequest.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear solicitud de pago: {ex.Message}");
            }
        }


        // Obtiene una solicitud de pago por su código QR
        //Datos de la solicitud de pago
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ActionResult<PaymentRequestResponse> GetPaymentRequestByCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Código de solicitud requerido");
                }

                var transactionManager = new TransactionManager();
                var paymentRequest = transactionManager.GetPaymentRequestByCode(code);

                if (paymentRequest == null)
                {
                    return NotFound("Solicitud de pago no encontrada o expirada");
                }

                // Verificar si está vigente
                bool isValid = transactionManager.IsPaymentRequestValid(code);
                if (!isValid)
                {
                    return BadRequest("La solicitud de pago ha expirado o no está disponible");
                }

                // Generar QR para response
                var qrCodeUrl = QRCodeHelper.GeneratePaymentQR(paymentRequest.PaymentRequestCode);
                var qrContent = QRCodeHelper.GeneratePaymentQRContent(paymentRequest.PaymentRequestCode);

                var response = new PaymentRequestResponse
                {
                    TransactionId = paymentRequest.ID,
                    PaymentRequestCode = paymentRequest.PaymentRequestCode,
                    QRCodeUrl = qrCodeUrl,
                    QRContent = qrContent,
                    GrossAmount = (decimal)paymentRequest.GrossAmount,
                    NetAmount = (decimal)paymentRequest.NetAmount,
                    SalesTaxAmount = (decimal)paymentRequest.SalesTaxAmount,
                    ExpiresAt = paymentRequest.ExpiresAt ?? DateTime.Now.AddMinutes(30),
                    Status = paymentRequest.TransactionStatus,
                    Description = paymentRequest.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener solicitud de pago: {ex.Message}");
            }
        }

        /// Obtiene las promociones aplicables para una solicitud de pago específica
        // Código de la solicitud de pago
        // ID de la entidad financiera
        // Lista de promociones aplicables
        [HttpGet]
        [Route("GetPromotions/{code}")]
        public ActionResult<List<object>> GetApplicablePromotions(string code, [FromQuery] int? financialEntityId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Código de solicitud requerido");
                }

                var transactionManager = new TransactionManager();
                
                // Verificar que la solicitud sea válida
                bool isValid = transactionManager.IsPaymentRequestValid(code);
                if (!isValid)
                {
                    return BadRequest("La solicitud de pago no es válida o ha expirado");
                }

                // Obtener promociones
                var promotions = transactionManager.GetApplicablePromotionsForPayment(code, financialEntityId);

                return Ok(promotions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener promociones: {ex.Message}");
            }
        }

        /// <summary>
        /// Ejecuta el pago de una solicitud aplicando promoción opcional
        /// </summary>
        /// <param name="request">Datos para ejecutar el pago</param>
        /// <returns>Resultado de la ejecución del pago</returns>
        [HttpPost]
        [Route("Execute")]
        public ActionResult<PaymentExecutionResponse> ExecutePayment(ExecutePaymentDto request)
        {
            try
            {
                // Validar parámetros
                if (string.IsNullOrEmpty(request.PaymentRequestCode))
                {
                    return BadRequest("Código de solicitud requerido");
                }

                if (request.UserId <= 0)
                {
                    return BadRequest("ID de usuario inválido");
                }

                if (request.BankAccountId <= 0)
                {
                    return BadRequest("ID de cuenta bancaria inválido");
                }

                // Validar promoción si se especificó
                if (request.PromotionId.HasValue)
                {
                    if (string.IsNullOrEmpty(request.PromotionType) || 
                        (request.PromotionType != "Merchant" && request.PromotionType != "Financial"))
                    {
                        return BadRequest("Tipo de promoción debe ser 'Merchant' o 'Financial'");
                    }
                }

                var transactionManager = new TransactionManager();
                
                // Verificar que la solicitud sea válida antes de ejecutar
                bool isValid = transactionManager.IsPaymentRequestValid(request.PaymentRequestCode);
                if (!isValid)
                {
                    return BadRequest("La solicitud de pago no es válida, ha expirado o ya fue procesada");
                }

                // Obtener solicitud original para cálculos
                var originalRequest = transactionManager.GetPaymentRequestByCode(request.PaymentRequestCode);
                if (originalRequest == null)
                {
                    return NotFound("Solicitud de pago no encontrada");
                }

                // Ejecutar el pago
                bool success = transactionManager.ExecutePaymentWithPromotion(
                    request.PaymentRequestCode, 
                    request.UserId, 
                    request.BankAccountId, 
                    request.PromotionId, 
                    request.PromotionType
                );

                if (success)
                {
                    // Obtener la transacción actualizada para ver el resultado final
                    var updatedTransaction = transactionManager.GetPaymentRequestByCode(request.PaymentRequestCode);
                    
                    var response = new PaymentExecutionResponse
                    {
                        Success = true,
                        TransactionId = originalRequest.ID,
                        FinalAmount = updatedTransaction != null ? (decimal)updatedTransaction.GrossAmount : (decimal)originalRequest.GrossAmount,
                        DiscountApplied = 0, // Se calculará en el SP
                        Message = "Pago ejecutado exitosamente"
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest("No se pudo ejecutar el pago. Verifique los datos o fondos insuficientes.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al ejecutar pago: {ex.Message}");
            }
        }

        // Cancela una solicitud de pago pendiente
        // Código de la solicitud a cancelar
        // Resultado de la cancelación
        [HttpPost]
        [Route("Cancel/{code}")]
        public ActionResult CancelPaymentRequest(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Código de solicitud requerido");
                }

                var transactionManager = new TransactionManager();
                bool cancelled = transactionManager.CancelPaymentRequest(code);

                if (cancelled)
                {
                    return Ok("Solicitud de pago cancelada exitosamente");
                }
                else
                {
                    return BadRequest("No se pudo cancelar la solicitud. Puede que ya esté procesada o no exista.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cancelar solicitud: {ex.Message}");
            }
        }

        // Valida si una solicitud de pago está vigente
        // Estado de validez de la solicitud
        [HttpGet]
        [Route("Validate/{code}")]
        public ActionResult<object> ValidatePaymentRequest(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Código de solicitud requerido");
                }

                var transactionManager = new TransactionManager();
                bool isValid = transactionManager.IsPaymentRequestValid(code);
                
                var response = new 
                {
                    Code = code,
                    IsValid = isValid,
                    Message = isValid ? "Solicitud válida" : "Solicitud expirada o no válida",
                    CheckedAt = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al validar solicitud: {ex.Message}");
            }
        }
    }
}