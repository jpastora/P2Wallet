using CoreApp;
using DTOs;
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
                return Ok("Transaction created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult UpdateTransaction(Transaction transaction)
        {
            try
            {
                var transactionManager = new TransactionManager();
                transactionManager.UpdateTransaction(transaction);
                return Ok("Transaction updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<Transaction>> RetrieveAllTransactions()
        {
            try
            {
                var transactionManager = new TransactionManager();
                var listTransactionResult = transactionManager.RetrieveAllTransactions();
                return Ok(listTransactionResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult<Transaction> RetrieveTransactionById(int id)
        {
            try
            {
                var transactionManager = new TransactionManager();
                var transactionResult = transactionManager.RetrieveTransactionById(id);
                if (transactionResult == null)
                {
                    return NotFound("Transaction not found.");
                }
                return Ok(transactionResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok("Transaction deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
