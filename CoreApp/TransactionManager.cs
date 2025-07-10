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
    }
}


