document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('loginForm');
    const facialBtn = document.getElementById('facialLoginBtn');

    form.addEventListener('submit', (e) => {
        e.preventDefault();
        e.stopPropagation();

        if (form.checkValidity()) {
            window.location.href = '/dashboard.html';
        } else {
            form.classList.add('was-validated');
        }
    });

    facialBtn.addEventListener('click', () => {
        window.location.href = './verificacion-facial.html';
    });
});
