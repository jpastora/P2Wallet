/**
 * PaymentSuccess.js
 * Funcionalidades JavaScript para la página PaymentSuccess
 * P2Wallet - Sistema de Pagos
 */

class PaymentSuccessManager {
    constructor() {
        this.message = '';
        this.transactionId = 0;
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
            this.setupAnimations();
            this.setupShareFunctionality();
            this.handleInitialMessage();
            this.initialized = true;
        }
    }

    /**
     * Configurar datos desde Razor Page
     */
    configure(config) {
        if (config) {
            this.message = config.message || '';
            this.transactionId = config.transactionId || 0;
        }
    }

    /**
     * Configurar animaciones de entrada
     */
    setupAnimations() {
        try {
            // Animar icono de éxito
            const successIcon = document.querySelector('.success-icon i');
            if (successIcon) {
                successIcon.style.transform = 'scale(0)';
                successIcon.style.transition = 'transform 0.5s ease-out';
                
                setTimeout(() => {
                    successIcon.style.transform = 'scale(1)';
                }, 200);
            }

            // Animar aparición de cards
            const cards = document.querySelectorAll('.card');
            cards.forEach((card, index) => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
                
                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, 300 + (index * 100));
            });

        } catch (error) {
            console.error('Error setting up animations:', error);
        }
    }

    /**
     * Configurar funcionalidad de compartir
     */
    setupShareFunctionality() {
        try {
            // Event listener para botón de compartir
            const shareButton = document.querySelector('button[onclick="sharePayment()"]');
            if (shareButton) {
                shareButton.removeAttribute('onclick');
                shareButton.addEventListener('click', () => {
                    this.sharePayment();
                });
            }
        } catch (error) {
            console.error('Error setting up share functionality:', error);
        }
    }

    /**
     * Función para compartir pago
     */
    sharePayment() {
        try {
            const shareData = {
                title: 'Pago realizado con P2Wallet',
                text: '¡Acabo de realizar un pago con P2Wallet!',
                url: window.location.href
            };

            if (navigator.share) {
                // Usar Web Share API si está disponible
                navigator.share(shareData).catch(error => {
                    console.log('Error sharing:', error);
                    this.fallbackShare(shareData);
                });
            } else {
                this.fallbackShare(shareData);
            }
        } catch (error) {
            console.error('Error in sharePayment:', error);
            this.fallbackShare({
                title: 'Pago realizado con P2Wallet',
                text: '¡Acabo de realizar un pago con P2Wallet!',
                url: window.location.href
            });
        }
    }

    /**
     * Fallback para compartir cuando Web Share API no está disponible
     */
    fallbackShare(shareData) {
        try {
            const shareText = shareData.text + ' ' + shareData.url;
            
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(shareText).then(() => {
                    this.showSuccessMessage('¡Copiado!', 'Información del pago copiada al portapapeles');
                }).catch(() => {
                    this.promptShare(shareText);
                });
            } else {
                this.promptShare(shareText);
            }
        } catch (error) {
            console.error('Error in fallbackShare:', error);
            this.promptShare(shareData.text + ' ' + shareData.url);
        }
    }

    /**
     * Mostrar prompt para copiar texto
     */
    promptShare(text) {
        try {
            // Usar SweetAlert si está disponible
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Compartir Pago',
                    html: `
                        <p>Copia este texto para compartir:</p>
                        <textarea class="form-control mt-2" readonly style="height: 100px;">${text}</textarea>
                    `,
                    icon: 'info',
                    confirmButtonText: 'Cerrar',
                    didOpen: () => {
                        const textarea = Swal.getPopup().querySelector('textarea');
                        if (textarea) {
                            textarea.select();
                        }
                    }
                });
            } else {
                // Fallback básico
                prompt('Copia este texto para compartir:', text);
            }
        } catch (error) {
            console.error('Error in promptShare:', error);
            alert('Error al compartir. Intenta nuevamente.');
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
            let messageType = 'success'; // Por defecto success en página de éxito
            let cleanMessage = this.message;

            if (this.message.includes('?')) {
                messageType = 'success';
                cleanMessage = this.message.replace(/?/g, '').trim();
            } else if (this.message.includes('?')) {
                messageType = 'error';
                cleanMessage = this.message.replace(/?/g, '').trim();
            }
            
            if (typeof Swal !== 'undefined') {
                // Mostrar mensaje con animación especial para página de éxito
                Swal.fire({
                    icon: messageType,
                    title: messageType === 'success' ? '¡Éxito!' : 'Información',
                    text: cleanMessage,
                    confirmButtonText: 'Excelente',
                    timer: 4000,
                    timerProgressBar: true,
                    showClass: {
                        popup: 'animate__animated animate__fadeInDown'
                    },
                    hideClass: {
                        popup: 'animate__animated animate__fadeOutUp'
                    }
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
    showSuccessMessage(title, text) {
        try {
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'success',
                    title: title,
                    text: text,
                    timer: 2000,
                    showConfirmButton: false,
                    toast: true,
                    position: 'top-end'
                });
            } else {
                alert(text);
            }
        } catch (error) {
            console.error('Error showing success message:', error);
            alert(text);
        }
    }

    /**
     * Mostrar mensaje de error
     */
    showErrorMessage(title, text) {
        try {
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: title,
                    text: text,
                    confirmButtonText: 'Entendido'
                });
            } else {
                alert(text);
            }
        } catch (error) {
            console.error('Error showing error message:', error);
            alert(text);
        }
    }

    /**
     * Descargar comprobante
     */
    downloadReceipt() {
        try {
            if (this.transactionId > 0) {
                window.open(`/User/UserTransactionInvoice?transactionId=${this.transactionId}`, '_blank');
            } else {
                this.showErrorMessage('Error', 'No se puede descargar el comprobante. ID de transacción no válido.');
            }
        } catch (error) {
            console.error('Error downloading receipt:', error);
            this.showErrorMessage('Error', 'Error al descargar el comprobante.');
        }
    }

    /**
     * Ir a realizar otro pago
     */
    makeAnotherPayment() {
        try {
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Nuevo Pago',
                    text: '¿Quieres realizar otro pago?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Sí, pagar otro',
                    cancelButtonText: 'No, ir al panel',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = '/User/ScanPayment';
                    } else {
                        window.location.href = '/User/UserPanel';
                    }
                });
            } else {
                if (confirm('¿Quieres realizar otro pago?')) {
                    window.location.href = '/User/ScanPayment';
                } else {
                    window.location.href = '/User/UserPanel';
                }
            }
        } catch (error) {
            console.error('Error in makeAnotherPayment:', error);
            window.location.href = '/User/ScanPayment';
        }
    }

    /**
     * Volver al panel principal
     */
    goToPanel() {
        try {
            window.location.href = '/User/UserPanel';
        } catch (error) {
            console.error('Error in goToPanel:', error);
            window.location.href = '/';
        }
    }
}

