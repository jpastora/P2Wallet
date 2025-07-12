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
        public string IDNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string IDPhotoFrontUrl { get; set; }
        public string IDPhotoBackUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Password { get; set; }
        public String EmailVerified { get; set; }
        public String MobileVerified { get; set; }
        public String BiometricVerified { get; set; }
        public String ValidationStatus { get; set; }
        public string SMSNotification { get; set; }
        public string EmailNotification { get; set; }
        public string PushNotification { get; set; }

    }
}
