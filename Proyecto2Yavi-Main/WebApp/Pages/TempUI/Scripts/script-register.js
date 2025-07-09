document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('registerForm');
    const photoInput = document.getElementById('photo');
    const photoPreview = document.getElementById('photoPreview');
    const alertPlaceholder = document.getElementById('alertPlaceholder');
    const btnLocate = document.getElementById('btnLocate');
    const latInput = document.getElementById('latitude');
    const lngInput = document.getElementById('longitude');

    // Inicializar mapa Leaflet
    const map = L.map('map').setView([0, 0], 2);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);
    let marker;

    // Obtener y mostrar ubicación del usuario
    btnLocate.addEventListener('click', () => {
        if (!navigator.geolocation) {
            showAlert('Tu navegador no soporta geolocalización.', 'warning');
            return;
        }
        navigator.geolocation.getCurrentPosition(pos => {
            const { latitude, longitude } = pos.coords;
            latInput.value = latitude;
            lngInput.value = longitude;
            map.setView([latitude, longitude], 13);
            if (marker) map.removeLayer(marker);
            marker = L.marker([latitude, longitude]).addTo(map)
                .bindPopup('Tu ubicación').openPopup();
        }, () => {
            showAlert('No se pudo obtener tu ubicación.', 'danger');
        });
    });

    // Vista previa de la imagen
    photoInput.addEventListener('change', () => {
        const file = photoInput.files[0];
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = e => {
                photoPreview.src = e.target.result;
                photoPreview.classList.remove('d-none');
            };
            reader.readAsDataURL(file);
        } else {
            photoPreview.classList.add('d-none');
        }
    });

    // Mostrar alertas
    function showAlert(message, type = 'success') {
        alertPlaceholder.innerHTML = `
      <div class="alert alert-${type} alert-dismissible" role="alert">
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      </div>`;
    }

    // Envío del formulario
    form.addEventListener('submit', e => {
        e.preventDefault();
        e.stopPropagation();

        // Validación nativa + contraseña
        let valid = form.checkValidity();
        const pwd = document.getElementById('password');
        const pwd2 = document.getElementById('confirmPassword');
        if (pwd.value !== pwd2.value) {
            valid = false;
            pwd2.classList.add('is-invalid');
        } else {
            pwd2.classList.remove('is-invalid');
        }

        if (!valid) {
            form.classList.add('was-validated');
            return;
        }

        // Aquí podrías enviar al backend...
        showAlert('Formulario válido y listo para enviar.', 'success');
    });
});