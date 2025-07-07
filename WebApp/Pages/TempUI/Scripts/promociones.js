document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.ver-detalles').forEach((btn, i) => {
        btn.addEventListener('click', () => {
            alert(`Mostrando detalles de la promoción #${i + 1}`);
        });
    });
});
