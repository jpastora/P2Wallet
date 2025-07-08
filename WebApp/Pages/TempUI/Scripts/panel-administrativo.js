document.addEventListener('DOMContentLoaded', () => {
    // Inicializar gráficos
    const transaccionesChart = new Chart(document.getElementById('transaccionesChart'), {
        type: 'line',
        data: {
            labels: Array.from({ length: 30 }, (_, i) => `Día ${i + 1}`),
            datasets: [{
                label: 'Transacciones',
                data: Array.from({ length: 30 }, () => Math.floor(Math.random() * 50) + 10),
                fill: false,
                borderColor: '#553DF2',
                tension: 0.3
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } }
        }
    });

    const ingresosChart = new Chart(document.getElementById('ingresosChart'), {
        type: 'doughnut',
        data: {
            labels: ['Comercios', 'Entidades'],
            datasets: [{
                data: [65, 35],
                backgroundColor: ['#553DF2', '#39BF68']
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });

    // Acciones de botones
    document.querySelectorAll('.aprobar').forEach(btn => {
        btn.addEventListener('click', () => alert('Solicitud aprobada.'));
    });

    document.querySelectorAll('.rechazar').forEach(btn => {
        btn.addEventListener('click', () => alert('Solicitud rechazada.'));
    });
});
