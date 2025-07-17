using System;

namespace DTOs
{
    public class UserEntity : BaseDTO
    {
        public int UserID { get; set; }
        public int FinancialEntityID { get; set; }
    }
}
