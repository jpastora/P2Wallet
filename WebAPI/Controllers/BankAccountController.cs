using CoreApp;
using DTOs;
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
                return Ok("Bank account created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Bank account updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<BankAccount>> RetrieveAllBankAccounts()
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                var listBankAccountResult = bankAccountManager.RetrieveAllBankAccounts();
                return Ok(listBankAccountResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult<BankAccount> RetrieveBankAccountById(int id)
        {
            try
            {
                var bankAccountManager = new BankAccountManager();
                var bankAccountResult = bankAccountManager.RetrieveBankAccountById(id);
                if (bankAccountResult == null)
                {
                    return NotFound("Bank account not found.");
                }
                return Ok(bankAccountResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Bank account deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
