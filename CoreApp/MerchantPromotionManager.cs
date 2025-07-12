using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    // Manages operations related to merchant promotions.
    public class MerchantPromotionManager : BaseManager
    {
        // Creates a new merchant promotion.
        public void CreatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var crud = new MerchantPromotionCrudFactory();
                crud.Create(promotion); // Inserts the promotion into the database
            }
            catch (Exception ex)
            {
                ManageException(ex); // Handles the exception using the base method
            }
        }

        // Retrieves all merchant promotions from the system.
        public List<MerchantPromotion> RetrieveAllPromotions()
        {
            var crud = new MerchantPromotionCrudFactory();
            return crud.RetrieveAll<MerchantPromotion>();
        }

        // Retrieves a merchant promotion by its ID.
        public MerchantPromotion RetrievePromotionById(int id)
        {
            var crud = new MerchantPromotionCrudFactory();
            return crud.RetrieveById<MerchantPromotion>(id);
        }

        // Updates an existing merchant promotion.
        public void UpdatePromotion(MerchantPromotion promotion)
        {
            try
            {
                var crud = new MerchantPromotionCrudFactory();
                crud.Update(promotion); // Updates the promotion in the database
            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }

        // Deletes a merchant promotion.
        public void DeletePromotion(MerchantPromotion promotion)
        {
            try
            {
                var crud = new MerchantPromotionCrudFactory();
                crud.Delete(promotion); // Removes the promotion from the database
            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }
    }
}
