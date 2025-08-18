using DataAccess.CRUD;
using DTOs;
using Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public class AccountMovementManager : BaseManager
    {
        public void CreateAccountMovement(AccountMovement movement)
        {
            try
            {
                var crud = new AccountMovementCrudFactory();
                if (crud.RetrieveById<AccountMovement>(movement.ID) == null)
                {
                    crud.Create(movement);
                }
                else
                {
                    throw new Exception("El movimiento ya existe.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw new Exception("Ocurrió un error al crear el movimiento: " + ex.Message);
            }
        }

        public List<AccountMovement> RetrieveAllAccountMovements()
        {
            var crud = new AccountMovementCrudFactory();
            return crud.RetrieveAll<AccountMovement>();
        }

        public AccountMovement RetrieveAccountMovementById(int id)
        {
            var crud = new AccountMovementCrudFactory();
            return crud.RetrieveById<AccountMovement>(id);
        }

        public void UpdateAccountMovement(AccountMovement movement)
        {
            try
            {
                var crud = new AccountMovementCrudFactory();
                crud.Update(movement);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex);
            }
        }

        public void DeleteAccountMovement(AccountMovement movement)
        {
            try
            {
                var crud = new AccountMovementCrudFactory();
                crud.Delete(movement);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex);
            }
        }
    }
}
