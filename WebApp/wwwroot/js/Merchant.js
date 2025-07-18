function MerchantViewController() {
    this.ViewName = "Merchant";
    this.ApiEndpoint = "Merchant";

    // Metodo Constructor
    this.InitView = function () {
        console.log("MerchantViewController.InitView --> OK");

    };

    // Metodo crear un comercio
    this.Create = function () {
        var merchantDTO = {};

        // Atributos por defecto
        merchantDTO.ID = 0;
        merchantDTO.CommissionPercentage = 0;
        merchantDTO.ValidationStatus = "Inactive";
        userDTO.IDNumber = "";
        userDTO.Created = "2000-01-01";
        userDTO.Updated = "2000-01-01";

        // Atributos del formulario
        merchantDTO.MerchantName = document.getElementById("nombreComercio").value.trim();
        merchantDTO.TaxID = document.getElementById("cedulaJuridica").value.trim();
        merchantDTO.Email = document.getElementById("correoElectronico").value.trim();

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
        merchantDTO.ContactPhone = phoneValue;
        merchantDTO.Latitude = parseFloat(document.getElementById("txtLatitude").value);
        merchantDTO.Longitude = parseFloat(document.getElementById("txtLongitude").value);

        // ============ SUBIR IMAGEN A CLOUDINARY =============
        var logoInput = document.getElementById("logoComercio");
        var logoFile = logoInput.files[0];

        if (logoFile) {
            try {
                const formData = new FormData();
                formData.append("photo", logoFile);

                const response = await fetch("/api/cloudinary/Save", {
                    method: "POST",
                    body: formData
                });

                if (!response.ok) {
                    const errorMsg = await response.text();
                    throw new Error("Error al subir la imagen: " + errorMsg);
                }

                const uploadResult = await response.json();
                merchantDTO.LogoURL = uploadResult.secure_url; // o public_id si lo necesitas guardar también

            } catch (error) {
                console.error(error);
                alert("Error al subir la imagen del logo. Intente nuevamente.");
                return;
            }
        } else {
            merchantDTO.LogoURL = null; // o algún valor por defecto
        }

        //Enviar la data al API
        var ca = new ControlActions();
        var urlService = this.ApiEndpoint + "/Create";

        ca.PostToAPI(urlService, merchantDTO, function () {
            console.log("Merchant created successfully");
        });

    };
}
