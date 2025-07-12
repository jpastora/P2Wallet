using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    // Manages operations related to financial promotions.
    public class FinancialPromotionManager : BaseManager
    {
        // Creates a new financial promotion.
        public void CreatePromotion(FinancialPromotion promotion)
        {
            try
            {
                var crud = new FinancialPromotionCrudFactory();
                crud.Create(promotion); // Inserts the promotion into the database
            }
            catch (Exception ex)
            {
                ManageException(ex); // Handles the exception using the base method
            }
        }

        // Retrieves all financial promotions from the system.
        public List<FinancialPromotion> RetrieveAllPromotions()
        {
            var crud = new FinancialPromotionCrudFactory();
            return crud.RetrieveAll<FinancialPromotion>();
        }

        // Retrieves a financial promotion by its ID.
        public FinancialPromotion RetrievePromotionById(int id)
        {
            var crud = new FinancialPromotionCrudFactory();
            return crud.RetrieveById<FinancialPromotion>(id);
        }

        // Updates an existing financial promotion.
        public void UpdatePromotion(FinancialPromotion promotion)
        {
            try
            {
                var crud = new FinancialPromotionCrudFactory();
                crud.Update(promotion); // Updates the promotion in the database
            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }

        // Deletes a financial promotion.
        public void DeletePromotion(FinancialPromotion promotion)
        {
            try
            {
                var crud = new FinancialPromotionCrudFactory();
                crud.Delete(promotion); // Removes the promotion from the database
            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }
    }
}
