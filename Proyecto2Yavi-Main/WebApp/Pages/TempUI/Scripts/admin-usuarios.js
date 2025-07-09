document.addEventListener('DOMContentLoaded', () => {
    const editarBtns = document.querySelectorAll('.editar');
    const bloquearBtns = document.querySelectorAll('.bloquear');
    const eliminarBtns = document.querySelectorAll('.eliminar');

    editarBtns.forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Editar usuario ${i + 1}`));
    });

    bloquearBtns.forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Bloquear usuario ${i + 1}`));
    });

    eliminarBtns.forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Eliminar usuario ${i + 1}`));
    });
});
