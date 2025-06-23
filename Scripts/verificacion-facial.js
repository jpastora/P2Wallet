document.addEventListener('DOMContentLoaded', () => {
    const initBtn = document.getElementById('initCaptureButton');
    const takeBtn = document.getElementById('takePhotoButton');
    const submitBtn = document.getElementById('submitButton');
    const recaptureBtn = document.getElementById('recaptureButton');

    const video = document.getElementById('cameraFeed');
    const canvas = document.getElementById('canvas');
    const img = document.getElementById('photoPreview');
    const feedback = document.getElementById('feedbackText');

    let stream;

    // Iniciar cámara
    initBtn.addEventListener('click', async () => {
        try {
            stream = await navigator.mediaDevices.getUserMedia({ video: true });
            video.srcObject = stream;
            video.classList.remove('d-none');
            takeBtn.classList.remove('d-none');
            initBtn.classList.add('d-none');
            feedback.textContent = '';
        } catch (err) {
            feedback.textContent = 'No se pudo acceder a la cámara.';
        }
    });

    // Tomar foto
    takeBtn.addEventListener('click', () => {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        const ctx = canvas.getContext('2d');
        ctx.drawImage(video, 0, 0);
        const imageDataUrl = canvas.toDataURL('image/jpeg');
        img.src = imageDataUrl;
        img.classList.remove('d-none');
        video.classList.add('d-none');

        takeBtn.classList.add('d-none');
        submitBtn.classList.remove('d-none');
        recaptureBtn.classList.remove('d-none');
    });

    // Reintentar
    recaptureBtn.addEventListener('click', () => {
        img.classList.add('d-none');
        video.classList.remove('d-none');

        submitBtn.classList.add('d-none');
        recaptureBtn.classList.add('d-none');
        takeBtn.classList.remove('d-none');
        feedback.textContent = '';
    });

    // Enviar selfie
    submitBtn.addEventListener('click', () => {
        feedback.textContent = 'Procesando...';
        submitBtn.disabled = true;
        setTimeout(() => {
            feedback.textContent = '¡Verificación Exitosa!';
            stream.getTracks().forEach(track => track.stop());
            // window.location.href = '/dashboard.html';
        }, 2500);
    });
});
