document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('formPerfil').addEventListener('submit', e => {
        e.preventDefault();
        alert('Datos de perfil actualizados.');
    });

    document.getElementById('btnCambiarClave').addEventListener('click', () => {
        alert('Redirigiendo a cambio de contraseña...');
    });

    document.getElementById('btnCerrarSesion').addEventListener('click', () => {
        alert('Cerrando sesión...');
    });

    document.querySelectorAll('.ir-panel').forEach(btn => {
        btn.addEventListener('click', () => {
            const nombre = btn.getAttribute('data-nombre');
            alert(`Redirigiendo al panel de administración de ${nombre}...`);
        });
    });
});
