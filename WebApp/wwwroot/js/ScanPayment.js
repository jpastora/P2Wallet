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
            document.addEventListener('DOMContentLoaded', () => this.onDOMReady());
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
            // Delay auto-load to ensure DOM is fully ready
            setTimeout(() => this.autoLoadInitialPromotions(), 100);
            this.initialized = true;
        }
    }

    /**
     * Configurar datos desde Razor Page
     */
    configure(config) { if (config) this.message = config.message || ''; }

    /**
     * Auto-cargar promociones al entrar cuando ya existe PaymentRequest y cuentas
     * MEJORADO: Mejor detección de cuenta seleccionada y financialEntityId
     */
    autoLoadInitialPromotions() {
        try {
            const paymentCode = this.getPaymentCode();
            const bankSelect = document.querySelector('select[name="SelectedBankAccountId"]');
            
            console.debug('[ScanPayment] Auto-load check:', { 
                paymentCode: paymentCode || 'EMPTY', 
                bankSelectExists: !!bankSelect,
                bankSelectValue: bankSelect?.value || 'EMPTY',
                totalOptions: bankSelect?.options?.length || 0
            });
            
            if (!paymentCode || !bankSelect) {
                console.debug('[ScanPayment] Auto-load skipped: missing paymentCode or bankSelect');
                return;
            }

            // Si no hay value seleccionado pero hay opciones, seleccionar la primera válida
            if (!bankSelect.value && bankSelect.options.length > 1) {
                for (let i = 1; i < bankSelect.options.length; i++) {
                    const option = bankSelect.options[i];
                    if (option.value && option.value.trim() !== '') {
                        console.debug('[ScanPayment] Auto-selecting first account:', option.value);
                        bankSelect.value = option.value;
                        break;
                    }
                }
            }

            // Ahora verificar si tenemos una cuenta seleccionada
            if (bankSelect.value) {
                const selectedIndex = bankSelect.selectedIndex;
                const finId = bankSelect.options[selectedIndex]?.getAttribute('data-financial');
                
                console.debug('[ScanPayment] Auto-loading promotions on init', { 
                    paymentCode, 
                    accountId: bankSelect.value, 
                    finId: finId || 'EMPTY',
                    selectedIndex
                });
                
                this.reloadPromotions(paymentCode, bankSelect.value, true, finId);
            } else {
                console.debug('[ScanPayment] Auto-load skipped: no account available or selected');
                // Si no hay cuenta seleccionada, al menos cargar promociones del comercio
                this.reloadPromotions(paymentCode, '', false);
            }
        } catch (e) { 
            console.warn('[ScanPayment] Auto-load promotions failed:', e); 
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
            if (!btnActivar || !this.videoElement) return;
            btnActivar.addEventListener('click', async () => { try { await this.startQRScanner(); } catch (e) { this.handleScannerError(e); } });
            if (btnDetener) btnDetener.addEventListener('click', () => this.stopScanner());
            window.addEventListener('beforeunload', () => this.stopScanner());
            document.addEventListener('visibilitychange', () => {
                if (document.hidden && this.isScanning) this.pauseScanner();
                else if (!document.hidden && this.qrScanner && !this.isScanning) this.resumeScanner();
            });
        } catch (e) { console.error('Error setting up QR scanner:', e); }
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
            if (scanStatus) scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Verificando permisos...</small>';
            if (!this.checkCameraSupport()) throw new Error('Tu navegador no soporta acceso a cámara. Usa Chrome, Firefox o Safari actualizado.');
            if (scanStatus) scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Solicitando permisos de cámara...</small>';
            await this.requestCameraPermission();
            if (scannerContainer) scannerContainer.classList.remove('d-none');
            if (btnActivar) btnActivar.classList.add('d-none');
            if (btnDetener) btnDetener.classList.remove('d-none');
            if (scanStatus) scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Iniciando escáner QR...</small>';
            
            // Verificar QrScanner con mejor detección y espera
            let QrScannerClass = window.QrScanner || QrScanner;
            
            // Si no está disponible, esperar un poco más
            if (typeof QrScannerClass === 'undefined') {
                if (scanStatus) scanStatus.innerHTML = '<small class="text-info"><i class="spinner-border spinner-border-sm me-1"></i>Cargando librería QR...</small>';
                
                // Intentar varias veces con delay
                for (let i = 0; i < 5; i++) {
                    await new Promise(resolve => setTimeout(resolve, 500));
                    QrScannerClass = window.QrScanner || QrScanner;
                    if (typeof QrScannerClass !== 'undefined') break;
                }
            }
            
            if (typeof QrScannerClass === 'undefined') {
                throw new Error('Librería QR Scanner no disponible. Recarga la página e intenta nuevamente.');
            }
            
            this.qrScanner = new QrScannerClass(this.videoElement, r => { console.log('QR detectado:', r.data); this.handleQRDetected(r.data); }, {
                onDecodeError: () => { if (this.isScanning && scanStatus) scanStatus.innerHTML = '<small class="text-muted">Buscando código QR...</small>'; },
                preferredCamera: 'environment', highlightScanRegion: true, highlightCodeOutline: true, maxScansPerSecond: 5,
                calculateScanRegion: v => { const s = Math.min(v.videoWidth, v.videoHeight); const size = Math.round(0.6 * s); return { x: Math.round((v.videoWidth - size)/2), y: Math.round((v.videoHeight - size)/2), width: size, height: size }; }
            });
            if (this.cameraStream && this.videoElement) this.videoElement.srcObject = this.cameraStream;
            await this.qrScanner.start();
            this.isScanning = true;
            if (scanStatus) scanStatus.innerHTML = '<small class="text-success"><i class="bi bi-camera-video me-1"></i>Cámara activa - Apunta al código QR</small>';
        } catch (e) { console.error('Error al iniciar cámara:', e); this.handleScannerError(e); }
    }

    /**
     * Verificar soporte de cámara
     */
    checkCameraSupport() { return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia); }

    /**
     * Solicitar permisos de cámara
     */
    async requestCameraPermission() {
        try {
            if (navigator.permissions) {
                try {
                    const perm = await navigator.permissions.query({ name: 'camera' });
                    if (perm.state === 'denied') throw new Error('Permisos de cámara denegados. Ve a configuración del navegador para habilitarlos.');
                } catch { /* some browsers throw */ }
            }
            const stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment', width: { ideal: 1280 }, height: { ideal: 720 } } });
            this.cameraStream = stream; return true;
        } catch (e) {
            if (e.name === 'NotAllowedError') throw new Error('Acceso a cámara denegado. Por favor permite el acceso y recarga la página.');
            if (e.name === 'NotFoundError') throw new Error('No se encontró ninguna cámara en el dispositivo.');
            if (e.name === 'NotReadableError') throw new Error('La cámara está siendo usada por otra aplicación.');
            throw new Error('Error desconocido: ' + e.message);
        }
    }

    /**
     * Manejar QR detectado
     */
    handleQRDetected(qrData) {
        if (!this.isScanning) return;
        this.stopScanner();
        const paymentCodeInput = document.getElementById('paymentCodeInput');
        if (paymentCodeInput) paymentCodeInput.value = qrData;
        if (typeof Swal !== 'undefined') {
            Swal.fire({ title: '¡QR Detectado!', text: 'Procesando pago...', icon: 'success', timer: 1500, timerProgressBar: true, showConfirmButton: false, allowOutsideClick: false })
                .then(() => this.submitScanForm());
        } else { this.submitScanForm(); }
    }

    /**
     * Enviar formulario de escaneo
     */
    submitScanForm() { const f = document.getElementById('scanForm'); if (f) f.submit(); }

    /**
     * Detener escáner
     */
    stopScanner() {
        if (this.qrScanner) { this.qrScanner.destroy(); this.qrScanner = null; }
        if (this.cameraStream) { this.cameraStream.getTracks().forEach(t => t.stop()); this.cameraStream = null; }
        if (this.videoElement) this.videoElement.srcObject = null;
        this.isScanning = false; this.restoreUI();
    }

    /**
     * Pausar escáner
     */
    pauseScanner() { if (this.qrScanner && this.isScanning) { this.qrScanner.stop(); this.isScanning = false; } }
    /**
     * Reanudar escáner
     */
    async resumeScanner() { if (this.qrScanner && !this.isScanning) { try { await this.qrScanner.start(); this.isScanning = true; } catch (e) { console.error('Error reanudando escáner:', e); } } }

    /**
     * Restaurar UI
     */
    restoreUI() {
        const sc = document.getElementById('scanner-container');
        const a = document.getElementById('btnActivarCamara');
        const d = document.getElementById('btnDetenerCamara');
        if (sc) sc.classList.add('d-none'); if (a) a.classList.remove('d-none'); if (d) d.classList.add('d-none');
    }

    /**
     * Manejar errores del escáner
     */
    handleScannerError(error) {
        const st = document.getElementById('scan-status');
        if (st) st.innerHTML = '<small class="text-danger"><i class="bi bi-exclamation-triangle me-1"></i>' + error.message + '</small>';
        this.stopScanner();
        if (typeof Swal !== 'undefined') {
            Swal.fire({ title: 'Error de Cámara', html: `<p>${error.message}</p><div class="mt-3"><strong>Soluciones:</strong><ul class="text-start"><li>Permite el acceso a la cámara</li><li>Verifica que no esté en uso</li><li>Recarga la página</li><li>Ingresa el código manualmente</li></ul></div>`, icon: 'error', confirmButtonText: 'Entendido', showCancelButton: true, cancelButtonText: 'Recargar Página', width: '500px' })
                .then(r => { if (r.dismiss === Swal.DismissReason.cancel) window.location.reload(); });
        } else alert(error.message);
    }

    /**
     * Configurar handlers de formularios
     */
    setupFormHandlers() {
        document.querySelectorAll('input[name="promotionSelection"]').forEach(r => {
            if (!r._boundPromo) {
                r.addEventListener('change', e => {
                    const selectedId = e.target.value;
                    const selectedType = e.target.getAttribute('data-type') || '';
                    const pct = parseFloat(e.target.getAttribute('data-pct')) || 0;
                    this.selectPromotion(selectedId, selectedType, pct);
                    this.updatePreview();
                });
                r._boundPromo = true;
            }
        });
        const bankSelect = document.querySelector('select[name="SelectedBankAccountId"]');
        if (bankSelect && !bankSelect._promoBound) {
            bankSelect.addEventListener('change', () => {
                const paymentCode = this.getPaymentCode();
                const accountId = bankSelect.value;
                const finId = bankSelect.options[bankSelect.selectedIndex]?.getAttribute('data-financial');
                
                console.debug('[ScanPayment] Bank selection changed:', { 
                    paymentCode: paymentCode || 'EMPTY', 
                    accountId: accountId || 'EMPTY', 
                    finId: finId || 'EMPTY' 
                });
                
                if (paymentCode && accountId) {
                    this.reloadPromotions(paymentCode, accountId, true, finId);
                } else if (paymentCode) {
                    // Si no hay cuenta seleccionada, cargar solo promociones del comercio
                    this.reloadPromotions(paymentCode, '', false);
                }
                this.updatePreview();
            });
            bankSelect._promoBound = true;
        }
    }

    getPaymentCode() {
        const el = document.getElementById('paymentCodeInput');
        if (el && el.value) return el.value.trim();
        const byName = document.querySelector('input[name="PaymentCode"]');
        if (byName && byName.value) return byName.value.trim();
        return '';
    }

    reloadPromotions(paymentCode, accountId, autoPreview = false, financialEntityId = null) {
        const container = document.getElementById('promotionContainer');
        if (container) {
            container.classList.add('position-relative');
            container.innerHTML = `<div class="text-center text-muted py-2"><div class="spinner-border spinner-border-sm me-2"></div>Cargando promociones...</div>`;
        }
        
        let url = `/User/ScanPayment?handler=Promotions&paymentCode=${encodeURIComponent(paymentCode)}`;
        if (accountId) {
            url += `&accountId=${encodeURIComponent(accountId)}`;
        }
        if (financialEntityId) {
            url += `&financialEntityId=${encodeURIComponent(financialEntityId)}`;
        }
        
        console.debug('[ScanPayment] Fetching promotions', { url, paymentCode, accountId, financialEntityId });
        
        fetch(url)
            .then(r => { 
                console.debug('[ScanPayment] Response status:', r.status);
                if (!r.ok) throw new Error(`HTTP ${r.status}: Respuesta no válida del servidor`); 
                return r.json(); 
            })
            .then(data => {
                console.debug('[ScanPayment] Promotions loaded', { count: data?.length || 0, data });
                this.updatePromotionsUI(Array.isArray(data) ? data : []);
                if (autoPreview) this.updatePreview();
            })
            .catch(err => {
                console.error('[ScanPayment] Error recargando promociones:', err);
                if (container) {
                    container.innerHTML = `<div class="text-center text-danger py-2"><small><i class="bi bi-exclamation-triangle me-1"></i>Error cargando promociones: ${err.message}</small></div>`;
                }
                this.updatePromotionsUI([]);
                if (autoPreview) this.updatePreview();
            });
    }

    updatePromotionsUI(promotions) {
        const promoContainer = document.getElementById('promotionContainer');
        if (!promoContainer) return;
        
        const prevSelected = document.querySelector('input[name="promotionSelection"]:checked');
        const prevId = prevSelected ? prevSelected.value : '';
        
        // Limpiar contenedor
        promoContainer.innerHTML = '';
        
        const noPromoDiv = document.createElement('div');
        noPromoDiv.className = 'form-check mb-1';
        
        const noPromoInput = document.createElement('input');
        noPromoInput.className = 'form-check-input';
        noPromoInput.type = 'radio';
        noPromoInput.name = 'promotionSelection';
        noPromoInput.id = 'noPromotion';
        noPromoInput.value = '';
        noPromoInput.setAttribute('data-type', '');
        noPromoInput.setAttribute('data-pct', '0');
        noPromoInput.checked = true;
        
        const noPromoLabel = document.createElement('label');
        noPromoLabel.className = 'form-check-label';
        noPromoLabel.setAttribute('for', 'noPromotion');
        noPromoLabel.textContent = 'Sin promoción';
        
        noPromoDiv.appendChild(noPromoInput);
        noPromoDiv.appendChild(noPromoLabel);
        promoContainer.appendChild(noPromoDiv);
        
        if (promotions.length === 0) {
            const noPromosDiv = document.createElement('div');
            noPromosDiv.className = 'text-muted small mt-2';
            noPromosDiv.textContent = 'No hay promociones disponibles para esta selección.';
            promoContainer.appendChild(noPromosDiv);
        } else {
            promotions.forEach(p => {
                const id = p.Id ?? p.id ?? p.PromotionID ?? '';
                const type = (p.Type ?? p.type ?? p.PromotionType ?? '').toString();
                const name = (p.Name ?? p.name ?? 'Promoción');
                const desc = (p.Description ?? p.description ?? '');
                const pct = p.DiscountPercentage ?? p.discountPercentage ?? 0;
                const pctLabel = pct > 0 ? ` - ${pct}% desc.` : '';
                const badge = type.toLowerCase().startsWith('finan') || type.toLowerCase() === 'financial' ? '<span class="badge bg-primary ms-1">Banco</span>' : '<span class="badge bg-success ms-1">Comercio</span>';
                
                const promoDiv = document.createElement('div');
                promoDiv.className = 'form-check mb-1';
                
                const promoInput = document.createElement('input');
                promoInput.className = 'form-check-input';
                promoInput.type = 'radio';
                promoInput.name = 'promotionSelection';
                promoInput.id = `promo${id}`;
                promoInput.value = id;
                promoInput.setAttribute('data-type', type);
                promoInput.setAttribute('data-pct', pct);
                
                const promoLabel = document.createElement('label');
                promoLabel.className = 'form-check-label';
                promoLabel.setAttribute('for', `promo${id}`);
                promoLabel.innerHTML = `<strong>${this.escapeHtml(name)}</strong>${badge}${pctLabel}<br><small class="text-muted">${this.escapeHtml(desc)}</small>`;
                
                promoDiv.appendChild(promoInput);
                promoDiv.appendChild(promoLabel);
                promoContainer.appendChild(promoDiv);
            });
        }
        
        // Restaurar selección previa si existe
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
        let discount = 0; if (pct > 0) discount = base * (pct / 100.0);
        const netAfter = Math.max(0, base - discount);
        const total = netAfter + tax;
        document.getElementById('pvGross').textContent = this.formatCurrency(gross);
        document.getElementById('pvTax').textContent = this.formatCurrency(tax);
        document.getElementById('pvBase').textContent = this.formatCurrency(base);
        if (pct > 0) {
            document.getElementById('pvDiscountRow').style.display = '';
            document.getElementById('pvPct').textContent = pct.toString();
            document.getElementById('pvDiscount').textContent = '-' + this.formatCurrency(discount);
        } else { document.getElementById('pvDiscountRow').style.display = 'none'; }
        document.getElementById('pvTotal').textContent = this.formatCurrency(total);
        previewCard.style.display = 'block';
    }

    formatCurrency(val) { return new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC', minimumFractionDigits: 2 }).format(val); }

    escapeHtml(str) { return String(str).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;').replace(/'/g,'&#039;'); }

    /**
     * Manejar mensaje inicial de TempData
     * CORREGIDO: Evitar caracteres problemáticos en regex
     */
    handleInitialMessage() {
        if (!this.message || this.message.trim() === '') return;
        try {
            let type = 'info'; 
            let clean = this.message;
            
            // Buscar indicadores de éxito o error de forma más segura
            if (this.message.includes('exitosa') || this.message.includes('éxito') || this.message.includes('correcto')) { 
                type = 'success'; 
            } else if (this.message.includes('error') || this.message.includes('fallo') || this.message.includes('incorrecto')) {
                type = 'error';
            }
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({ 
                    icon: type, 
                    title: type === 'success' ? '¡Éxito!' : type === 'error' ? 'Error' : 'Información', 
                    text: clean, 
                    confirmButtonText: 'Entendido', 
                    timer: type === 'success' ? 3000 : undefined, 
                    timerProgressBar: true 
                });
            } else alert(clean);
        } catch (e) { 
            console.error('Error handling initial message:', e); 
        }
    }

    buscarOtro() { this.stopScanner(); window.location.href = '/User/ScanPayment'; }
    pagarOtro() { this.stopScanner(); window.location.href = '/User/ScanPayment'; }
}

try { window.ScanPaymentManager = new ScanPaymentManager(); } catch (e) { console.error('Error creating ScanPaymentManager:', e); }

function selectPromotion(id, type) { try { window.ScanPaymentManager?.selectPromotion(id, type); } catch (e) { console.error('Error in selectPromotion:', e); } }
function buscarOtro() { try { window.ScanPaymentManager?.buscarOtro(); } catch (e) { console.error('Error in buscarOtro:', e); window.location.href = '/User/ScanPayment'; } }
function pagarOtro() { try { window.ScanPaymentManager?.pagarOtro(); } catch (e) { console.error('Error in pagarOtro:', e); window.location.href = '/User/ScanPayment'; } }