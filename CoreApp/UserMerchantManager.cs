using DTOs;
using DataAccess.CRUD;
using System.Collections.Generic;

namespace CoreApp
{
    public class UserMerchantManager : BaseManager
    {
        private readonly UserMerchantCrudFactory _crudFactory;

        public UserMerchantManager()
        {
            _crudFactory = new UserMerchantCrudFactory();
        }

        public void CreateUserMerchant(UserMerchant userMerchant)
        {
            _crudFactory.Create(userMerchant);
        }

        public void DeleteUserMerchant(UserMerchant userMerchant)
        {
            _crudFactory.Delete(userMerchant);
        }

        public List<UserMerchant> RetrieveAllUserMerchants()
        {
            return _crudFactory.RetrieveAll<UserMerchant>();
        }

        // Nuevo: obtener comercios completos asociados a un usuario
        public List<Merchant> GetMerchantsForUser(int userId)
        {
            return _crudFactory.RetrieveMerchantsByUserId(userId);
        }
    }
}
