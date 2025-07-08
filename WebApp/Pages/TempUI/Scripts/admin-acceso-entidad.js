
    document.addEventListener('DOMContentLoaded', function() {
            const loginForm = document.getElementById('loginForm');
    const statusMessage = document.getElementById('statusMessage');
    const statusModal = new bootstrap.Modal(document.getElementById('statusModal'));

        loginForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;

            try {
                const response = await fetch('https://localhost:5001/api/Auth/login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, password })
                });

                if (!response.ok) {
                    throw new Error("Credenciales inválidas.");
                }

                const entidad = await response.json();

                if (entidad.estado === "approved") {
                    statusMessage.className = "alert alert-success";
                    statusMessage.innerHTML = `Bienvenido, ${entidad.nombre}. Redirigiendo...`;
                    statusMessage.classList.remove('d-none');

                    setTimeout(() => {
                        window.location.href = "dashboard-entidad.html";
                    }, 1500);
                } else {
                    const modalContent = document.getElementById('modalContent');
                    let statusClass = entidad.estado === "pending" ? "status-pending" : "status-rejected";
                    let statusText = entidad.estado === "pending" ? "Pendiente" : "Rechazada";

                    modalContent.innerHTML = `
                <div class="text-center mb-4">
                    <span class="status-badge ${statusClass}">${statusText}</span>
                </div>
                <p>Hola, <strong>${entidad.nombre}</strong>.</p>
                <p>${entidad.mensaje}</p>
                ${entidad.estado === "pending" ? '<div class="alert alert-info mt-3">Tiempo estimado de revisión: 3-5 días hábiles</div>' : ''}
            `;
                    statusModal.show();
                }
            } catch (err) {
                statusMessage.className = "alert alert-danger";
                statusMessage.innerHTML = err.message;
                statusMessage.classList.remove('d-none');
            }
        });


    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

                // Simular validación con los datos de prueba
                const entidad = entidadesRegistradas.find(ent => ent.email === email && ent.password === password);

    if (entidad) {
                    // Mostrar mensaje según estado
                    if (entidad.estado === "approved") {
        // Redirigir al dashboard (simulado)
        statusMessage.className = "alert alert-success";
    statusMessage.innerHTML = `Bienvenido, ${entidad.nombre}. Redirigiendo...`;
    statusMessage.classList.remove('d-none');

                        // Simular redirección
                        setTimeout(() => {
        window.location.href = "dashboard-entidad.html";
                        }, 1500);
                    } else {
                        // Mostrar modal con estado de la solicitud
                        const modalContent = document.getElementById('modalContent');
    let statusClass, statusText;

    if (entidad.estado === "pending") {
        statusClass = "status-pending";
    statusText = "Pendiente";
                        } else {
        statusClass = "status-rejected";
    statusText = "Rechazada";
                        }

    modalContent.innerHTML = `
    <div class="text-center mb-4">
        <span class="status-badge ${statusClass}">${statusText}</span>
    </div>
    <p>Hola, <strong>${entidad.nombre}</strong>.</p>
    <p>${entidad.mensaje}</p>
    ${entidad.estado === "pending" ? '<div class="alert alert-info mt-3">Tiempo estimado de revisión: 3-5 días hábiles</div>' : ''}
    `;

    statusModal.show();
                    }
                } else {
        // Credenciales incorrectas
        statusMessage.className = "alert alert-danger";
    statusMessage.innerHTML = "Correo electrónico o contraseña incorrectos. Intenta nuevamente.";
    statusMessage.classList.remove('d-none');
                }
            });

    // Limpiar mensajes al cambiar los campos
    document.getElementById('email').addEventListener('input', function() {
        statusMessage.classList.add('d-none');
            });

    document.getElementById('password').addEventListener('input', function() {
        statusMessage.classList.add('d-none');
            });
        });
