document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.ver-detalle').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Mostrando detalles de la transacción ${i + 1}`));
    });

    document.querySelectorAll('.reembolsar').forEach((btn, i) => {
        btn.addEventListener('click', () => alert(`Solicitando reembolso para la transacción ${i + 1}`));
    });

    document.getElementById('btnFiltrar').addEventListener('click', () => {
        alert('Aplicando filtros (simulado)');
    });

    document.getElementById('btnExportar').addEventListener('click', () => {
        alert('Exportando resultados a CSV (simulado)');
    });
});
