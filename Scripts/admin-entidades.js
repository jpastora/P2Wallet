document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.ver').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Ver detalles de entidad ${i + 1}`));
    });

    document.querySelectorAll('.editar').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Editar comisión de entidad ${i + 1}`));
    });

    document.querySelectorAll('.eliminar').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Desafiliar entidad ${i + 1}`));
    });
});
