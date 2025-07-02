using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class Merchant : BaseDTO
    {
        public string MerchantName { get; set; }
        public string TaxID { get; set; }
        public string LogoImage { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public float CommissionPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}
