


using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;

public class Program
{
    public static void Main(string[] args)
    {
        var sqlOperation = new SqlOperation();
        /*sqlOperation.ProcedureName = "CRE_Usuario_PR";

        sqlOperation.AddStringParameter("@P_NombreCompleto", "Juan Diego Gonzalez");
        sqlOperation.AddStringParameter("@P_CorreoElectronico", "juangv199523@gmail.com");
        sqlOperation.AddStringParameter("@P_TelefonoCelular", "86870299");
        sqlOperation.AddStringParameter("@P_FotoPerfil", "1URLTEST123456");
        sqlOperation.AddStringParameter("@P_FotoCedulaFrente", "1URLTEST123456");
        sqlOperation.AddStringParameter("@P_FotoCedulaAtras", "1URLTEST123456");
        sqlOperation.AddDoubleParam("@P_Latitud", 9.9333);
        sqlOperation.AddDoubleParam("@P_Longitud", -84.0833);
        sqlOperation.AddStringParameter("@P_Password", "Cenfotec123!");*/

        var sqlDAO = SqlDAO.GetInstance();
        //sqlDAO.ExecuteProcedure(sqlOperation);

        /*sqlOperation.ProcedureName = "DEL_USUARIO_PR";

        sqlOperation.AddIntParam("@P_ID", 1);
        sqlDAO.ExecuteProcedure(sqlOperation);*/
        sqlOperation.ProcedureName = "RET_ALL_USUARIOS_PR";
        sqlDAO.ExecuteProcedure(sqlOperation);
    }
}