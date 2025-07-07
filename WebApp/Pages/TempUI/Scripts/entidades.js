document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.ver-mas').forEach((btn, i) => {
        btn.addEventListener('click', () => {
            alert(`Mostrando más información sobre Banco Ejemplo ${i + 1}`);
        });
    });
});
