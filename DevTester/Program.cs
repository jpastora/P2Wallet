


using DataAccess.CRUD;
using DataAccess.DAO;
using DTOs;
using Newtonsoft.Json;
using System.Xml;
using System.Data.SqlTypes;
using Formatting = Newtonsoft.Json.Formatting;

public class Program
{
    public static void Main(string[] args)
    {
        /* 
         * USER TEST 
        var user = new User()
        {
            ID = 6,
            FullName = "Test User2395 - Update2",
            Email = "tes345@gmail.com",
            MobilePhone = "8817890",
            ProfilePhotoUrl = "https://example.com/photo.jpg",
            IDPhotoFrontUrl = "https://example.com/front.jpg",
            IDPhotoBackUrl = "https://example.com/back.jpg",
            Latitude = 0.0,
            Longitude = 0.0,
            Password = "password123",
            UserStatus = true
        };

        var userCrudFactory = new UserCrudFactory();
        userCrudFactory.Update(user);

        var lstUsers = userCrudFactory.RetrieveAll<User>();
        foreach (var u in lstUsers)
        {
            Console.WriteLine(JsonConvert.SerializeObject(u, Formatting.Indented));
        }

        var userP = userCrudFactory.RetrieveById<User>(6);
        Console.WriteLine(JsonConvert.SerializeObject(userP, Formatting.Indented));
        */

        /*
         * MERCHANT TEST
        var merchant = new Merchant()
        {
            ID = 5,
            MerchantName = "Test Merchant 3 - Updated",
            TaxID = "4587456789",
            LogoImage = "https://example.com/logo.jpg",
            Latitude = 12.345678,
            Longitude = 98.765432,
            Phone = "23456789781",
            Email = "test4@email2.com",
            CommissionPercentage = 0.025,
            ValidationStatus = true

        };

        var merchantCrudFactory = new MerchantCrudFactory();
        merchantCrudFactory.Update(merchant);

        var lstMerchants = merchantCrudFactory.RetrieveAll<Merchant>();
        foreach (var m in lstMerchants)
        {
            Console.WriteLine(JsonConvert.SerializeObject(m, Formatting.Indented));
        }

        var merchantP = merchantCrudFactory.RetrieveById<Merchant>(5);
        Console.WriteLine(JsonConvert.SerializeObject(merchantP, Formatting.Indented));
        */

        /*
         * ADMINISTRATOR TEST
        var admin = new Administrator()
        {
            ID = 6,
            Name = "Admin Test 2",
            AcessUsername = "admin_test2",
            AccessPassword = "Ppassword123",
            AdminStatus = false
        };

        var adminCrudFactory = new AdministratorCrudFactory();
        adminCrudFactory.Delete(admin);

        var lstAdmins = adminCrudFactory.RetrieveAll<Administrator>();
        foreach (var a in lstAdmins)
        {
            Console.WriteLine(JsonConvert.SerializeObject(a, Formatting.Indented));
        }
        var adminP = adminCrudFactory.RetrieveById<Administrator>(1);
        Console.WriteLine(JsonConvert.SerializeObject(adminP, Formatting.Indented));
        */

        /*
         * FINANCIAL ENTITY TEST
         

        var financialEntity = new FinancialEntity()
        {
            ID = 2,
            EntityName = "Test Financial Entity 3 - Updated",
            TaxID = "3234567890",
            Latitude = 12.345678,
            Longitude = 98.765432,
            ContactPhone = "3465567890",
            Email = "email3@test.com",
            CommissionPercentage = 0.01,
            ValidationStatus = true
        };

        var financialEntityCrudFactory = new FinancialEntityCrudFactory();
        //financialEntityCrudFactory.Delete(financialEntity);

        var lstFinancialEntities = financialEntityCrudFactory.RetrieveAll<FinancialEntity>();
        foreach (var fe in lstFinancialEntities)
        {
            Console.WriteLine(JsonConvert.SerializeObject(fe, Formatting.Indented));
        }
        var financialEntityP = financialEntityCrudFactory.RetrieveById<FinancialEntity>(3);
        Console.WriteLine(JsonConvert.SerializeObject(financialEntityP, Formatting.Indented));
        */

        /*
         * BANK ACCOUNT TEST
          

        var bankAccount = new BankAccount()
        {
            ID = 3,
            UserID = 6,
            IBAN = "CR1234567890123456789987",
            FinancialEntityID = 1,
            Status = false
        };

        var bankAccountCrudFactory = new BankAccountCrudFactory();
        bankAccountCrudFactory.Delete(bankAccount);

        var lstBankAccounts = bankAccountCrudFactory.RetrieveAll<BankAccount>();
        foreach (var ba in lstBankAccounts)
        {
            Console.WriteLine(JsonConvert.SerializeObject(ba, Formatting.Indented));
        }
        var bankAccountP = bankAccountCrudFactory.RetrieveById<BankAccount>(4);
        Console.WriteLine(JsonConvert.SerializeObject(bankAccountP, Formatting.Indented));
        */

        /*
         * TRANSACTION TEST
        
        var transaction = new Transaction()
        {
            ID = 2,
            UserID = 6,
            MerchantID = 1,
            BankAccountID = 4,
            GrossAmount = 110.0,
            NetAmount = 107.5,
            DiscountApplied = 2.5,
            CommissionApplied = 0.0,
            TransactionStatus = "Pending Payment"
        };

        var transactionCrudFactory = new TransactionCrudFactory();
        //transactionCrudFactory.Create(transaction);

        var lstTransactions = transactionCrudFactory.RetrieveAll<Transaction>();
        foreach (var t in lstTransactions)
        {
            Console.WriteLine(JsonConvert.SerializeObject(t, Formatting.Indented));
        }
        var transactionP = transactionCrudFactory.RetrieveById<Transaction>(4);
        Console.WriteLine(JsonConvert.SerializeObject(transactionP, Formatting.Indented));
        */
    }
}