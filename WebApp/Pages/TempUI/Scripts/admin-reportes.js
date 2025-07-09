<script>
    document.addEventListener('DOMContentLoaded', function() {
        // Configurar fechas por defecto (mes actual)
        const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    document.getElementById('fechaInicio').valueAsDate = firstDay;
    document.getElementById('fechaFin').valueAsDate = lastDay;

    // Inicializar gráficos
    const barCtx = document.getElementById('barChart').getContext('2d');
    const pieCtx = document.getElementById('pieChart').getContext('2d');

    const barChart = new Chart(barCtx, {
        type: 'bar',
    data: {
        labels: ['Banco Inter', 'Financiera Costa', 'Banco Nacional'],
    datasets: [{
        label: 'Comisiones ($)',
    data: [1200, 850, 450],
    backgroundColor: [
    'rgba(54, 162, 235, 0.7)',
    'rgba(255, 99, 132, 0.7)',
    'rgba(75, 192, 192, 0.7)'
    ],
    borderColor: [
    'rgba(54, 162, 235, 1)',
    'rgba(255, 99, 132, 1)',
    'rgba(75, 192, 192, 1)'
    ],
    borderWidth: 1
                }]
            },
    options: {
        responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
        position: 'top',
                    },
    tooltip: {
        callbacks: {
        label: function(context) {
                                return `$${context.raw.toFixed(2)}`;
                            }
                        }
                    }
                },
    scales: {
        y: {
        beginAtZero: true,
    ticks: {
        callback: function(value) {
                                return `$${value}`;
                            }
                        }
                    }
                }
            }
        });

    const pieChart = new Chart(pieCtx, {
        type: 'pie',
    data: {
        labels: ['Transferencias', 'Pagos', 'Depósitos', 'Retiros'],
    datasets: [{
        data: [1200, 850, 500, 300],
    backgroundColor: [
    'rgba(54, 162, 235, 0.7)',
    'rgba(255, 99, 132, 0.7)',
    'rgba(75, 192, 192, 0.7)',
    'rgba(153, 102, 255, 0.7)'
    ],
    borderWidth: 1
                }]
            },
    options: {
        responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
        position: 'right',
                    },
    tooltip: {
        callbacks: {
        label: function(context) {
                                const label = context.label || '';
    const value = context.raw || 0;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
    const percentage = Math.round((value / total) * 100);
    return `${label}: $${value.toFixed(2)} (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });

    // Generar reporte
    document.getElementById('generarReporte').addEventListener('click', function() {
            const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;
    const entidad = document.getElementById('entidad').value;
    const tipoTransaccion = document.getElementById('tipoTransaccion').value;

    // Aquí iría la llamada AJAX para obtener los datos filtrados
    console.log('Generando reporte con:', {
        fechaInicio,
        fechaFin,
        entidad,
        tipoTransaccion
    });

    // Simular actualización de datos
    alert(`Reporte generado para el período ${fechaInicio} al ${fechaFin}`);

    // Actualizar los totales (en una implementación real esto vendría del servidor)
    document.querySelectorAll('.text-muted')[0].textContent =
    document.querySelectorAll('.text-muted')[1].textContent =
    document.querySelectorAll('.text-muted')[2].textContent =
    `Del ${formatDate(fechaInicio)} al ${formatDate(fechaFin)}`;
        });

    // Limpiar filtros
    document.getElementById('limpiarFiltros').addEventListener('click', function() {
        document.getElementById('fechaInicio').valueAsDate = firstDay;
    document.getElementById('fechaFin').valueAsDate = lastDay;
    document.getElementById('entidad').value = 'todas';
    document.getElementById('tipoTransaccion').value = 'todas';
        });

    // Exportar a PDF
    document.getElementById('exportarPDF').addEventListener('click', function() {
        // Usar jsPDF para generar PDF
        alert('Exportando a PDF...');
            // En una implementación real, se usaría jsPDF para generar el documento
        });

    // Exportar a Excel
    document.getElementById('exportarExcel').addEventListener('click', function() {
            // Usar SheetJS para generar Excel
            const tabla = 