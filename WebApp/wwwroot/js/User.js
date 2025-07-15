function UserViewController()
{

    this.ViewName = "User";
    this.ApiEndpoint = "User";

    //Metodo Constructor
    this.InitView = function ()
    {

        console.log("UserViewController.InitView --> Ok");
        this.loadTable();
    }

    // Metodo crear usuario
    this.Create = function ()
    {
        var userDTO = {};

        //Atributos por defecto
        userDTO.Id = 0;
        userDTO.CreatedAt = "2000-01-01";
        userDTO.UpdatedAt = "2000-01-01";
        userDTO.IDNumber = "";
        userDTO.ProfilePhotoUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.IDPhotoFrontUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.IDPhotoBackUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.EmailVerified = "Inactive";
        userDTO.MobileVerified = "Inactive";
        userDTO.BiometricVerified = "Inactive";
        userDTO.ValidationStatus = "Inactive";
        userDTO.SMSNotification = "Inactive";
        userDTO.EmailNotification = "Inactive";
        userDTO.PushNotification = "Inactive";
        userDTO.BirthDate = new Date("2000-01-01T00:00:00Z").toISOString();

        //Atributos del formulario
        userDTO.FullName = document.getElementById("FullName").value;
        userDTO.IDNumber = document.getElementById("IDNumber").value;
        userDTO.Email = document.getElementById("Email").value;
        userDTO.MobilePhone = document.getElementById("MobilePhone").value;
        userDTO.Latitude = document.getElementById("Latitude").value;
        userDTO.Longitude = document.getElementById("Longitude").value;
        userDTO.Password = document.getElementById("Password").value;

        //Enviar la data al API
        var ca = new ControlActions();
        var urlService = this.ApiEndpoint + "/Create";

        ca.PostToAPI(urlService, userDTO, function ()
        {
            console.log("User created successfully");

        });




        /*public string FullName { get; set; }
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
        public string EmailVerified { get; set; }
        public string MobileVerified { get; set; }
        public string BiometricVerified { get; set; }
        public string ValidationStatus { get; set; }
        public string SMSNotification { get; set; }
        public string EmailNotification { get; set; }
        public string PushNotification { get; set; }*/



    }



}

