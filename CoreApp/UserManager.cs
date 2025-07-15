using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    // Administra las operaciones relacionadas con usuarios.
    public class UserManager : BaseManager
    {
        // Crea un nuevo usuario si el correo electrónico no existe previamente.
        public void CreateUser(User user)
        {
            try
            {
                var userCrud = new UserCrudFactory();

                // Consultamos si en la base de datos existe un usuario con ese correo electrónico
                var userExist = userCrud.RetrieveByEmail<User>(user);

                if (userExist == null)
                    userCrud.Create(user); // Si no existe, crea el usuario
                else
                    throw new Exception("User already exists with this email."); // Si existe, lanza excepción
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Recupera la lista de todos los usuarios registrados.
        public List<User> RetrieveAllUsers()
        {
            var userCrud = new UserCrudFactory();
            return userCrud.RetrieveAll<User>();
        }

        // Recupera un usuario por su identificador único.
        public User RetrieveUserById(int id)
        {
            var userCrud = new UserCrudFactory();
            return userCrud.RetrieveById<User>(id);
        }

        // Recupera un usuario por su correo electrónico.
        public User RetrieveUserByEmail(User user)
        {
            var userCrud = new UserCrudFactory();
            return userCrud.RetrieveByEmail<User>(user); // Recupera el usuario por correo electrónico
        }

        // Actualiza la información de un usuario existente.
        public void UpdateUser(User user)
        {
            try
            {
                var userCrud = new UserCrudFactory();
                userCrud.Update(user); // Actualiza el usuario en la base de datos
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Elimina un usuario del sistema.
        public void DeleteUser(User user)
        {
            try
            {
                var userCrud = new UserCrudFactory();
                userCrud.Delete(user); // Elimina el usuario de la base de datos
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
    }
}
