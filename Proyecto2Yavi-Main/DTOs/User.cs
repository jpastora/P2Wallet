using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class User : BaseDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string IDPhotoFrontUrl { get; set; }
        public string IDPhotoBackUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Password { get; set; }
        public bool EmailVerified { get; set; }
        public bool MobileVerified { get; set; }
        public bool BiometricVerified { get; set; }
        public bool IsActive { get; set; }
    }
}
