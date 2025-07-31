using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class AccountMovement : BaseDTO
    {
        public int TransactionID { get; set; }
        public string SourceAccountDescription { get; set; }
        public string DestinationAccountDescription { get; set; }
        public string MovementType { get; set; }
        public double Amount { get; set; }

    }
}
