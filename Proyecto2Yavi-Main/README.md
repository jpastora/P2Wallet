🧱 1. Estructura del Proyecto
Nombrado del proyecto: Usar PascalCase, sin espacios ni guiones. Ej: MiProyectoERP.


Separación por capas (si aplica):


MiProyecto.Domain


MiProyecto.Application


MiProyecto.Infrastructure


MiProyecto.API


MiProyecto.Tests



🔠 2. Convenciones de Nomenclatura

Ser explícitos en los nombres de variables y tenga sentido a la funcionalidad todo en idioma inglés. 

Elemento
Estilo
Ejemplo
Clases
PascalCase
ClienteService
Interfaces
I + PascalCase
IClienteRepository
Métodos
PascalCase
CalcularMontoTotal()
Variables locales
camelCase
montoFinal
Campos privados
_camelCase
_logger
Constantes
PascalCase
MaximoIntentos
Archivos
Igual a la clase que contienen
ClienteService.cs


🧼 3. Estilo de Código
Llaves siempre en nueva línea:

 csharp
CopyEdit
if (condicion)
{
    // código
}

Usar var para tipos evidentes, tipo explícito si no lo es:

 csharp
CopyEdit
var cliente = new Cliente(); // correcto
int edad = 30;               // correcto

Espaciado consistente:


1 línea entre métodos


No espacios innecesarios antes de paréntesis: if (condicion) (no if( condicion ))


Evitar abreviaturas. Es preferible cliente en lugar de cli.








🧪 4. Pruebas Unitarias
Framework recomendado: xUnit o NUnit


Nombres descriptivos: Metodo_Escenario_ResultadoEsperado

 csharp
CopyEdit
[Fact]
public void CalcularMontoTotal_ConDescuento_RetornaMontoCorrecto()

Separar tests por clase en archivos separados


Usar mocks para dependencias (ej. Moq)



🔐 5. Manejo de Errores
Nunca atrapar Exception de forma genérica sin al menos hacer un log.

 csharp
CopyEdit
try
{
    // código
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error en el proceso.");
    throw;
}

Utilizar excepciones específicas cuando sea posible.




🧰6. Uso de Dependencias
Usar inyección de dependencias (por constructor).


Evitar servicios estáticos o singletons sin control.



📄 7. Documentación
Documentar métodos públicos con ///, especialmente si son parte de una API o librería.

 csharp
CopyEdit
/// <summary>
/// Calcula el total del pedido con impuestos.
/// </summary>
public decimal CalcularTotal() { ... }


📦 8. Reglas de Commit y Git
Mensajes claros y en imperativo:


✅ Agrega validación de correo electrónico *Se lo dejamos a copilot


❌ Agregando validación


Commits pequeños y enfocados


Usar ramas con nombres estándar:


feature/registro-clientes


bugfix/validacion-email




✅ 9. Revisión y Validación
Activar análisis de código (Code Analysis).


Configurar reglas de StyleCop o EditorConfig.


Revisiones de código obligatorias para pull requests.
