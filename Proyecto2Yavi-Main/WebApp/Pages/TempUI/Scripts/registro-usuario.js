document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const photoPreview = document.getElementById('photoPreview');
    const startCamera = document.getElementById('startCamera');
    const takePhoto = document.getElementById('takePhoto');
    const retakePhoto = document.getElementById('retakePhoto');
    const fotoPerfilInput = document.getElementById('fotoPerfil');

    // Map elements
    const mapContainer = document.getElementById('mapContainer');
    const loadingElement = document.getElementById('loadingLocation');
    const errorElement = document.getElementById('locationError');
    const refreshBtn = document.getElementById('btnRefrescarUbicacion');
    const mapElement = document.getElementById('map');
    const direccionInput = document.getElementById('direccion');
    const latitudInput = document.getElementById('latitud');
    const longitudInput = document.getElementById('longitud');

    // ID elements
    const idDocumentInput = document.getElementById('idDocument');
    const idPreview = document.getElementById('idPreview');
    const idPreviewContainer = document.getElementById('idPreviewContainer');
    const btnRemoveId = document.getElementById('btnRemoveId');

    let stream = null;
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
            marker.on('dragend', function () {
                updatePosition(marker.getLatLng());
            });

            // Update position on map click
            map.on('click', function (e) {
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
        latitudInput.value = position.lat;
        longitudInput.value = position.lng;
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
                        data.address.neighbourhood,
                        data.address.city,
                        data.address.country
                    ].filter(Boolean);
                    direccionInput.value = addressParts.join(', ');
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

    // ID Document Preview
    idDocumentInput.addEventListener('change', function(e) {
        if (this.files && this.files[0]) {
            const file = this.files[0];
            const reader = new FileReader();
            
            reader.onload = function(e) {
                idPreview.src = e.target.result;
                idPreviewContainer.style.display = 'block';
            }
            
            if (file.type.match('image.*')) {
                reader.readAsDataURL(file);
            } else {
                // Handle PDF or other document types
                idPreview.src = './images/document-icon.png';
                idPreviewContainer.style.display = 'block';
            }
        }
    });

    // Remove ID Document
    btnRemoveId.addEventListener('click', function() {
        idDocumentInput.value = '';
        idPreview.src = '#';
        idPreviewContainer.style.display = 'none';
    });

    // Refresh location button
    refreshBtn.addEventListener('click', function() {
        const btn = this;
        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Detectando...';
        getUserLocation();
        setTimeout(() => {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-geo-alt-fill"></i> Actualizar ubicación';
        }, 2000);
    });

    // Camera functionality
    startCamera.addEventListener('click', async function() {
        try {
            stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    width: { ideal: 1280 },
                    height: { ideal: 720 },
                    facingMode: 'user'
                },
                audio: false
            });
            video.srcObject = stream;
            takePhoto.disabled = false;
            startCamera.disabled = true;
        } catch (err) {
            console.error("Error al acceder a la cámara: ", err);
            alert("No se pudo acceder a la cámara. Asegúrate de permitir el acceso.");
        }
    });

    // Take photo
    takePhoto.addEventListener('click', function() {
        const context = canvas.getContext('2d');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageDataUrl = canvas.toDataURL('image/png');
        photoPreview.src = imageDataUrl;
        fotoPerfilInput.value = imageDataUrl;

        photoPreview.classList.remove('d-none');
        video.classList.add('d-none');
        takePhoto.classList.add('d-none');
        retakePhoto.classList.remove('d-none');

        if (stream) {
            stream.getTracks().forEach(track => track.stop());
        }
    });

    // Retake photo
    retakePhoto.addEventListener('click', function() {
        photoPreview.classList.add('d-none');
        video.classList.remove('d-none');
        retakePhoto.classList.add('d-none');
        takePhoto.classList.remove('d-none');
        startCamera.disabled = false;
        fotoPerfilInput.value = '';
    });

    // Form validation
    function validateForm() {
        let isValid = true;
        
        // Validate name
        const nombre = document.getElementById('nombreCompleto').value.trim();
        if (nombre.length < 3) {
            isValid = false;
            document.getElementById('nombreCompleto').classList.add('is-invalid');
        }
        
        // Validate email
        const email = document.getElementById('correoElectronico').value;
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            isValid = false;
            document.getElementById('correoElectronico').classList.add('is-invalid');
        }
        
        // Validate phone
        const phone = document.getElementById('telefonoCelular').value;
        if (!/^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/.test(phone)) {
            isValid = false;
            document.getElementById('telefonoCelular').classList.add('is-invalid');
        }
        
        // Validate photo
        if (!fotoPerfilInput.value) {
            isValid = false;
            alert('Por favor tome una foto de perfil');
        }
        
        // Validate ID document
        if (!idDocumentInput.files || idDocumentInput.files.length === 0) {
            isValid = false;
            idDocumentInput.classList.add('is-invalid');
        }
        
        // Validate location
        if (!latitudInput.value || !longitudInput.value) {
            isValid = false;
            alert('Por favor seleccione una ubicación en el mapa');
        }
        
        return isValid;
    }

    // Form submission
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        form.classList.add('was-validated');

        if (!validateForm()) {
            return;
        }

        // Show loading state
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalBtnText = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Registrando...';

        try {
            // Prepare form data
            const formData = new FormData(form);
            
           // Aqui es donde podriamos mandar los datos al servidor
            console.log('Form data:', {
                name: formData.get('NombreCompleto'),
                email: formData.get('CorreoElectronico'),
                phone: formData.get('TelefonoCelular'),
                address: formData.get('Direccion'),
                coordinates: {
                    lat: formData.get('Latitud'),
                    lng: formData.get('Longitud')
                },
                hasPhoto: !!fotoPerfilInput.value,
                hasIdDocument: !!idDocumentInput.files[0]
            });

            // Simulate API call
            await new Promise(resolve => setTimeout(resolve, 2000));
            
            // Show success message
            alert('Registro exitoso! Será redirigido al inicio de sesión.');
            // In a real app: window.location.href = './Login/user.html';
            
        } catch (error) {
            console.error('Error:', error);
            alert('Error en el registro. Por favor intente nuevamente.');
        } finally {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalBtnText;
        }
    });

    // Initialize map on load
    getUserLocation();

    // Clean up camera stream when leaving page
    window.addEventListener('beforeunload', function() {
        if (stream) {
            stream.getTracks().forEach(track => track.stop());
        }
    });
});