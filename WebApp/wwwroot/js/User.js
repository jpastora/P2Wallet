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
        userDTO.ID = 0;
        userDTO.IDNumber = ""
        userDTO.Created = "2000-01-01";
        userDTO.Updated = "2000-01-01";
        userDTO.ProfilePhotoUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg",
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

        //Atributos del formulario (IDs corregidos)
        userDTO.FullName = document.getElementById("fullName").value;
        userDTO.Email = document.getElementById("email").value;
        userDTO.MobilePhone = document.getElementById("telefono").value;
        userDTO.Latitude = parseFloat(document.getElementById("txtLatitude").value);
        userDTO.Longitude = parseFloat(document.getElementById("txtLongitude").value);
        userDTO.Password = document.getElementById("password").value;

        //Enviar la data al API
        var ca = new ControlActions();
        var urlService = this.ApiEndpoint + "/Create";

        ca.PostToAPI(urlService, userDTO, function ()
        {
            console.log("User created successfully");
        });
    }
}



