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
    // Gestiona las operaciones relacionadas con las promociones financieras.
    public class FinancialPromotionManager : BaseManager
    {
        // Crea una nueva promoción financiera.
        public void CreatePromotion(FinancialPromotion promotion)
        {
            try
            {
                var crud = new FinancialPromotionCrudFactory();
                crud.Create(promotion); // Inserta la promoción en la base de datos
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Recupera todas las promociones financieras del sistema.
        public List<FinancialPromotion> RetrieveAllPromotions()
        {
            var crud = new FinancialPromotionCrudFactory();
            return crud.RetrieveAll<FinancialPromotion>();
        }

        // Recupera una promoción financiera por su ID.
        public FinancialPromotion RetrievePromotionById(int id)
        {
            var crud = new FinancialPromotionCrudFactory();
            return crud.RetrieveById<FinancialPromotion>(id);
        }

        // Actualiza una promoción financiera existente.
        public void UpdatePromotion(FinancialPromotion promotion)
        {
            try
            {
                var crud = new FinancialPromotionCrudFactory();
                crud.Update(promotion); // Actualiza la promoción en la base de datos
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex);
            }
        }

        // Elimina una promoción financiera.
        public void DeletePromotion(FinancialPromotion promotion)
        {
            try
            {
                var crud = new FinancialPromotionCrudFactory();
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
