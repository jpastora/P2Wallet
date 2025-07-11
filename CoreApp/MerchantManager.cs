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
        public void CreateMerchant(Merchant merchant)
        {
            try
            {
                var merchantCrud = new MerchantCrudFactory();
                if (merchantCrud.RetrieveById<Merchant>(merchant.ID) == null)
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
        public List<Merchant> RetrieveAllMerchants()
        {
            var merchantCrud = new MerchantCrudFactory();
            return merchantCrud.RetrieveAll<Merchant>();
        }
        // Recupera un comerciante por su identificador único.
        public Merchant RetrieveMerchantById(int id)
        {
            var merchantCrud = new MerchantCrudFactory();
            return merchantCrud.RetrieveById<Merchant>(id);
        }
        // Actualiza la información de un comerciante existente.
        public void UpdateMerchant(Merchant merchant)
        {
            try
            {
                var merchantCrud = new MerchantCrudFactory();
                merchantCrud.Update(merchant);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Elimina un comerciante por su identificador único.
        public void DeleteMerchant(Merchant merchant)
        {
            try
            {
                var merchantCrud = new MerchantCrudFactory();
                merchantCrud.Delete(merchant);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
    }
}
