using Exceptions;
using System;
using System.Text;

namespace CoreApp
{
    // Helper class para generar códigos QR para solicitudes de pago.
    // Versión simplificada que genera URLs para servicios externos de QR.
    public static class QRCodeHelper
    {
        // Genera una URL de servicio externo para mostrar un código QR de solicitud de pago.
        // paymentRequestCode: Código único de la solicitud de pago
        // size: Tamaño del QR en píxeles (por defecto 300)
        // Devuelve la URL para mostrar el QR como imagen
        public static string GeneratePaymentQR(string paymentRequestCode, int size = 300)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentRequestCode))
                {
                    throw new ArgumentException("El código de solicitud de pago no puede estar vacío");
                }

                // Crear el contenido del QR con formato específico de Yavi
                string qrContent = $"{paymentRequestCode}";

                return GenerateQRCodeUrl(qrContent, size);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw new Exception($"Error al generar código QR: {ex.Message}");
            }
        }

        // Genera una URL de servicio externo para mostrar un código QR genérico.
        // content: Contenido del código QR
        // size: Tamaño en píxeles
        // Devuelve la URL del servicio QR externo
        public static string GenerateQRCodeUrl(string content, int size = 300)
        {
            try
            {
                // Validar parámetros
                if (string.IsNullOrEmpty(content))
                {
                    throw new ArgumentException("El contenido del QR no puede estar vacío");
                }

                if (size < 50 || size > 1000)
                {
                    throw new ArgumentException("El tamaño debe estar entre 50 y 1000 píxeles");
                }

                // Codificar el contenido para URL
                string encodedContent = Uri.EscapeDataString(content);

                // Usar servicio público de QR (Google Charts API alternativa)
                return $"https://api.qrserver.com/v1/create-qr-code/?size={size}x{size}&data={encodedContent}&format=png";
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw new Exception($"Error al generar URL del código QR: {ex.Message}");
            }
        }

        // Genera una imagen QR como base64 usando un placeholder para desarrollo.
        // paymentRequestCode: Código único de la solicitud de pago
        // size: Tamaño del QR
        // Devuelve un base64 de imagen placeholder (actualmente retorna la URL)
        public static string GeneratePaymentQRBase64(string paymentRequestCode, int size = 300)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentRequestCode))
                {
                    throw new ArgumentException("El código de solicitud de pago no puede estar vacío");
                }

                // Por ahora retornamos la URL del servicio externo
                // En una implementación futura se puede reemplazar con generación local
                return GeneratePaymentQR(paymentRequestCode, size);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw new Exception($"Error al generar código QR base64: {ex.Message}");
            }
        }

        // Valida si un string es un código de solicitud de pago válido de Yavi.
        // qrContent: Contenido escaneado del QR
        // Devuelve el código de solicitud si es válido, null si no lo es
        public static string? ExtractPaymentCodeFromQR(string qrContent)
        {
            try
            {
                if (string.IsNullOrEmpty(qrContent))
                {
                    return null;
                }

                // Verificar formato yavi://payment/{code}
                const string yaviPrefix = "yavi://payment/";
                
                if (qrContent.StartsWith(yaviPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string paymentCode = qrContent.Substring(yaviPrefix.Length);
                    
                    // Validar que el código no esté vacío y tenga formato válido
                    if (!string.IsNullOrEmpty(paymentCode) && paymentCode.Length > 5)
                    {
                        return paymentCode;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Genera un QR de prueba para testing y desarrollo.
        // testMessage: Mensaje de prueba
        // size: Tamaño del QR
        // Devuelve la URL del QR de prueba
        public static string GenerateTestQR(string testMessage = "Yavi Test QR", int size = 200)
        {
            return GenerateQRCodeUrl($"yavi://test/{testMessage}", size);
        }

        // Genera la URL para mostrar el QR como imagen en páginas web.
        // paymentRequestCode: Código de la solicitud de pago
        // size: Tamaño del QR
        // Devuelve la URL completa para usar en img src
        public static string GeneratePaymentQRDataUrl(string paymentRequestCode, int size = 300)
        {
            return GeneratePaymentQR(paymentRequestCode, size);
        }

        // Genera el contenido del QR (sin imagen) para casos donde solo se necesita el texto.
        // paymentRequestCode: Código de la solicitud de pago
        // Devuelve el texto del contenido del QR
        public static string GeneratePaymentQRContent(string paymentRequestCode)
        {
            if (string.IsNullOrEmpty(paymentRequestCode))
            {
                throw new ArgumentException("El código de solicitud de pago no puede estar vacío");
            }

            return $"yavi://payment/{paymentRequestCode}";
        }

        // Genera una imagen QR usando un servicio externo y devuelve el HTML img tag.
        // paymentRequestCode: Código de la solicitud de pago
        // size: Tamaño del QR
        // altText: Texto alternativo
        // Devuelve el tag HTML completo
        public static string GeneratePaymentQRImageTag(string paymentRequestCode, int size = 300, string altText = "Código QR de Pago")
        {
            string qrUrl = GeneratePaymentQR(paymentRequestCode, size);
            return $"<img src=\"{qrUrl}\" alt=\"{altText}\" width=\"{size}\" height=\"{size}\" class=\"qr-code\" />";
        }
    }
}