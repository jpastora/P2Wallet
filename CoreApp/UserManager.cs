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
                {
                    // Hash de la contraseña antes de guardar
                    user.Password = PasswordHelper.HashPassword(user.Password);
                    userCrud.Create(user); // Si no existe, crea el usuario
                }
                else
                    throw new Exception("El usuario con este correo ya existe."); // Si existe, lanza excepción
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
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
                
                // Validar y completar campos requeridos
                ValidateAndCompleteUserFields(user);
                
                // CRÍTICO: Solo hashear el password si NO está ya hasheado
                // Un password hasheado típicamente tiene más de 40 caracteres y contiene caracteres base64
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    // Verificar si el password ya está hasheado (evitar doble hashing)
                    if (user.Password.Length < 40 || !IsBase64String(user.Password))
                    {
                        user.Password = PasswordHelper.HashPassword(user.Password);
                    }
                    // Si ya está hasheado (>40 chars y base64), lo dejamos como está
                }
                
                userCrud.Update(user); // Actualiza el usuario en la base de datos
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        // Método helper para validar y completar campos requeridos por el procedimiento almacenado
        private void ValidateAndCompleteUserFields(User user)
        {
            // Asegurar que todos los campos requeridos por el SP tengan valores por defecto
            if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
                user.ProfilePhotoUrl = "";
            if (string.IsNullOrEmpty(user.IDPhotoFrontUrl))
                user.IDPhotoFrontUrl = "";
            if (string.IsNullOrEmpty(user.IDPhotoBackUrl))
                user.IDPhotoBackUrl = "";
            if (string.IsNullOrEmpty(user.EmailVerified))
                user.EmailVerified = "Active";
            if (string.IsNullOrEmpty(user.MobileVerified))
                user.MobileVerified = "Active";
            if (string.IsNullOrEmpty(user.BiometricVerified))
                user.BiometricVerified = "Inactive";
            if (string.IsNullOrEmpty(user.SMSNotification))
                user.SMSNotification = "Active";
            if (string.IsNullOrEmpty(user.EmailNotification))
                user.EmailNotification = "Active";
            if (string.IsNullOrEmpty(user.PushNotification))
                user.PushNotification = "Active";
            if (string.IsNullOrEmpty(user.ValidationStatus))
                user.ValidationStatus = "Inactive";
            if (string.IsNullOrEmpty(user.Role))
                user.Role = "User";
            if (string.IsNullOrEmpty(user.FirstName))
                user.FirstName = "";
            if (string.IsNullOrEmpty(user.LastName))
                user.LastName = "";
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentException("Email es requerido");
            if (string.IsNullOrEmpty(user.MobilePhone))
                user.MobilePhone = "";
            if (string.IsNullOrEmpty(user.IDNumber))
                user.IDNumber = "";
        }

        // Método helper para verificar si una cadena es Base64
        private bool IsBase64String(string s)
        {
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Actualiza la información biometrica de un usuario validado.
        public void UpdateBiometric(User user)
        {
            try
            {
                var userCrud = new UserCrudFactory();
                userCrud.UpdateBiometricStatus(user); // Actualiza la información biométrica del usuario
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
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
                ExceptionLogger.LogException(ex);
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }

        public List<Merchant> GetMerchantsForUser(int userId)
        {
            var userMerchantManager = new UserMerchantManager();
            return userMerchantManager.GetMerchantsForUser(userId);
        }
        public List<FinancialEntity> GetEntitiesForUser(int userId)
        {
            var userEntityManager = new UserEntityManager();
            return userEntityManager.GetFinancialEntitiesForUser(userId);
        }
        public void ChangeUserPassword(int userId, string newPassword)
        {
            try
            {
                var userCrud = new UserCrudFactory();
                userCrud.ChangePassword(userId, PasswordHelper.HashPassword(newPassword));
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                ManageException(ex);
            }
        }
    }
}
