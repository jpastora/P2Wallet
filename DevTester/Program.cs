


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

        var transaction = new Transaction()
        {
            ID = 3,
            UserID = 2,
            MerchantID = 1,
            BankAccountID = 1,
            GrossAmount = 30.00,
            NetAmount = 30.00,
            DiscountApplied = 0,
            CommissionApplied = 0,
            Timestamp = DateTime.Now,
            TransactionStatus = "Refunded"
        };


        var tCrudFactory = new TransactionCrudFactory();
        //tCrudFactory.Create(transaction);

        tCrudFactory.Update(transaction);

    }
}