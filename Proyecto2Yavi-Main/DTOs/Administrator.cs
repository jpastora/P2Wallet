using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class Administrator : BaseDTO
    {
        public int AdminID { get; set; }
        public string Name { get; set; }
        public string AcessUsername { get; set; }
        public string AccessPassword { get; set; }
        public string AdminStatus { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
