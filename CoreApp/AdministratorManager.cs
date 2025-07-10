using DataAccess.CRUD;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{    // Administra las operaciones relacionadas con administradores.
    public class AdministratorManager : BaseManager
    {
        // Crea un nuevo administrador si el identificador no existe previamente.
        public void CreateAdministrator(Administrator administrator)
        {
            try
            {
                var administratorCrud = new AdministratorCrudFactory();

                if (administratorCrud.RetrieveById<Administrator>(administrator.ID) == null)
                {
                    administratorCrud.Create(administrator);
                }
                else
                {
                    throw new Exception("El administrador ya existe.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al crear el administrador: " + ex.Message);
            }
        }

        // Recupera la lista de todos los administradores registrados.
        public List<Administrator> RetrieveAllAdministrators()
        {
            var administratorCrud = new AdministratorCrudFactory();
            return administratorCrud.RetrieveAll<Administrator>();
        }

        // Recupera un administrador por su identificador único.
        public Administrator RetrieveAdministratorById(int id)
        {
            var administratorCrud = new AdministratorCrudFactory();
            return administratorCrud.RetrieveById<Administrator>(id);
        }

        // Actualiza la información de un administrador existente.
        public void UpdateAdministrator(Administrator administrator)
        {
            try
            {
                var administratorCrud = new AdministratorCrudFactory();
                administratorCrud.Update(administrator);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }


        }

        // Elimina un administrador.
        public void DeleteAdministrator(Administrator administrator)
        {
            try
            {
                var administratorCrud = new AdministratorCrudFactory();
                administratorCrud.Delete(administrator);
            }
            catch (Exception ex)
            {
                ManageException(ex); // Maneja la excepción usando el método base
            }
        }
    }
}
