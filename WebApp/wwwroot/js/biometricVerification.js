const constraints = {
    selfie: { video: { facingMode: "user" } },
    front: { video: { facingMode: { exact: "environment" } } },
    back: { video: { facingMode: { exact: "environment" } } }
};

const videoElements = {
    selfie: document.getElementById('selfieVideo'),
    front: document.getElementById('frontIdVideo'),
    back: document.getElementById('backIdVideo')
};

const canvasElements = {
    selfie: document.getElementById('selfieCanvas'),
    front: document.getElementById('frontIdCanvas'),
    back: document.getElementById('backIdCanvas')
};

const inputElements = {
    selfie: document.getElementById('selfieInput'),
    front: document.getElementById('frontIdInput'),
    back: document.getElementById('backIdInput')
};

async function startCamera(type) {
    try {
        const stream = await navigator.mediaDevices.getUserMedia(constraints[type]);
        videoElements[type].srcObject = stream;
    } catch (error) {
        console.error(`Error con cámara ${type}:`, error);
        alert('No se pudo acceder a la cámara. Intenta desde un celular.');
    }
}

function takePhoto(type) {
    const video = videoElements[type];
    const canvas = canvasElements[type];
    const ctx = canvas.getContext('2d');

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    const dataUrl = canvas.toDataURL('image/jpeg');
    inputElements[type].value = dataUrl;

    canvas.classList.remove('d-none');
    video.classList.add('d-none');

    document.getElementById(`btnTake${capitalize(type)}`).classList.add('d-none');
    document.getElementById(`btnConfirm${capitalize(type)}`).classList.remove('d-none');
    document.getElementById(`btnRetake${capitalize(type)}`).classList.remove('d-none');
}

function confirmPhoto(type) {
    stopCamera(type);

    if (type === 'selfie') {
        document.getElementById('selfieSection').classList.add('d-none');
        document.getElementById('frontIdSection').classList.remove('d-none');
        startCamera('front');
    } else if (type === 'front') {
        document.getElementById('frontIdSection').classList.add('d-none');
        document.getElementById('backIdSection').classList.remove('d-none');
        startCamera('back');
    } else if (type === 'back') {
        alert('¡Capturas completadas!');
    }
}

function retakePhoto(type) {
    const canvas = canvasElements[type];
    const video = videoElements[type];

    canvas.classList.add('d-none');
    video.classList.remove('d-none');

    document.getElementById(`btnTake${capitalize(type)}`).classList.remove('d-none');
    document.getElementById(`btnConfirm${capitalize(type)}`).classList.add('d-none');
    document.getElementById(`btnRetake${capitalize(type)}`).classList.add('d-none');

    inputElements[type].value = '';
}

function stopCamera(type) {
    const stream = videoElements[type].srcObject;
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
        videoElements[type].srcObject = null;
    }
}

function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

window.addEventListener('load', () => {
    document.getElementById('selfieSection').classList.remove('d-none');
    document.getElementById('frontIdSection').classList.add('d-none');
    document.getElementById('backIdSection').classList.add('d-none');
    startCamera('selfie');
});

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('verifyForm');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const selfie = inputElements.selfie.value;
        const front = inputElements.front.value;
        const back = inputElements.back.value;

        const resultDiv = document.getElementById('result');
        resultDiv.textContent = 'Procesando...';

        if (!selfie || !front || !back) {
            resultDiv.innerHTML = `<strong>Error:</strong> Faltan imágenes por capturar.`;
            return;
        }

        const formData = new FormData();
        formData.append('selfie', dataURLtoBlob(selfie), 'selfie.jpg');
        formData.append('id_front', dataURLtoBlob(front), 'id_front.jpg');
        formData.append('id_back', dataURLtoBlob(back), 'id_back.jpg');

        try {
            const response = await fetch('https://ad3affdc8ec6.ngrok-free.app/verify', {
                method: 'POST',
                body: formData
            });

            const data = await response.json();

            if (!response.ok) {
                resultDiv.innerHTML = `<strong>Error:</strong> ${data.error}`;
            } else {
                resultDiv.innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
            }
        } catch (error) {
            resultDiv.innerHTML = `<strong>Error de red:</strong> ${error.message}`;
        }
    });
});

/**
 * Convierte un dataURL base64 en un objeto Blob
 */
function dataURLtoBlob(dataurl) {
    const arr = dataurl.split(',');
    const mimeMatch = arr[0].match(/:(.*?);/);
    const mime = mimeMatch ? mimeMatch[1] : '';
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new Blob([u8arr], { type: mime });
}
