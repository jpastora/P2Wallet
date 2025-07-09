document.addEventListener("DOMContentLoaded", () => {
    const tabla = document.getElementById("tabla-pendientes");

    tabla.addEventListener("click", (e) => {
        if (e.target.closest(".aprobar")) {
            const fila = e.target.closest("tr");
            const nombre = fila.children[1].textContent;
            alert(`✅ Entidad "${nombre}" aprobada.`);
            fila.remove();
        }

        if (e.target.closest(".rechazar")) {
            const fila = e.target.closest("tr");
            const nombre = fila.children[1].textContent;
            alert(`❌ Entidad "${nombre}" rechazada.`);
            fila.remove();
        }
    });
});
