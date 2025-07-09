document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('registroComercioForm');

    //Map elements
    const loadingElement = document.getElementById('loadingLocation');
    const errorElement = document.getElementById('locationError');
    const refreshBtn = document.getElementById('btnRefrescarUbicacion');
    const mapElement = document.getElementById('map');
    const direccionInput = document.getElementById('direccion');
    const latitudInput = document.getElementById('latitud');
    const longitudInput = document.getElementById('longitud');
    
    // Logo upload elements
    const logoInput = document.getElementById('logoComercio');
    const logoPreview = document.getElementById('logoPreview');
    const logoPreviewContainer = document.getElementById('logoPreviewContainer');
    const btnRemoveLogo = document.getElementById('btnRemoveLogo');
    
    let map = null;
    let marker = null;

    // Initialize map with coordinates
    function initMap(lat, lng) {
        // Hide loading, show map
        loadingElement.style.display = 'none';
        errorElement.style.display = 'none';
        mapElement.style.display = 'block';
        
        // Create new map instance if it doesn't exist
        if (!map) {
            map = L.map('map').setView([lat, lng], 16);
            
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
            }).addTo(map);
            
            // Create draggable marker
            marker = L.marker([lat, lng], {
                draggable: true
            }).addTo(map);
            
            // Update position on marker move
            marker.on('dragend', function() {
                updatePosition(marker.getLatLng());
            });
            
            // Update position on map click
            map.on('click', function(e) {
                marker.setLatLng(e.latlng);
                updatePosition(e.latlng);
            });
        } else {
            // Update existing map view
            map.setView([lat, lng], 16);
            marker.setLatLng([lat, lng]);
        }
        
        updatePosition({ lat, lng });
    }

    // Update form fields with position
    function updatePosition(position) {
        document.getElementById('latitud').value = position.lat;
        document.getElementById('longitud').value = position.lng;
        reverseGeocode(position.lat, position.lng);
    }

    // Convert coordinates to address
    function reverseGeocode(lat, lng) {
        fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`)
            .then(response => response.json())
            .then(data => {
                if (data.address) {
                    const addressParts = [
                        data.address.road,
                        data.address.house_number,
                        data.address.city,
                        data.address.country
                    ].filter(Boolean);
                    document.getElementById('direccion').value = addressParts.join(', ');
                }
            })
            .catch(console.error);
    }

    // Get user's current location
    function getUserLocation() {
        loadingElement.style.display = 'block';
        errorElement.style.display = 'none';
        mapElement.style.display = 'none';
        
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                position => {
                    initMap(position.coords.latitude, position.coords.longitude);
                },
                error => {
                    loadingElement.style.display = 'none';
                    errorElement.textContent = `Error: ${error.message}`;
                    errorElement.style.display = 'block';
                    // Fallback to San José, Costa Rica
                    initMap(9.9281, -84.0907);
                },
                { enableHighAccuracy: true, timeout: 10000 }
            );
        } else {
            loadingElement.style.display = 'none';
            errorElement.textContent = 'Geolocalización no soportada por tu navegador';
            errorElement.style.display = 'block';
            initMap(9.9281, -84.0907); // Fallback
        }
    }

    // Refresh location button
    refreshBtn.addEventListener('click', function() {
        const btn = this;
        btn.disabled = true;
        btn.innerHTML = '<i class="bi bi-arrow-clockwise"></i> Detectando...';
        getUserLocation();
        setTimeout(() => {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-geo-alt"></i> Usar mi ubicación';
        }, 2000);
    });

    // IBAN Validation
    document.getElementById('iban').addEventListener('input', function(e) {
        // Eliminar espacios y convertir a mayúsculas
        let iban = e.target.value.replace(/\s/g, '').toUpperCase();
        e.target.value = iban.match(/.{1,4}/g)?.join(' ') || iban;
        
        // Validación básica del formato
        const ibanRegex = /^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$/;
        const isValid = ibanRegex.test(iban.replace(/\s/g, ''));
        
        if (iban.length > 0 && !isValid) {
            e.target.classList.add('is-invalid');
        } else {
            e.target.classList.remove('is-invalid');
        }
    });

    // Logo Upload Handling
    logoInput.addEventListener('change', function(e) {
        const file = e.target.files[0];
        
        if (file) {
            // Validar tipo de archivo
            const validTypes = ['image/jpeg', 'image/png', 'image/gif'];
            if (!validTypes.includes(file.type)) {
                logoInput.classList.add('is-invalid');
                return;
            }
            
            // Validar tamaño (2MB máximo)
            if (file.size > 2 * 1024 * 1024) {
                logoInput.classList.add('is-invalid');
                alert('El archivo es demasiado grande. El tamaño máximo permitido es 2MB.');
                return;
            }
            
            // Mostrar vista previa
            const reader = new FileReader();
            reader.onload = function(event) {
                logoPreview.src = event.target.result;
                logoPreviewContainer.style.display = 'block';
                logoInput.classList.remove('is-invalid');
            };
            reader.readAsDataURL(file);
        }
    });
    
    // Remove Logo
    btnRemoveLogo.addEventListener('click', function() {
        logoInput.value = '';
        logoPreview.src = '#';
        logoPreviewContainer.style.display = 'none';
    });

    // Form submission
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        if (!form.checkValidity()) {
            e.stopPropagation();
            form.classList.add('was-validated');
            return;
        }
        
        // Validate coordinates
        if (!document.getElementById('latitud').value) {
            alert('Por favor seleccione una ubicación en el mapa');
            return;
        }
        
        // Validate IBAN
        const iban = document.getElementById('iban').value.replace(/\s/g, '');
        const ibanRegex = /^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$/;
        if (!ibanRegex.test(iban)) {
            e.preventDefault();
            e.stopPropagation();
            document.getElementById('iban').classList.add('is-invalid');
            return;
        }
        
        // Validate logo if uploaded
        if (logoInput.files.length > 0) {
            const file = logoInput.files[0];
            const validTypes = ['image/jpeg', 'image/png', 'image/gif'];
            
            if (!validTypes.includes(file.type) || file.size > 2 * 1024 * 1024) {
                e.preventDefault();
                e.stopPropagation();
                logoInput.classList.add('is-invalid');
                return;
            }
        }
        
        // Prepare form data (including file if uploaded)
        const formData = new FormData(form);
        
        // Aqui es donde podriamos mandar los datos al servidor
        console.log('Form data ready for submission:', {
            NombreComercio: formData.get('NombreComercio'),
            CedulaJuridica: formData.get('CedulaJuridica'),
            IBAN: formData.get('IBAN'),
            LogoComercio: logoInput.files.length > 0 ? logoInput.files[0].name : 'No logo',
            Direccion: formData.get('Direccion'),
            Latitud: formData.get('Latitud'),
            Longitud: formData.get('Longitud'),
            Telefono: formData.get('Telefono'),
            CorreoElectronico: formData.get('CorreoElectronico')
        });
        
        alert('Formulario enviado correctamente (simulado)');
        
        // For actual submission, you would use:
        // fetch('your-endpoint', {
        //     method: 'POST',
        //     body: formData
        // })
        // .then(response => response.json())
        // .then(data => {
        //     console.log('Success:', data);
        //     // Handle success
        // })
        // .catch(error => {
        //     console.error('Error:', error);
        //     // Handle error
        // });
    });

    // Initialize map on load
    getUserLocation();
});