/**
 * UserPaymentStatus.js
 * Funcionalidades JavaScript para la página UserPaymentStatus
 * P2Wallet - Sistema de Pagos
 */

class UserPaymentStatusManager {
    constructor() {
        this.merchantName = '';
        this.activeCount = 0;
        this.message = '';
        this.initialized = false;
        this.initializeEventListeners();
    }

    /**
     * Inicializar event listeners cuando la página carga
     */
    initializeEventListeners() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                this.onDOMReady();
            });
        } else {
            this.onDOMReady();
        }
    }

    /**
     * Ejecutar cuando el DOM está listo
     */
    onDOMReady() {
        if (!this.initialized) {
            this.setupTableHover();
            this.setupAutoRefresh();
            this.handleInitialMessage();
            this.initialized = true;
        }
    }

    /**
     * Configurar datos desde Razor Page
     */
    configure(config) {
        if (config) {
            this.merchantName = config.merchantName || '';
            this.activeCount = config.activeCount || 0;
            this.message = config.message || '';
        }
    }

    /**
     * Mostrar QR en modal grande
     */
    showQR(code, qrUrl) {
        try {
            const modalImage = document.getElementById('modalQRImage');
            const modalCode = document.getElementById('modalQRCode');
            const modalElement = document.getElementById('qrModal');
            
            if (modalImage && modalCode && modalElement) {
                modalImage.src = qrUrl.replace(/150x150/g, '300x300');
                modalCode.textContent = code;
                
                if (typeof bootstrap !== 'undefined') {
                    const modal = new bootstrap.Modal(modalElement);
                    modal.show();
                } else {
                    console.error('Bootstrap no está disponible');
                }
            }
        } catch (error) {
            console.error('Error showing QR modal:', error);
        }
    }

    /**
     * Descargar QR modal
     */
    downloadModalQR() {
        try {
            const qrImage = document.getElementById('modalQRImage');
            const code = document.getElementById('modalQRCode');
            
            if (qrImage && code && qrImage.src) {
                const link = document.createElement('a');
                link.download = `qr-pago-${code.textContent}.jpg`;
                link.href = qrImage.src;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        } catch (error) {
            console.error('Error downloading QR:', error);
        }
    }

    /**
     * Imprimir QR modal
     */
    printModalQR() {
        try {
            const qrImage = document.getElementById('modalQRImage');
            const code = document.getElementById('modalQRCode');
            
            if (qrImage && code && qrImage.src) {
                const printWindow = window.open('', '_blank');
                if (printWindow) {
                    printWindow.document.write(`
                        <html>
                            <head>
                                <title>Código QR - ${code.textContent}</title>
                                <style>
                                    body { text-align: center; font-family: Arial, sans-serif; }
                                    img { max-width: 300px; }
                                </style>
                            </head>
                            <body>
                                <h2>${this.merchantName}</h2>
                                <h3>Código QR de Pago</h3>
                                <img src="${qrImage.src}" alt="QR Code" />
                                <p><strong>Código:</strong> ${code.textContent}</p>
                            </body>
                        </html>
                    `);
                    printWindow.document.close();
                    printWindow.print();
                }
            }
        } catch (error) {
            console.error('Error printing QR:', error);
        }
    }

    /**
     * Confirmar cancelación de pago con SweetAlert
     */
    confirmCancelPayment(paymentCode, description, amount) {
        if (typeof Swal === 'undefined') {
            if (confirm(`¿Está seguro de cancelar la solicitud: ${description}?`)) {
                this.cancelPaymentRequest(paymentCode);
            }
            return;
        }

        try {
            Swal.fire({
                title: '¿Cancelar Solicitud?',
                html: `
                    <div class="text-start">
                        <p><strong>Descripción:</strong> ${this.escapeHtml(description)}</p>
                        <p><strong>Monto:</strong> ₡${this.escapeHtml(amount)}</p>
                        <p><strong>Código:</strong> ${this.escapeHtml(paymentCode)}</p>
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
        } catch (error) {
            console.error('Error in confirmCancelPayment:', error);
            if (confirm(`¿Está seguro de cancelar la solicitud: ${description}?`)) {
                this.cancelPaymentRequest(paymentCode);
            }
        }
    }

    /**
     * Escapar HTML para prevenir XSS
     */
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Cancelar pago vía formulario
     */
    cancelPaymentRequest(paymentCode) {
        try {
            if (typeof Swal !== 'undefined') {
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
            }
            
            // Enviar formulario
            const paymentCodeInput = document.getElementById('cancelPaymentCode');
            const cancelForm = document.getElementById('cancelPaymentForm');
            
            if (paymentCodeInput && cancelForm) {
                paymentCodeInput.value = paymentCode;
                cancelForm.submit();
            } else {
                console.error('Form elements not found');
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'No se pudo procesar la cancelación'
                    });
                }
            }
        } catch (error) {
            console.error('Error in cancelPaymentRequest:', error);
        }
    }

    /**
     * Refrescar pagos con animación
     */
    refreshPayments() {
        try {
            if (typeof Swal !== 'undefined') {
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
            } else {
                window.location.reload();
            }
        } catch (error) {
            console.error('Error in refreshPayments:', error);
            window.location.reload();
        }
    }

    /**
     * Configurar hover en filas de tabla
     */
    setupTableHover() {
        try {
            const tableRows = document.querySelectorAll('tbody tr');
            tableRows.forEach(row => {
                row.addEventListener('mouseenter', function() {
                    this.style.backgroundColor = '#f8f9fa';
                });
                
                row.addEventListener('mouseleave', function() {
                    this.style.backgroundColor = '';
                });
            });
        } catch (error) {
            console.error('Error setting up table hover:', error);
        }
    }

    /**
     * Configurar auto-refresh inteligente
     */
    setupAutoRefresh() {
        try {
            setInterval(() => {
                const activeTab = document.getElementById('active-tab');
                if (activeTab && activeTab.classList.contains('active')) {
                    if (this.activeCount > 0) {
                        window.location.reload();
                    }
                }
            }, 30000);
        } catch (error) {
            console.error('Error setting up auto-refresh:', error);
        }
    }

    /**
     * Manejar mensaje inicial de TempData
     */
    handleInitialMessage() {
        if (!this.message || this.message.trim() === '') {
            return;
        }

        try {
            let messageType = 'info';
            let cleanMessage = this.message;

            if (this.message.includes('✅')) {
                messageType = 'success';
                cleanMessage = this.message.replace(/✅/g, '').trim();
            } else if (this.message.includes('❌')) {
                messageType = 'error';
                cleanMessage = this.message.replace(/❌/g, '').trim();
            }
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: messageType,
                    title: messageType === 'success' ? '¡Éxito!' : messageType === 'error' ? 'Error' : 'Información',
                    text: cleanMessage,
                    confirmButtonText: 'Entendido',
                    timer: 5000,
                    timerProgressBar: true
                });
            } else if (cleanMessage) {
                alert(cleanMessage);
            }
        } catch (error) {
            console.error('Error handling initial message:', error);
        }
    }

    /**
     * Mostrar mensaje de éxito
     */
    showSuccessMessage(message) {
        try {
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'success',
                    title: '¡Éxito!',
                    text: message,
                    confirmButtonText: 'Entendido',
                    timer: 3000,
                    timerProgressBar: true
                });
            } else {
                alert(message);
            }
        } catch (error) {
            console.error('Error showing success message:', error);
            alert(message);
        }
    }

    /**
     * Mostrar mensaje de error
     */
    showErrorMessage(message) {
        try {
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: message,
                    confirmButtonText: 'Entendido'
                });
            } else {
                alert(message);
            }
        } catch (error) {
            console.error('Error showing error message:', error);
            alert(message);
        }
    }
}

// Crear instancia global de forma segura
try {
    window.UserPaymentStatusManager = new UserPaymentStatusManager();
} catch (error) {
    console.error('Error creating UserPaymentStatusManager:', error);
}

// Funciones globales para compatibilidad con Razor
function showQR(code, qrUrl) {
    try {
        if (window.UserPaymentStatusManager) {
            window.UserPaymentStatusManager.showQR(code, qrUrl);
        }
    } catch (error) {
        console.error('Error in showQR:', error);
    }
}

function downloadModalQR() {
    try {
        if (window.UserPaymentStatusManager) {
            window.UserPaymentStatusManager.downloadModalQR();
        }
    } catch (error) {
        console.error('Error in downloadModalQR:', error);
    }
}

function printModalQR() {
    try {
        if (window.UserPaymentStatusManager) {
            window.UserPaymentStatusManager.printModalQR();
        }
    } catch (error) {
        console.error('Error in printModalQR:', error);
    }
}

function confirmCancelPayment(paymentCode, description, amount) {
    try {
        if (window.UserPaymentStatusManager) {
            window.UserPaymentStatusManager.confirmCancelPayment(paymentCode, description, amount);
        }
    } catch (error) {
        console.error('Error in confirmCancelPayment:', error);
    }
}

function refreshPayments() {
    try {
        if (window.UserPaymentStatusManager) {
            window.UserPaymentStatusManager.refreshPayments();
        } else {
            window.location.reload();
        }
    } catch (error) {
        console.error('Error in refreshPayments:', error);
        window.location.reload();
    }
}