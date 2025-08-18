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
    // Gestiona las operaciones relacionadas con las promociones de comerciantes.
    public class MerchantPromotionManager : BaseManager
    {
        // Crea una nueva promoción para un comerciante.
        public void CreatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var crud = new MerchantPromotionCrudFactory();
                crud.Create(promotion); // Inserta la promoción en la base de datos
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Recupera todas las promociones de comerciantes del sistema.
        public List<MerchantPromotion> RetrieveAllPromotions()
        {
            var crud = new MerchantPromotionCrudFactory();
            return crud.RetrieveAll<MerchantPromotion>();
        }

        // Recupera una promoción de comerciante por su ID.
        public MerchantPromotion RetrievePromotionById(int id)
        {
            var crud = new MerchantPromotionCrudFactory();
            return crud.RetrieveById<MerchantPromotion>(id);
        }

        // Actualiza una promoción existente de comerciante.
        public void UpdatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var crud = new MerchantPromotionCrudFactory();
                crud.Update(promotion); // Actualiza la promoción en la base de datos
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex);
            }
        }

        // Elimina una promoción de comerciante.
        public void DeletePromotion(MerchantPromotion promotion)
        {
            try
            {
                var crud = new MerchantPromotionCrudFactory();
                crud.Delete(promotion); // Elimina la promoción de la base de datos
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex);
            }
        }
    }
}
