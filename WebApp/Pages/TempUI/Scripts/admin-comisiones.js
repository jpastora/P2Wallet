
document.addEventListener('DOMContentLoaded', function () {
    // Validación de comisiones al cambiar
    document.querySelectorAll('.commission-input input').forEach(input => {
        input.addEventListener('change', function () {
            const min = parseFloat(document.querySelector('.commission-settings input:nth-of-type(1)').value);
            const max = parseFloat(document.querySelector('.commission-settings input:nth-of-type(2)').value);
            const value = parseFloat(this.value);

            if (value < min) {
                this.value = min;
                alert(`La comisión no puede ser menor a ${min}%`);
            } else if (value > max) {
                this.value = max;
                alert(`La comisión no puede ser mayor a ${max}%`);
            }
        });
    });

    // Aplicar cambio individual
    document.querySelectorAll('.aplicar-cambio').forEach(btn => {
        btn.addEventListener('click', function () {
            const row = this.closest('tr');
            const entidad = row.cells[0].textContent;
            const tipo = row.cells[1].textContent;
            const nuevaComision = row.querySelector('input').value;

            // Aquí iría la llamada AJAX para guardar el cambio
            console.log(`Actualizando comisión para ${entidad} (${tipo}): ${nuevaComision}%`);
            alert(`Comisión actualizada para ${entidad} (${tipo}): ${nuevaComision}%`);
        });
    });

    // Revertir cambio individual
    document.querySelectorAll('.revertir-cambio').forEach(btn => {
        btn.addEventListener('click', function () {
            const row = this.closest('tr');
            const comisionActual = row.cells[2].textContent.replace('%', '');
            row.querySelector('input').value = comisionActual;
        });
    });

    // Guardar todos los cambios
    document.getElementById('aplicarComisiones').addEventListener('click', function () {
        const cambios = [];
        document.querySelectorAll('tbody tr').forEach(row => {
            const entidad = row.cells[0].textContent;
            const tipo = row.cells[1].textContent;
            const nuevaComision = row.querySelector('input').value;
            const comisionActual = row.cells[2].textContent.replace('%', '');

            if (nuevaComision !== comisionActual) {
                cambios.push({
                    entidad,
                    tipo,
                    comision: nuevaComision
                });
            }
        });

        if (cambios.length > 0) {
            // Aquí iría la llamada AJAX para guardar todos los cambios
            console.log('Cambios a guardar:', cambios);
            alert(`Se guardaron ${cambios.length} cambios de comisión`);
        } else {
            alert('No hay cambios para guardar');
        }
    });

    // Guardar configuración global
    document.getElementById('guardarConfigGlobal').addEventListener('click', function () {
        const inputs = document.querySelectorAll('.commission-settings input');
        const config = {
            min: inputs[0].value,
            max: inputs[1].value,
            default: inputs[2].value
        };

        if (parseFloat(config.min) >= parseFloat(config.max)) {
            alert('La comisión mínima debe ser menor que la máxima');
            return;
        }

        // Aquí iría la llamada AJAX para guardar la configuración
        console.log('Guardando configuración global:', config);
        alert('Configuración global guardada correctamente');
    });
});
