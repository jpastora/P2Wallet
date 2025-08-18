using System;
using System.IO;

namespace Exceptions
{
    public static class ExceptionLogger
    {
        private static readonly string logFilePath = Path.Combine(
            Environment.GetEnvironmentVariable("HOME") ?? AppDomain.CurrentDomain.BaseDirectory,
            "LogFiles",
            "ExceptionLog.txt"
        );

        public static void LogException(Exception ex)
        {
            try
            {
                // Crear carpeta si no existe
                var directory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Formato del log
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType()}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";

                // Escribir en archivo
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Evitar que un error en el logger tumbe la app
            }
        }
    }
}
