using DTOs;
using DataAccess.CRUD;
using System.Collections.Generic;

namespace CoreApp
{
    public class UserEntityManager : BaseManager
    {
        private readonly UserEntityCrudFactory _crudFactory;

        public UserEntityManager()
        {
            _crudFactory = new UserEntityCrudFactory();
        }

        public void CreateUserEntity(UserEntity userEntity)
        {
            _crudFactory.Create(userEntity);
        }

        public void DeleteUserEntity(UserEntity userEntity)
        {
            _crudFactory.Delete(userEntity);
        }

        public List<UserEntity> RetrieveAllUserEntities()
        {
            return _crudFactory.RetrieveAll<UserEntity>();
        }

        // Nuevo: obtener entidades financieras completas asociadas a un usuario
        public List<FinancialEntity> GetFinancialEntitiesForUser(int userId)
        {
            return _crudFactory.RetrieveFinancialEntitiesByUserId(userId);
        }
    }
}
