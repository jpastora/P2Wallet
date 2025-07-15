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
                "columns": columns
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

        Swal.fire(
            title,
            message,
            icon
        );

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
                } else if (jqXHR.responseText) {
                    const response = JSON.parse(jqXHR.responseText)
                    if (response.message)
                        message = response.message;
                    else
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
            footer: 'Yavi App'
        });
    }
}