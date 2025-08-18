using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateBankAccount(BankAccount bankAccount)
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                bankAccountManager.CreateBankAccount(bankAccount);

                return Ok(new
                {
                    message = "Cuenta bancaria creada exitosamente.",
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
        public ActionResult UpdateBankAccount(BankAccount bankAccount)
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                bankAccountManager.UpdateBankAccount(bankAccount);

                return Ok(new
                {
                    message = "Cuenta bancaria actualizada exitosamente.",
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
        public ActionResult RetrieveAllBankAccounts()
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                var listBankAccountResult = bankAccountManager.RetrieveAllBankAccounts();

                return Ok(new
                {
                    message = "Cuentas bancarias recuperadas exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = listBankAccountResult
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
        public ActionResult RetrieveBankAccountById(int id)
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                var bankAccountResult = bankAccountManager.RetrieveBankAccountById(id);

                if (bankAccountResult == null)
                {
                    return NotFound(new
                    {
                        message = "Cuenta bancaria no encontrada.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Cuenta bancaria encontrada.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = bankAccountResult
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
        public ActionResult DeleteBankAccount(BankAccount bankAccount)
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                bankAccountManager.BankAccountDelete(bankAccount);

                return Ok(new
                {
                    message = "Cuenta bancaria eliminada exitosamente.",
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
