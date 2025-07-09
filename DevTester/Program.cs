


using DataAccess.CRUD;
using DataAccess.DAO;
using DTOs;
using Newtonsoft.Json;
using System.Xml;
using System.Data.SqlTypes;

public class Program
{
    public static void Main(string[] args)
    {
        var user = new User()
        {
            FullName = "Test User",
            Email = "test",
            MobilePhone = "1234567890",
            IDPhotoFrontUrl = "https://example.com/front.jpg",
            IDPhotoBackUrl = "https://example.com/back.jpg",
            Latitude = 0.0,
            Longitude = 0.0,
            Password = "password123"
        };

        var userCrudFactory = new UserCrudFactory();
        userCrudFactory.Create(user);


    }
}