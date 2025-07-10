using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public class UserManager :BaseManager
    {
        public void Create(User user)
        {

            try
            {
                //Validar la edad
                if (IsOver18(user))
                {
                    var userCrud = new UserCrudFactory();

                    //Consultamos en la base de datos si existe un usuario con ese codigo
                    var userExist = userCrud.RetrieveById<User>(user);

                    if (uExist == null)
                    {

                        if (uExist == null)
                        {
                            //Consultamos si en la bd existe un usuario con ese mail
                            uExist = userCrud.RetrieveByUserEmail<User>(user);

                            if (uExist == null)
                                uCrud.Create(user);

                            // var emailService = new SendGridService();
                            // emailService.SendWelcomeEmail(user.Email, user.Name).Wait();

                        }
                        else
                        {
                            throw new Exception("Este correo ya se encuentra registrado.");
                        }
                    }
                    else
                    {
                        throw new Exception("Codigo de usuario no disponible.");
                    }

                    uCrud.Create(user);
                }
                else
                {
                    throw new Exception("Usuario no cumple con la edad");
                }

            }
            catch (Exception ex)
            {
                {
                    ManagerException(ex);
                }

            }
        }

        public void UpdateUser(User user)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }

        public void DeleteUser(User user)
        {
            try
            {
                var userCrud = new UserCrudFactory();
            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }


    }
}