// Crear instancia global de forma segura
try {
    window.PaymentSuccessManager = new PaymentSuccessManager();
} catch (error) {
    console.error('Error creating PaymentSuccessManager:', error);
}

// Funciones globales para compatibilidad con Razor
function sharePayment() {
    try {
        if (window.PaymentSuccessManager) {
            window.PaymentSuccessManager.sharePayment();
        }
    } catch (error) {
        console.error('Error in sharePayment:', error);
    }
}

function downloadReceipt() {
    try {
        if (window.PaymentSuccessManager) {
            window.PaymentSuccessManager.downloadReceipt();
        }
    } catch (error) {
        console.error('Error in downloadReceipt:', error);
    }
}

function makeAnotherPayment() {
    try {
        if (window.PaymentSuccessManager) {
            window.PaymentSuccessManager.makeAnotherPayment();
        } else {
            window.location.href = '/User/ScanPayment';
        }
    } catch (error) {
        console.error('Error in makeAnotherPayment:', error);
        window.location.href = '/User/ScanPayment';
    }
}

function goToPanel() {
    try {
        if (window.PaymentSuccessManager) {
            window.PaymentSuccessManager.goToPanel();
        } else {
            window.location.href = '/User/UserPanel';
        }
    } catch (error) {
        console.error('Error in goToPanel:', error);
        window.location.href = '/User/UserPanel';
    }
}