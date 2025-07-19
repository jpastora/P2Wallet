function FinancialEntityViewController() {
    this.ViewName = "FinancialEntity";
    this.ApiEndpoint = "FinancialEntity";

    this.InitView = function () {
        console.log("FinancialEntityViewController.InitView --> OK");

        var self = this;
        $('#btnRegisterEntity').click(function () {
            self.Create();
        });
    };

    this.Create = async function () {
        var entityDTO = {
            ID: 0,
            ValidationStatus: "Inactive",
            CommissionPercentage: 0,
            Created: "2000-01-01",
            Updated: "2000-01-01"
        };

        entityDTO.EntityName = document.getElementById("nombreEntidad").value.trim();
        entityDTO.TaxID = document.getElementById("taxid").value.trim();
        entityDTO.Email = document.getElementById("correo").value.trim();

        let phoneValue = document.getElementById("telefono").value.trim();
        if (!phoneValue.startsWith("+")) {
            phoneValue = "+506" + phoneValue;
        }
        if (!/^\+\d{1,3}\d{8,}$/.test(phoneValue)) {
            alert("El número telefónico debe incluir el código de país, por ejemplo: +506XXXXXXXX");
            return;
        }

        entityDTO.ContactPhone = phoneValue;
        entityDTO.Latitude = parseFloat(document.getElementById("latitud").value);
        entityDTO.Longitude = parseFloat(document.getElementById("longitud").value);

        // Subir imagen si existe
        const logoInput = document.getElementById("logoEntidad");
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
                } else {
                    const uploadResult = await response.json();
                    entityDTO.LogoImage = uploadResult.secureUrl;
                }

            } catch (error) {
                console.error(error);
                alert("Error al subir la imagen del logo. Intente nuevamente.");
                return;
            }
        } else {
            entityDTO.LogoImage = null;
        }

        // Enviar al API
        const ca = new ControlActions();
        const urlService = this.ApiEndpoint + "/Create";

        ca.PostToAPI(urlService, entityDTO, function () {
            const taxId = entityDTO.TaxID;

            // Buscar la entidad creada por TaxID para obtener el ID
            ca.GetToApi(`FinancialEntity/GetByTaxID?taxId=${encodeURIComponent(taxId)}`, function (entity) {
                if (!entity || !entity.id) {
                    alert("No se pudo obtener la entidad financiera recién creada.");
                    return;
                }

                var userEntityDTO = {
                    Created: "2000-01-01",
                    FinancialEntityID: entity.id,
                    UserID: parseInt(document.getElementById("userId").value)
                };

                const urlService2 = "UserFinancialEntity/Create";

                ca.PostToAPI(urlService2, userEntityDTO, function () {
                    document.getElementById("nombreEntidad").value = "";
                    document.getElementById("taxid").value = "";
                    document.getElementById("correo").value = "";
                    document.getElementById("telefono").value = "";
                    document.getElementById("latitud").value = "";
                    document.getElementById("longitud").value = "";
                    document.getElementById("logoEntidad").value = "";
                    document.getElementById("commission").value = "";

                    // Limpiar el input de archivo
                    const logoInput = document.getElementById("logoEntidad");
                    logoInput.value = "";

                    // Limpiar vista previa
                    const previewContainer = document.getElementById("logoPreviewContainer");
                    if (previewContainer) {
                        previewContainer.innerHTML = "";
                    }


                });
            });
        });
    };
}

// Ejecutar al cargar el DOM
$(document).ready(function () {
    const financialEntityVC = new FinancialEntityViewController();
    financialEntityVC.InitView();
});
