# Imagen base con Python
FROM python:3.10-slim

# Establece el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copia los archivos necesarios
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copia tu aplicación
COPY app.py .

# Expone el puerto que usará Flask
EXPOSE 5000

# Comando de inicio para producción (usa gunicorn)
CMD ["gunicorn", "-w", "4", "-b", "0.0.0.0:5000", "app:app"]