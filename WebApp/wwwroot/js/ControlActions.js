function ControlActions() {
    'use strict';
    const self = this;

    // ----------------
    // CONFIGURACIÓN
    // ----------------
    this.URL_API = "https://p2wallet-api-eyefddgeeda9c2fk.eastus-01.azurewebsites.net/api/";

    this.GetUrlApiService = function (service) {
        return self.URL_API + service;
    };

    // ----------------
    // INICIALIZACIÓN DE LA PÁGINA
    // ----------------
    this.InitializePage = function() {
        // Inicializar smooth scrolling
        self.InitSmoothScrolling();
        
        // Inicializar carousel
        self.InitCarousel();
        
        // Inicializar tooltips
        self.InitTooltips();
        
        // Inicializar animaciones en scroll
        self.InitScrollAnimations();
        
        console.log('Página Yavi inicializada correctamente');
    };

    this.InitSmoothScrolling = function() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    };

    this.InitCarousel = function() {
        const carouselElement = document.querySelector('#heroCarousel');
        if (carouselElement) {
            const carousel = new bootstrap.Carousel(carouselElement, {
                interval: 5000,
                ride: 'carousel',
                pause: 'hover'
            });
            
            // Pausar en hover
            carouselElement.addEventListener('mouseenter', () => {
                carousel.pause();
            });
            
            carouselElement.addEventListener('mouseleave', () => {
                carousel.cycle();
            });
        }
    };

    this.InitTooltips = function() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    };

    this.InitScrollAnimations = function() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }
            });
        }, observerOptions);

        // Aplicar animaciones a las secciones
        document.querySelectorAll('section').forEach(section => {
            section.style.opacity = '0';
            section.style.transform = 'translateY(30px)';
            section.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            observer.observe(section);
        });
    };

    // ----------------
    // MANEJO DE TABLAS (DataTables)
    // ----------------
    this.FillTable = function (service, tableId, refresh) {
        // Almacena las columnas en un atributo de la tabla para evitar recalcularlas.
        if (!$.fn.DataTable.isDataTable('#' + tableId)) {
            const columnsDataName = $('#' + tableId).attr("ColumnsDataName");
            const columns = columnsDataName.split(',').map(name => ({ data: name }));

            $('#' + tableId).DataTable({
                "processing": true,
                "ajax": {
                    "url": self.GetUrlApiService(service),
                    "dataSrc": "" 
                },
                "columns": columns,
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json"
                }
            });
        } else if (refresh) {
            $('#' + tableId).DataTable().ajax.reload();
        }
    };

    this.GetSelectedRow = function (tableId) {
        const data = sessionStorage.getItem(tableId + '_selected');
        return data ? JSON.parse(data) : null;
    };

    // ----------------
    // MANEJO DE FORMULARIOS
    // ----------------
    this.BindFields = function (formId, data) {
        $('#' + formId + ' *').filter(':input').each(function () {
            const columnDataName = $(this).attr("ColumnDataName");
            if (data[columnDataName]) {
                this.value = data[columnDataName];
            }
        });
    };

    this.GetDataForm = function (formId) {
        const data = {};
        $('#' + formId + ' *').filter(':input').each(function () {
            const columnDataName = $(this).attr("ColumnDataName");
            if (columnDataName) {
                data[columnDataName] = this.value;
            }
        });
        return data;
    };

    this.ValidateForm = function(formId) {
        const form = document.getElementById(formId);
        if (!form) return false;
        
        let isValid = true;
        const requiredFields = form.querySelectorAll('[required]');
        
        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                field.classList.add('is-invalid');
                isValid = false;
            } else {
                field.classList.remove('is-invalid');
                field.classList.add('is-valid');
            }
        });
        
        return isValid;
    };

    // ----------------
    // UTILIDADES PARA LA PÁGINA DE INICIO
    // ----------------
    this.ShowPromoDetails = function(promoId) {
        // Mostrar detalles de una promoción específica
        Swal.fire({
            title: 'Detalles de la Promoción',
            text: 'Aquí irían los detalles específicos de la promoción seleccionada.',
            icon: 'info',
            confirmButtonText: 'Entendido',
            confirmButtonColor: '#553DF2'
        });
    };

    this.ContactMerchant = function(merchantName) {
        // Simular contacto con comercio
        Swal.fire({
            title: 'Contactar ' + merchantName,
            text: '¿Te gustaría recibir más información sobre este comercio?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Sí, contactar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#553DF2'
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.fire(
                    '¡Perfecto!',
                    'Te contactaremos pronto con más información.',
                    'success'
                );
            }
        });
    };

    this.SubscribeToNewsletter = function(email) {
        if (!email || !self.ValidateEmail(email)) {
            Swal.fire({
                title: 'Email inválido',
                text: 'Por favor ingresa un email válido.',
                icon: 'error'
            });
            return;
        }

        // Simular suscripción al newsletter
        Swal.fire({
            title: '¡Suscripción exitosa!',
            text: 'Te has suscrito correctamente a nuestro newsletter.',
            icon: 'success',
            confirmButtonColor: '#553DF2'
        });
    };

    this.ValidateEmail = function(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    };

    // ----------------
    // LLAMADAS AL API (AJAX)
    // ----------------
    this.PostToAPI = function (service, data, callBackFunction) {
        $.ajax({
            type: "POST",
            url: self.GetUrlApiService(service),
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                _handleSuccess(response, callBackFunction);
            },
            error: _handleError
        });
    };

    this.PutToAPI = function (service, data, callBackFunction) {
        $.ajax({
            type: "PUT",
            url: self.GetUrlApiService(service),
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                _handleSuccess(response, callBackFunction);
            },
            error: _handleError
        });
    };

    this.DeleteToAPI = function (service, data, callBackFunction) {
        $.ajax({
            type: 'DELETE',
            url: self.GetUrlApiService(service),
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                _handleSuccess(response, callBackFunction);
            },
            error: _handleError
        });
    };

    this.GetToApi = function (service, callBackFunction) {
        $.get(self.GetUrlApiService(service), function (response) {
            if (callBackFunction) {
                callBackFunction(response);
            }
        }).fail(_handleError);
    };

    // ----------------
    // FUNCIONES AUXILIARES PRIVADAS
    // ----------------
    function _handleSuccess(response, callBackFunction) {
        let message = 'La transacción se completó correctamente.';
        let icon = 'success';
        let title = '¡Éxito!';

        if (response) {
            if (response.message) {
                message = response.message;
            }
            if (response.icon) {
                icon = response.icon;
            }
            if (response.title) {
                title = response.title;
            }
        }

        Swal.fire({
            title: title,
            text: message,
            icon: icon,
            confirmButtonColor: '#553DF2'
        });

        if (callBackFunction) {
            callBackFunction(response);
        }
    }

    function _handleError(jqXHR) {
        let message = "Ha ocurrido un error inesperado.";
        let icon = 'error';
        let title = 'Oops...';

        if (jqXHR.responseText) {
            try {
                const responseJson = JSON.parse(jqXHR.responseText);

                if (responseJson.title) {
                    title = responseJson.title;
                }

                if (responseJson.errors) {
                    message = Object.values(responseJson.errors).flat().join("<br/> ");
                } else if (responseJson.message) {
                    message = responseJson.message;
                } else {
                    message = jqXHR.responseText;
                }

                if (responseJson.icon) {
                    icon = responseJson.icon;
                }

            } catch (e) {
                message = jqXHR.responseText;
            }
        }

        Swal.fire({
            icon: icon,
            title: title,
            html: message,
            footer: 'Yavi App',
            confirmButtonColor: '#553DF2'
        });
    }
}

// Inicializar automáticamente cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    const controlActions = new ControlActions();
    controlActions.InitializePage();
    
    // Hacer disponible globalmente
    window.yaviController = controlActions;
});