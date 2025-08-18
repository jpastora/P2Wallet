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
    public class FinancialEntityManager : BaseManager
    {
        public void CreateFinancialEntity(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityCrud = new FinancialEntityCrudFactory();
                if (financialEntityCrud.RetrieveById<FinancialEntity>(financialEntity.ID) == null)
                {
                    financialEntityCrud.Create(financialEntity);
                }
                else
                {
                    throw new Exception("La entidad financiera ya existe.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
        public List<FinancialEntity> RetrieveAllFinancialEntities()
        {
            var financialEntityCrud = new FinancialEntityCrudFactory();
            return financialEntityCrud.RetrieveAll<FinancialEntity>();
        }
        public FinancialEntity RetrieveFinancialEntityById(int id)
        {
            var financialEntityCrud = new FinancialEntityCrudFactory();
            return financialEntityCrud.RetrieveById<FinancialEntity>(id);
        }
        public void UpdateFinancialEntity(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityCrud = new FinancialEntityCrudFactory();
                financialEntityCrud.Update(financialEntity);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
        public void DeleteFinancialEntity(FinancialEntity financialEntity)
        {
            try
            {
                var financialEntityCrud = new FinancialEntityCrudFactory();
                financialEntityCrud.Delete(financialEntity);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        public FinancialEntity RetrieveByTaxID(string taxId)
        {
            var financialEntityCrud = new FinancialEntityCrudFactory();
            return financialEntityCrud.RetrieveByTaxID<FinancialEntity>(taxId);
        }
    }
}
