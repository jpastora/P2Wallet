/**
 * ScanPayment.js
 * Funcionalidades JavaScript para la página ScanPayment
 * P2Wallet - Sistema de Pagos
 */

class ScanPaymentManager {
    constructor() {
        this.qrScanner = null;
        this.videoElement = null;
        this.isScanning = false;
        this.cameraStream = null;
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
            this.setupQRScanner();
            this.setupFormHandlers();
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
        }
    }

    /**
     * Configurar escáner QR
     */
    setupQRScanner() {
        try {
            this.videoElement = document.getElementById('qr-video');
            const btnActivar = document.getElementById('btnActivarCamara');
            const btnDetener = document.getElementById('btnDetenerCamara');
            const scannerContainer = document.getElementById('scanner-container');
            const scanStatus = document.getElementById('scan-status');
            const paymentCodeInput = document.getElementById('paymentCodeInput');

            if (!btnActivar || !this.videoElement) {
                console.log('QR scanner elements not found, skipping setup');
                return;
            }

            // Botón para activar cámara
            btnActivar.addEventListener('click', async () => {
                try {
                    await this.startQRScanner();
                } catch (error) {
                    this.handleScannerError(error);
                }
            });

            // Botón para detener cámara
            if (btnDetener) {
                btnDetener.addEventListener('click', () => {
                    this.stopScanner();
                });
            }

            // Limpiar cámara al salir de la página
            window.addEventListener('beforeunload', () => {
                this.stopScanner();
            });

            // Manejar cambio de visibilidad de página
            document.addEventListener('visibilitychange', () => {
                if (document.hidden && this.isScanning) {
                    this.pauseScanner();
                } else if (!document.hidden && this.qrScanner && !this.isScanning) {
                    this.resumeScanner();
                }
            });

        } catch (error) {
            console.error('Error setting up QR scanner:', error);
        }
    }

    /**
     * Iniciar escáner QR
     */
    async startQRScanner() {
        const scanStatus = document.getElementById('scan-status');
        const scannerContainer = document.getElementById('scanner-container');
        const btnActivar = document.getElementById('btnActivarCamara');
        const btnDetener = document.getElementById('btnDetenerCamara');

        try {
            if (scanStatus) {
                scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Verificando permisos...</small>';
            }

            // Verificar soporte básico
            if (!this.checkCameraSupport()) {
                throw new Error('Tu navegador no soporta acceso a cámara. Usa Chrome, Firefox o Safari actualizado.');
            }

            // Solicitar permisos de cámara
            if (scanStatus) {
                scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Solicitando permisos de cámara...</small>';
            }
            
            await this.requestCameraPermission();

            // Mostrar interfaz de escáner
            if (scannerContainer) scannerContainer.classList.remove('d-none');
            if (btnActivar) btnActivar.classList.add('d-none');
            if (btnDetener) btnDetener.classList.remove('d-none');

            if (scanStatus) {
                scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Iniciando escáner QR...</small>';
            }

            // Verificar si QrScanner está disponible
            if (typeof QrScanner === 'undefined') {
                throw new Error('Librería QR Scanner no está disponible. Recarga la página e intenta nuevamente.');
            }

            // Inicializar escáner QR
            this.qrScanner = new QrScanner(
                this.videoElement,
                result => {
                    console.log('QR detectado:', result.data);
                    this.handleQRDetected(result.data);
                },
                {
                    onDecodeError: error => {
                        // Error de decodificación (normal cuando no hay QR visible)
                        if (this.isScanning && scanStatus) {
                            scanStatus.innerHTML = '<small class="text-muted">Buscando código QR...</small>';
                        }
                    },
                    preferredCamera: 'environment',
                    highlightScanRegion: true,
                    highlightCodeOutline: true,
                    maxScansPerSecond: 5,
                    calculateScanRegion: (video) => {
                        const smallerDimension = Math.min(video.videoWidth, video.videoHeight);
                        const scanRegionSize = Math.round(0.6 * smallerDimension);
                        return {
                            x: Math.round((video.videoWidth - scanRegionSize) / 2),
                            y: Math.round((video.videoHeight - scanRegionSize) / 2),
                            width: scanRegionSize,
                            height: scanRegionSize,
                        };
                    }
                }
            );

            // Usar stream existente si ya lo tenemos
            if (this.cameraStream && this.videoElement) {
                this.videoElement.srcObject = this.cameraStream;
            }

            // Iniciar escáner
            await this.qrScanner.start();
            this.isScanning = true;
            
            if (scanStatus) {
                scanStatus.innerHTML = '<small class="text-success"><i class="bi bi-camera-video me-1"></i>Cámara activa - Apunta al código QR</small>';
            }

        } catch (error) {
            console.error('Error al iniciar cámara:', error);
            this.handleScannerError(error);
        }
    }

    /**
     * Verificar soporte de cámara
     */
    checkCameraSupport() {
        return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);
    }

    /**
     * Solicitar permisos de cámara
     */
    async requestCameraPermission() {
        try {
            // Verificar si ya tenemos permisos
            if (navigator.permissions) {
                const permissions = await navigator.permissions.query({ name: 'camera' });
                
                if (permissions.state === 'denied') {
                    throw new Error('Permisos de cámara denegados. Ve a configuración del navegador para habilitarlos.');
                }
            }
            
            // Intentar acceso directo
            const stream = await navigator.mediaDevices.getUserMedia({ 
                video: { 
                    facingMode: 'environment',
                    width: { ideal: 1280 },
                    height: { ideal: 720 }
                } 
            });
            
            this.cameraStream = stream;
            return true;
            
        } catch (error) {
            console.error('Error solicitando permisos:', error);
            
            if (error.name === 'NotAllowedError') {
                throw new Error('Acceso a cámara denegado. Por favor permite el acceso y recarga la página.');
            } else if (error.name === 'NotFoundError') {
                throw new Error('No se encontró ninguna cámara en el dispositivo.');
            } else if (error.name === 'NotReadableError') {
                throw new Error('La cámara está siendo usada por otra aplicación.');
            } else {
                throw new Error('Error desconocido: ' + error.message);
            }
        }
    }

    /**
     * Manejar QR detectado
     */
    handleQRDetected(qrData) {
        if (!this.isScanning) return;

        console.log('Procesando QR:', qrData);
        
        // Detener escáner inmediatamente
        this.stopScanner();

        // Llenar campo de código
        const paymentCodeInput = document.getElementById('paymentCodeInput');
        if (paymentCodeInput) {
            paymentCodeInput.value = qrData;
        }

        // Procesar automáticamente
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: '¡QR Detectado!',
                text: 'Procesando pago...',
                icon: 'success',
                timer: 1500,
                timerProgressBar: true,
                showConfirmButton: false,
                allowOutsideClick: false
            }).then(() => {
                this.submitScanForm();
            });
        } else {
            this.submitScanForm();
        }
    }

    /**
     * Enviar formulario de escaneo
     */
    submitScanForm() {
        const scanForm = document.getElementById('scanForm');
        if (scanForm) {
            scanForm.submit();
        }
    }

    /**
     * Detener escáner
     */
    stopScanner() {
        console.log('Deteniendo escáner...');
        
        if (this.qrScanner) {
            this.qrScanner.destroy();
            this.qrScanner = null;
        }
        
        if (this.cameraStream) {
            this.cameraStream.getTracks().forEach(track => {
                track.stop();
                console.log('Track detenido:', track.label);
            });
            this.cameraStream = null;
        }
        
        if (this.videoElement) {
            this.videoElement.srcObject = null;
        }
        
        this.isScanning = false;
        
        // Restaurar interfaz
        this.restoreUI();
    }

    /**
     * Pausar escáner
     */
    pauseScanner() {
        if (this.qrScanner && this.isScanning) {
            console.log('Pausando escáner...');
            this.qrScanner.stop();
            this.isScanning = false;
        }
    }

    /**
     * Reanudar escáner
     */
    async resumeScanner() {
        if (this.qrScanner && !this.isScanning) {
            try {
                console.log('Reanudando escáner...');
                await this.qrScanner.start();
                this.isScanning = true;
            } catch (error) {
                console.error('Error reanudando escáner:', error);
            }
        }
    }

    /**
     * Restaurar UI
     */
    restoreUI() {
        const scannerContainer = document.getElementById('scanner-container');
        const btnActivar = document.getElementById('btnActivarCamara');
        const btnDetener = document.getElementById('btnDetenerCamara');
        
        if (scannerContainer) scannerContainer.classList.add('d-none');
        if (btnActivar) btnActivar.classList.remove('d-none');
        if (btnDetener) btnDetener.classList.add('d-none');
    }

    /**
     * Manejar errores del escáner
     */
    handleScannerError(error) {
        const scanStatus = document.getElementById('scan-status');
        
        if (scanStatus) {
            scanStatus.innerHTML = '<small class="text-danger"><i class="bi bi-exclamation-triangle me-1"></i>' + error.message + '</small>';
        }
        
        this.stopScanner();

        // Mostrar alerta con opciones
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Error de Cámara',
                html: `
                    <p>${error.message}</p>
                    <div class="mt-3">
                        <strong>Soluciones:</strong>
                        <ul class="text-start">
                            <li>Permite el acceso a la cámara cuando el navegador lo solicite</li>
                            <li>Verifica que ninguna otra aplicación esté usando la cámara</li>
                            <li>Recarga la página e intenta nuevamente</li>
                            <li>Usa el campo de texto para ingresar el código manualmente</li>
                        </ul>
                    </div>
                `,
                icon: 'error',
                confirmButtonText: 'Entendido',
                showCancelButton: true,
                cancelButtonText: 'Recargar Página',
                width: '500px'
            }).then((result) => {
                if (result.dismiss === Swal.DismissReason.cancel) {
                    window.location.reload();
                }
            });
        } else {
            alert(error.message);
        }
    }

    /**
     * Configurar handlers de formularios
     */
    setupFormHandlers() {
        document.querySelectorAll('input[name="promotionSelection"]').forEach(radio => {
            radio.addEventListener('change', (event) => {
                const selectedId = event.target.value;
                const selectedType = event.target.getAttribute('data-type') || '';
                const pct = parseFloat(event.target.getAttribute('data-pct')) || 0;
                this.selectPromotion(selectedId, selectedType, pct);
                this.updatePreview();
            });
        });
        const bankSelect = document.querySelector('select[name="SelectedBankAccountId"]');
        if (bankSelect && !bankSelect._promoBound) {
            bankSelect.addEventListener('change', () => {
                const paymentCode = this.getPaymentCode();
                const accountId = bankSelect.value;
                if (paymentCode && accountId) {
                    this.reloadPromotions(paymentCode, accountId, true);
                }
                this.updatePreview();
            });
            bankSelect._promoBound = true;
        }
    }

    getPaymentCode() {
        const el1 = document.getElementById('paymentCodeInput');
        if (el1 && el1.value) return el1.value.trim();
        const el2 = document.getElementById('paymentCodeInputHidden');
        if (el2 && el2.value) return el2.value.trim();
        const byName = document.querySelector('input[name="PaymentCode"]');
        if (byName && byName.value) return byName.value.trim();
        return '';
    }

    reloadPromotions(paymentCode, accountId, autoPreview = false) {
        const container = document.getElementById('promotionContainer');
        if (container) {
            container.classList.add('position-relative');
            container.innerHTML = `<div class=\"text-center text-muted py-2\"><div class=\"spinner-border spinner-border-sm me-2\"></div>Cargando promociones...</div>`;
        }
        const url = `/User/ScanPayment?handler=Promotions&paymentCode=${encodeURIComponent(paymentCode)}&accountId=${encodeURIComponent(accountId)}`;
        console.debug('Fetching promotions (merged merchant+financial) ->', url);
        fetch(url)
            .then(response => {
                if (!response.ok) throw new Error('Respuesta no válida del servidor');
                return response.json();
            })
            .then(data => {
                console.debug('Promotions loaded (handler):', data);
                this.updatePromotionsUI(Array.isArray(data) ? data : []);
                if (autoPreview) this.updatePreview();
            })
            .catch(error => {
                console.error('Error recargando promociones:', error);
                this.updatePromotionsUI([]);
                if (autoPreview) this.updatePreview();
            });
    }

    updatePromotionsUI(promotions) {
        const promoContainer = document.getElementById('promotionContainer');
        if (!promoContainer) return;
        const prevSelected = document.querySelector('input[name="promotionSelection"]:checked');
        const prevId = prevSelected ? prevSelected.value : '';
        promoContainer.innerHTML = '';
        promoContainer.innerHTML += `<div class=\"form-check mb-1">
            <input class=\"form-check-input\" type=\"radio\" name=\"promotionSelection\" id=\"noPromotion\" value=\"\" data-type=\"\" data-pct=\"0\" checked>
            <label class=\"form-check-label\" for=\"noPromotion\">Sin promoción</label>
        </div>`;
        if (promotions.length === 0) {
            promoContainer.innerHTML += `<div class=\"text-muted small mt-2\">No hay promociones disponibles para esta selección.</div>`;
        } else {
            promotions.forEach(promo => {
                const id = promo.Id ?? promo.id ?? '';
                const type = (promo.Type ?? promo.type ?? '').toString();
                const name = (promo.Name ?? promo.name ?? 'Promoción');
                const desc = (promo.Description ?? promo.description ?? '');
                const pct = promo.DiscountPercentage ?? promo.discountPercentage ?? 0;
                const pctLabel = pct > 0 ? ` - ${pct}% desc.` : '';
                const badge = type.toLowerCase().startsWith('finan') ? '<span class=\"badge bg-primary ms-1\">Banco</span>' : '<span class=\"badge bg-success ms-1\">Comercio</span>';
                promoContainer.innerHTML += `<div class=\"form-check mb-1\">
                    <input class=\"form-check-input\" type=\"radio\" name=\"promotionSelection\" id=\"promo${id}\" value=\"${id}\" data-type=\"${type}\" data-pct=\"${pct}\">
                    <label class=\"form-check-label\" for=\"promo${id}\"><strong>${this.escapeHtml(name)}</strong>${badge}${pctLabel}<br><small class=\"text-muted\">${this.escapeHtml(desc)}</small></label>
                </div>`;
            });
        }
        if (prevId && prevId !== '') {
            const toSelect = promoContainer.querySelector(`#promo${prevId}`);
            if (toSelect) {
                toSelect.checked = true;
                this.selectPromotion(prevId, toSelect.getAttribute('data-type') || '', parseFloat(toSelect.getAttribute('data-pct')) || 0);
            } else {
                this.selectPromotion('', '', 0);
            }
        } else {
            this.selectPromotion('', '', 0);
        }
        this.setupFormHandlers();
    }

    selectPromotion(id, type, pct = 0) {
        const promotionIdInput = document.querySelector('input[name="SelectedPromotionId"]');
        const promotionTypeInput = document.querySelector('input[name="SelectedPromotionType"]');
        if (promotionIdInput) promotionIdInput.value = id || '';
        if (promotionTypeInput) promotionTypeInput.value = type || '';
    }

    updatePreview() {
        const grossInput = document.getElementById('grossAmountValue');
        const taxInput = document.getElementById('taxAmountValue');
        const previewCard = document.getElementById('previewCard');
        if (!grossInput || !taxInput || !previewCard) return;
        const gross = parseFloat(grossInput.value) || 0;
        const tax = parseFloat(taxInput.value) || 0;
        const base = gross - tax;
        const selectedPromoRadio = document.querySelector('input[name="promotionSelection"]:checked');
        const pct = selectedPromoRadio ? (parseFloat(selectedPromoRadio.getAttribute('data-pct')) || 0) : 0;
        let discount = 0;
        if (pct > 0) {
            discount = base * (pct / 100.0);
        }
        const netAfter = Math.max(0, base - discount);
        const total = netAfter + tax;
        // Mostrar
        document.getElementById('pvGross').textContent = this.formatCurrency(gross);
        document.getElementById('pvTax').textContent = this.formatCurrency(tax);
        document.getElementById('pvBase').textContent = this.formatCurrency(base);
        if (pct > 0) {
            document.getElementById('pvDiscountRow').style.display = '';
            document.getElementById('pvPct').textContent = pct.toString();
            document.getElementById('pvDiscount').textContent = '-' + this.formatCurrency(discount);
        } else {
            document.getElementById('pvDiscountRow').style.display = 'none';
        }
        document.getElementById('pvTotal').textContent = this.formatCurrency(total);
        previewCard.style.display = 'block';
    }

    formatCurrency(val) {
        return new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC', minimumFractionDigits: 2 }).format(val);
    }

    escapeHtml(str) {
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
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

            if (this.message.includes('?')) {
                messageType = 'success';
                cleanMessage = this.message.replace(/?/g, '').trim();
            } else if (this.message.includes('?')) {
                messageType = 'error';
                cleanMessage = this.message.replace(/?/g, '').trim();
            }
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: messageType,
                    title: messageType === 'success' ? '¡Éxito!' : messageType === 'error' ? 'Error' : 'Información',
                    text: cleanMessage,
                    confirmButtonText: 'Entendido',
                    timer: messageType === 'success' ? 3000 : 5000,
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
     * Buscar otro QR
     */
    buscarOtro() {
        this.stopScanner();
        window.location.href = '/User/ScanPayment';
    }

    /**
     * Pagar otro QR
     */
    pagarOtro() {
        this.stopScanner();
        window.location.href = '/User/ScanPayment';
    }
}

// Crear instancia global de forma segura
try {
    window.ScanPaymentManager = new ScanPaymentManager();
} catch (error) {
    console.error('Error creating ScanPaymentManager:', error);
}

// Funciones globales para compatibilidad con Razor
function selectPromotion(id, type) {
    try {
        if (window.ScanPaymentManager) {
            window.ScanPaymentManager.selectPromotion(id, type);
        }
    } catch (error) {
        console.error('Error in selectPromotion:', error);
    }
}

function buscarOtro() {
    try {
        if (window.ScanPaymentManager) {
            window.ScanPaymentManager.buscarOtro();
        } else {
            window.location.href = '/User/ScanPayment';
        }
    } catch (error) {
        console.error('Error in buscarOtro:', error);
        window.location.href = '/User/ScanPayment';
    }
}

function pagarOtro() {
    try {
        if (window.ScanPaymentManager) {
            window.ScanPaymentManager.pagarOtro();
        } else {
            window.location.href = '/User/ScanPayment';
        }
    } catch (error) {
        console.error('Error in pagarOtro:', error);
        window.location.href = '/User/ScanPayment';
    }
}