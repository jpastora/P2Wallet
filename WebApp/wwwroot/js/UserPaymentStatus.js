/**
 * UserPaymentStatus.js
 * Funcionalidades JavaScript para la página UserPaymentStatus
 * P2Wallet - Sistema de Pagos
 */

class UserPaymentStatusManager {
    constructor() {
        this.merchantName = '';
        this.initializeEventListeners();
    }

    /**
     * Inicializar event listeners cuando la página carga
     */
    initializeEventListeners() {
        document.addEventListener('DOMContentLoaded', () => {
            this.setupTableHover();
            this.setupAutoRefresh();
            this.handleInitialMessage();
        });
    }

    /**
     * Configurar datos desde Razor Page
     */
    configure(config) {
        this.merchantName = config.merchantName || '';
        this.activeCount = config.activeCount || 0;
        this.message = config.message || '';
    }

    /**
     * Mostrar QR en modal grande
     */
    showQR(code, qrUrl) {
        const modalImage = document.getElementById('modalQRImage');
        const modalCode = document.getElementById('modalQRCode');
        
        if (modalImage && modalCode) {
            modalImage.src = qrUrl.replace('150x150', '300x300');
            modalCode.textContent = code;
            
            const modal = new bootstrap.Modal(document.getElementById('qrModal'));
            modal.show();
        }
    }

    /**
     * Descargar QR modal
     */
    downloadModalQR() {
        const qrImage = document.getElementById('modalQRImage');
        const code = document.getElementById('modalQRCode');
        
        if (qrImage && code) {
            const link = document.createElement('a');
            link.download = `qr-pago-${code.textContent}.jpg`;
            link.href = qrImage.src;
            link.click();
        }
    }

    /**
     * Imprimir QR modal
     */
    printModalQR() {
        const qrImage = document.getElementById('modalQRImage');
        const code = document.getElementById('modalQRCode');
        
        if (qrImage && code) {
            const printWindow = window.open('', '_blank');
            printWindow.document.write(`
                <html>
                    <head><title>Código QR - ${code.textContent}</title></head>
                    <body style="text-align: center; font-family: Arial;">
                        <h2>${this.merchantName}</h2>
                        <h3>Código QR de Pago</h3>
                        <img src="${qrImage.src}" style="max-width: 300px;" />
                        <p><strong>Código:</strong> ${code.textContent}</p>
                    </body>
                </html>
            `);
            printWindow.document.close();
            printWindow.print();
        }
    }

    /**
     * Confirmar cancelación de pago con SweetAlert
     */
    confirmCancelPayment(paymentCode, description, amount) {
        Swal.fire({
            title: '¿Cancelar Solicitud?',
            html: `
                <div class="text-start">
                    <p><strong>Descripción:</strong> ${description}</p>
                    <p><strong>Monto:</strong> ?${amount}</p>
                    <p><strong>Código:</strong> ${paymentCode}</p>
                </div>
                <div class="alert alert-warning mt-3">
                    <i class="bi bi-exclamation-triangle"></i>
                    Esta acción no se puede deshacer
                </div>
            `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Sí, Cancelar',
            cancelButtonText: 'No, Mantener',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                this.cancelPaymentRequest(paymentCode);
            }
        });
    }

    /**
     * Cancelar pago vía formulario
     */
    cancelPaymentRequest(paymentCode) {
        // Mostrar loading
        Swal.fire({
            title: 'Cancelando solicitud...',
            text: 'Por favor espera',
            icon: 'info',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        
        // Enviar formulario
        const paymentCodeInput = document.getElementById('cancelPaymentCode');
        const cancelForm = document.getElementById('cancelPaymentForm');
        
        if (paymentCodeInput && cancelForm) {
            paymentCodeInput.value = paymentCode;
            cancelForm.submit();
        }
    }

    /**
     * Refrescar pagos con animación
     */
    refreshPayments() {
        Swal.fire({
            title: 'Actualizando...',
            text: 'Cargando datos más recientes',
            icon: 'info',
            timer: 1000,
            timerProgressBar: true,
            showConfirmButton: false,
            willClose: () => {
                window.location.reload();
            }
        });
    }

    /**
     * Configurar hover en filas de tabla
     */
    setupTableHover() {
        const tableRows = document.querySelectorAll('tbody tr');
        tableRows.forEach(row => {
            row.addEventListener('mouseenter', function() {
                this.style.backgroundColor = '#f8f9fa';
            });
            
            row.addEventListener('mouseleave', function() {
                this.style.backgroundColor = '';
            });
        });
    }

    /**
     * Configurar auto-refresh inteligente
     */
    setupAutoRefresh() {
        // Auto-refresh cada 30 segundos solo en pestaña activa y si hay solicitudes activas
        setInterval(() => {
            const activeTab = document.getElementById('active-tab');
            if (activeTab && activeTab.classList.contains('active')) {
                // Solo refrescar si hay solicitudes activas
                if (this.activeCount > 0) {
                    window.location.reload();
                }
            }
        }, 30000);
    }

    /**
     * Manejar mensaje inicial de TempData
     */
    handleInitialMessage() {
        if (this.message && this.message.trim() !== '') {
            // Determinar tipo de mensaje basado en contenido
            let messageType = 'info';
            if (this.message.includes('?')) {
                messageType = 'success';
            } else if (this.message.includes('?')) {
                messageType = 'error';
            }
            
            Swal.fire({
                icon: messageType,
                title: messageType === 'success' ? '¡Éxito!' : messageType === 'error' ? 'Error' : 'Información',
                text: this.message.replace(/?|?/g, '').trim(),
                confirmButtonText: 'Entendido',
                timer: 5000,
                timerProgressBar: true
            });
        }
    }

    /**
     * Mostrar mensaje de éxito
     */
    showSuccessMessage(message) {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: message,
            confirmButtonText: 'Entendido',
            timer: 3000,
            timerProgressBar: true
        });
    }

    /**
     * Mostrar mensaje de error
     */
    showErrorMessage(message) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: message,
            confirmButtonText: 'Entendido'
        });
    }
}

// Crear instancia global
window.UserPaymentStatusManager = new UserPaymentStatusManager();

// Funciones globales para compatibilidad con Razor
function showQR(code, qrUrl) {
    window.UserPaymentStatusManager.showQR(code, qrUrl);
}

function downloadModalQR() {
    window.UserPaymentStatusManager.downloadModalQR();
}

function printModalQR() {
    window.UserPaymentStatusManager.printModalQR();
}

function confirmCancelPayment(paymentCode, description, amount) {
    window.UserPaymentStatusManager.confirmCancelPayment(paymentCode, description, amount);
}

function refreshPayments() {
    window.UserPaymentStatusManager.refreshPayments();
}