using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateTransaction(Transaction transaction)
        {
            try
            {
                var transactionManager = new TransactionManager();
                transactionManager.CreateTransaction(transaction);

                return Ok(new
                {
                    message = "Transacción creada exitosamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult UpdateTransaction(Transaction transaction)
        {
            try
            {
                var transactionManager = new TransactionManager();
                transactionManager.UpdateTransaction(transaction);

                return Ok(new
                {
                    message = "Transacción actualizada exitosamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult RetrieveAllTransactions()
        {
            try
            {
                var transactionManager = new TransactionManager();
                var listTransactionResult = transactionManager.RetrieveAllTransactions();

                return Ok(new
                {
                    message = "Transacciones recuperadas exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = listTransactionResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult RetrieveTransactionById(int id)
        {
            try
            {
                var transactionManager = new TransactionManager();
                var transactionResult = transactionManager.RetrieveTransactionById(id);

                if (transactionResult == null)
                {
                    return NotFound(new
                    {
                        message = "Transacción no encontrada.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Transacción encontrada.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = transactionResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult DeleteTransaction(Transaction transaction)
        {
            try
            {
                var transactionManager = new TransactionManager();
                transactionManager.DeleteTransaction(transaction);

                return Ok(new
                {
                    message = "Transacción eliminada exitosamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }
    }
}
