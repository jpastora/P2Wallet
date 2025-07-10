using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class BankAccount : BaseDTO
    {
        public int UserID { get; set; }
        public string IBAN { get; set; }
        public int FinancialEntityID { get; set; }
        public bool Status { get; set; }
    }
}
