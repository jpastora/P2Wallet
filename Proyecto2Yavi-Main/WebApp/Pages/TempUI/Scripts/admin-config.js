document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', e => {
            e.preventDefault();
            alert('Cambios guardados (simulado).');
        });
    });

    document.getElementById('btn2fa')?.addEventListener('click', () => {
        alert('Autenticación de dos factores activada (simulado).');
    });

    document.getElementById('btnCerrarSesiones')?.addEventListener('click', () => {
        alert('Sesiones cerradas exitosamente (simulado).');
    });
});
