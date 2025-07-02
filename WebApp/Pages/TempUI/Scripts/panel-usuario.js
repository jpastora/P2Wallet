document.addEventListener('DOMContentLoaded', () => {
    // Agregar cuenta
    const agregarCuenta = document.getElementById('agregarCuenta');
    if (agregarCuenta) {
        agregarCuenta.addEventListener('click', (e) => {
            e.preventDefault();
            window.location.href = 'agregar-cuenta.html';
        });
    }

    // Ver historial
    const verHistorial = document.getElementById('verHistorial');
    if (verHistorial) {
        verHistorial.addEventListener('click', (e) => {
            e.preventDefault();
            window.location.href = 'historial-transacciones.html';
        });
    }

    // Escanear QR
    const escanearQR = document.getElementById('escanearQR');
    if (escanearQR) {
        escanearQR.addEventListener('click', () => {
            alert('Funcionalidad de cámara no implementada en este prototipo.');
        });
    }

    // Navegación inferior (simulada)
    document.querySelectorAll('.nav-bottom a').forEach((link, index) => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const rutas = ['panel-usuario.html', 'tarjetas.html', 'estadisticas.html', 'perfil.html'];
            window.location.href = rutas[index];
        });
    });
});
