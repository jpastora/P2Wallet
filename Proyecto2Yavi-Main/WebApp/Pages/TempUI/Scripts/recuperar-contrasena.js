document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('recoverForm');

    form.addEventListener('submit', (e) => {
        e.preventDefault();
        e.stopPropagation();

        if (form.checkValidity()) {
            window.location.href = 'confirmacion-envio.html';
        } else {
            form.classList.add('was-validated');
        }
    });
});
