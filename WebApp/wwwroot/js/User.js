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
        userDTO.IDNumber = "";
        userDTO.Created = "2000-01-01";
        userDTO.Updated = "2000-01-01";
        userDTO.ProfilePhotoUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.IDPhotoFrontUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.IDPhotoBackUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.EmailVerified = "Active";
        userDTO.MobileVerified = "Active";
        userDTO.BiometricVerified = "Inactive";
        userDTO.ValidationStatus = "Inactive";
        userDTO.SMSNotification = "Inactive";
        userDTO.EmailNotification = "Inactive";
        userDTO.PushNotification = "Inactive";
        userDTO.BirthDate = new Date("2000-01-01T00:00:00Z").toISOString();
        userDTO.Role = "User";

        //Atributos del formulario 
        userDTO.FirstName = document.getElementById("txtFirstName").value;
        userDTO.LastName = document.getElementById("txtLastName").value;
        userDTO.Email = document.getElementById("txtEmail").value;
        var phoneValue = document.getElementById("txtPhone").value.trim();
        // Agregar código de país automáticamente si no está presente
        if (!phoneValue.startsWith("+")) {
            phoneValue = "+506" + phoneValue;
        }
        // Validar que el teléfono incluya el código de país y tenga el formato correcto
        if (!/^\+\d{1,3}\d{8,}$/.test(phoneValue)) {
            alert("El número telefónico debe incluir el código de país, por ejemplo: +506XXXXXXXX");
            return;
        }
        userDTO.MobilePhone = phoneValue;
        userDTO.Latitude = parseFloat(document.getElementById("txtLatitude").value);
        userDTO.Longitude = parseFloat(document.getElementById("txtLongitude").value);
        userDTO.Password = document.getElementById("txtPassword").value;

        //Enviar la data al API
        var ca = new ControlActions();
        var urlService = this.ApiEndpoint + "/Create";

        ca.PostToAPI(urlService, userDTO, function ()
        {
            console.log("User created successfully");
        });
    }

    this.UpdateBiometricInfo = function (user)
    {
        var userDTO = {};

        //Atributos por defecto
        userDTO.ID = user.ID;
        userDTO.IDNumber = "";
        userDTO.Created = "2000-01-01";
        userDTO.Updated = "2000-01-01";
        userDTO.ProfilePhotoUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.IDPhotoFrontUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.IDPhotoBackUrl = "https://res.cloudinary.com/dxn6cnvz7/image/upload/v1752335713/cld-sample-5.jpg";
        userDTO.EmailVerified = "Active";
        userDTO.MobileVerified = "Active";
        userDTO.BiometricVerified = "Inactive";
        userDTO.ValidationStatus = "Inactive";
        userDTO.SMSNotification = "Inactive";
        userDTO.EmailNotification = "Inactive";
        userDTO.PushNotification = "Inactive";
        userDTO.BirthDate = new Date("2000-01-01T00:00:00Z").toISOString();
        userDTO.Role = "User";
        userDTO.FirstName = "";
        userDTO.LastName = "";
        userDTO.Email = "";
        var phoneValue = "";
        userDTO.MobilePhone = "";
        userDTO.Latitude = 0.00;
        userDTO.Longitude = 0.00
        userDTO.Password = "";


        var ca = new ControlActions();
        var urlService = this.ApiEndpoint + "/UpdateBiometricInfo";

        ca.PutToAPI(urlService, userDTO, function ()
        {
            console.log("Biometric information updated successfully for user ID: " + userDTO.ID);
        });

    }
}





