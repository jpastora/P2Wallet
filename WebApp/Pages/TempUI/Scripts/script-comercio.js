document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('registerCommerceForm');
    const logoInput = document.getElementById('logo');
    const logoPreview = document.getElementById('logoPreview');
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

    // Geolocalizar y marcar en el mapa
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
            marker = L.marker([latitude, longitude])
                .addTo(map)
                .bindPopup('Ubicación del Comercio')
                .openPopup();
        }, () => {
            showAlert('No se pudo obtener la ubicación.', 'danger');
        });
    });

    // Vista previa del logo
    logoInput.addEventListener('change', () => {
        const file = logoInput.files[0];
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = e => {
                logoPreview.src = e.target.result;
                logoPreview.classList.remove('d-none');
            };
            reader.readAsDataURL(file);
        } else {
            logoPreview.classList.add('d-none');
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

    // Validación y envío (simulado)
    form.addEventListener('submit', e => {
        e.preventDefault();
        e.stopPropagation();

        const valid = form.checkValidity();
        if (!valid) {
            form.classList.add('was-validated');
            return;
        }

        showAlert('Formulario válido y listo para enviar.', 'success');
        // Aquí iría el envío real al backend…
    });
});
