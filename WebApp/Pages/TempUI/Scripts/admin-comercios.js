document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.ver').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Ver detalles del comercio ${i + 1}`));
    });

    document.querySelectorAll('.editar').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Editar comisión del comercio ${i + 1}`));
    });

    document.querySelectorAll('.bloquear').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Bloquear comercio ${i + 1}`));
    });
});
