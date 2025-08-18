using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptions;


namespace CoreApp
{
    public class BankAccountManager : BaseManager
    {
        // Crea una nueva cuenta bancaria si el identificador no existe previamente.
        public void CreateBankAccount(BankAccount bankAccount)
        {
            try
            {
                var bankAccountCrud = new BankAccountCrudFactory();
                if (bankAccountCrud.RetrieveById<BankAccount>(bankAccount.ID) == null)
                {
                    bankAccountCrud.Create(bankAccount);
                }
                else
                {
                    throw new Exception("La cuenta bancaria ya existe.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw new Exception("Ocurrió un error al crear la cuenta bancaria: " + ex.Message);
            }
        }

        // Recupera la lista de todas las cuentas bancarias registradas.
        public List<BankAccount> RetrieveAllBankAccounts()
        {
            var bankAccountCrud = new BankAccountCrudFactory();
            return bankAccountCrud.RetrieveAll<BankAccount>();
        }

        // Recupera una cuenta bancaria por su identificador único.
        public BankAccount RetrieveBankAccountById(int id)
        {
            var bankAccountCrud = new BankAccountCrudFactory();
            return bankAccountCrud.RetrieveById<BankAccount>(id);
        }

        // Actualiza la información de una cuenta bancaria existente.
        public void UpdateBankAccount(BankAccount bankAccount)
        {
            try
            {
                var bankAccountCrud = new BankAccountCrudFactory();
                bankAccountCrud.Update(bankAccount);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Elimina una cuenta bancaria por su identificador único.
        public void BankAccountDelete(BankAccount bankAccount)
        {
            try
            {
                var bankAccountCrud = new BankAccountCrudFactory();
                bankAccountCrud.Delete(bankAccount);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
    }
}
