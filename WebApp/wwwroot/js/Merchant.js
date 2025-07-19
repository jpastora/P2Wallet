function MerchantViewController() {
    this.ViewName = "Merchant";
    this.ApiEndpoint = "Merchant";

    this.InitView = function () {
        console.log("MerchantViewController.InitView --> OK");

        var self = this;
        $('#btnCreate').click(function () {
            self.Create();
        });
    };

    this.Create = async function () {
        var merchantDTO = {
            ID: 0,
            CommissionPercentage: 0,
            ValidationStatus: "Inactive",
            Created: "2000-01-01",
            Updated: "2000-01-01"
        };

        merchantDTO.MerchantName = document.getElementById("nombreComercio").value.trim();
        merchantDTO.TaxID = document.getElementById("cedulaJuridica").value.trim();
        merchantDTO.Email = document.getElementById("correoElectronico").value.trim();

        let phoneValue = document.getElementById("txtPhone").value.trim();
        if (!phoneValue.startsWith("+")) {
            phoneValue = "+506" + phoneValue;
        }
        if (!/^\+\d{1,3}\d{8,}$/.test(phoneValue)) {
            alert("El número telefónico debe incluir el código de país, por ejemplo: +506XXXXXXXX");
            return;
        }

        merchantDTO.ContactPhone = phoneValue;
        merchantDTO.Latitude = parseFloat(document.getElementById("latitud").value);
        merchantDTO.Longitude = parseFloat(document.getElementById("longitud").value);

        // Subir imagen si existe
        const logoInput = document.getElementById("logoComercio");
        const logoFile = logoInput.files[0];
        if (logoFile) {
            try {
                const formData = new FormData();
                formData.append("photo", logoFile);

                const response = await fetch('https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net/api/cloudinary/Save', {
                    method: "POST",
                    body: formData
                });


                if (!response.ok) {
                    const errorMsg = await response.text();
                    throw new Error("Error al subir la imagen: " + errorMsg);
                }
                else {
                    const uploadResult = await response.json();
                    //console.log(uploadResult)
                    //console.log(uploadResult.secure_url);
                    merchantDTO.LogoImage = uploadResult.secureUrl;
                }

            } catch (error) {

                console.error(error);
                alert("Error al subir la imagen del logo. Intente nuevamente.");
                return;
            }
        } else {
            merchantDTO.LogoImage = null;
        }

        // Enviar la data al API para crear el merchant
        const ca = new ControlActions();
        const urlService = this.ApiEndpoint + "/Create";

        ca.PostToAPI(urlService, merchantDTO, function () {
            // Obtener el taxId para buscar el merchant recién creado
            const taxId = merchantDTO.TaxID;

            // Consultar merchant por TaxID para obtener el ID real
            ca.GetToApi(`Merchant/GetByTaxID?taxId=${encodeURIComponent(taxId)}`, function (merchant) {
                if (!merchant || !merchant.id) {
                    alert("No se pudo obtener el comercio recién creado.");
                    return;
                }

                var userMerchantDTO = {
                    Created: "2000-01-01",
                    MerchantID: merchant.id,
                    UserID: parseInt(document.getElementById("userId").value)
                };

                //console.log(userMerchantDTO.UserID);
                //console.log(userMerchantDTO.MerchantID);

                const urlService2 = "UserMerchant/Create";

                ca.PostToAPI(urlService2, userMerchantDTO, function () {
                    document.getElementById("nombreComercio").value = "";
                    document.getElementById("cedulaJuridica").value = "";
                    document.getElementById("correoElectronico").value = "";
                    document.getElementById("txtPhone").value = "";
                    document.getElementById("latitud").value = "";
                    document.getElementById("longitud").value = "";
                    document.getElementById("logoComercio").value = "";
                    document.getElementById("iban").value = "";

                    // Limpiar el input de archivo
                    const logoInput = document.getElementById("logoComercio");
                    logoInput.value = "";

                    // Eliminar vista previa si existe (imagen adjunta)
                    const previewContainer = document.getElementById("logoPreviewContainer");
                    if (previewContainer) {
                        previewContainer.innerHTML = ""; // Elimina el contenido (como una <img>)
                    }
                });
            });
        });

    };
}

// Ejecutar al cargar el DOM
$(document).ready(function () {
    const merchantVC = new MerchantViewController();
    merchantVC.InitView();
});
