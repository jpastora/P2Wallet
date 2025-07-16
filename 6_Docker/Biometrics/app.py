from flask import Flask, request, jsonify
from deepface import DeepFace
import cv2
import numpy as np
from io import BytesIO

app = Flask(__name__)

def resize_image(img, max_size=600):
    h, w = img.shape[:2]
    if max(h, w) <= max_size:
        return img
    scale = max_size / max(h, w)
    new_w = int(w * scale)
    new_h = int(h * scale)
    return cv2.resize(img, (new_w, new_h))

def extract_face_from_id(img_id):
    face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + "haarcascade_frontalface_default.xml")
    gray = cv2.cvtColor(img_id, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(gray, scaleFactor=1.1, minNeighbors=5)

    if len(faces) == 0:
        return None  # No face found

    (x, y, w, h) = faces[0]
    return img_id[y:y+h, x:x+w]

@app.route('/verify', methods=['POST'])
def verify_faces():
    if 'selfie' not in request.files or 'id_image' not in request.files:
        return jsonify({'error': 'Se requieren los archivos "selfie" e "id_image"'}), 400

    try:
        selfie_file = request.files['selfie']
        id_file = request.files['id_image']

        selfie_bytes = np.frombuffer(selfie_file.read(), np.uint8)
        id_bytes = np.frombuffer(id_file.read(), np.uint8)

        selfie_img = cv2.imdecode(selfie_bytes, cv2.IMREAD_COLOR)
        id_img = cv2.imdecode(id_bytes, cv2.IMREAD_COLOR)

        # Recortar rostro del ID
        id_face_img = extract_face_from_id(id_img)
        if id_face_img is None:
            return jsonify({'verified': False, 'message': 'No se detectó rostro en la imagen del ID'}), 200

        result = DeepFace.verify(img1_path=selfie_img, img2_path=id_face_img, enforce_detection=False)

        return jsonify({
            'verified': result['verified'],
            'distance': result['distance'],
            'threshold': result['threshold'],
            'model': result['model']
        })

    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True)
