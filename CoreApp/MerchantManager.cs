using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public class MerchantManager : BaseManager
    {
        // Crea un nuevo comerciante si el identificador no existe previamente.
        public void CreateMerchant(User merchant)
        {
            try
            {
                var merchantCrud = new UserCrudFactory();
                if (merchantCrud.RetrieveById<User>(merchant.ID) == null)
                {
                    merchantCrud.Create(merchant);
                }
                else
                {
                    throw new Exception("El comerciante ya existe.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al crear el comerciante: " + ex.Message);
            }
        }
        // Recupera la lista de todos los comerciantes registrados.
        public List<User> RetrieveAllMerchants()
        {
            var merchantCrud = new UserCrudFactory();
            return merchantCrud.RetrieveAll<User>();
        }
        // Recupera un comerciante por su identificador único.
        public User RetrieveMerchantById(int id)
        {
            var merchantCrud = new UserCrudFactory();
            return merchantCrud.RetrieveById<User>(id);
        }
        // Actualiza la información de un comerciante existente.
        public void UpdateMerchant(User merchant)
        {
            try
            {
                var merchantCrud = new UserCrudFactory();
                merchantCrud.Update(merchant);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
    }
}
