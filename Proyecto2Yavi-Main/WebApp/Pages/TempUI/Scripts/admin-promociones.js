document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.editar').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Editar promoción ${i + 1}`));
    });

    document.querySelectorAll('.desactivar').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Desactivar promoción ${i + 1}`));
    });

    document.querySelectorAll('.estadisticas').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Ver estadísticas de promoción ${i + 1}`));
    });
});
