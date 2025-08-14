using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreApp;
using DTOs;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // Propiedades para datos dinámicos de la página
        public List<ServicioInfo> Servicios { get; set; } = new();
        public List<PromocionInfo> Promociones { get; set; } = new();
        public List<ComercioInfo> ComerciosAfiliados { get; set; } = new();
        public List<EntidadFinancieraInfo> EntidadesFinancieras { get; set; } = new();
        public List<PreguntaFrecuenteInfo> PreguntasFrecuentes { get; set; } = new();

        public void OnGet()
        {
            try
            {
                // Inicializar datos para la página de inicio
                InicializarServicios();
                InicializarPromociones();
                InicializarComerciosAfiliados();
                InicializarEntidadesFinancieras();
                InicializarPreguntasFrecuentes();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los datos de la página de inicio");
                // En caso de error, inicializar listas vacías para evitar errores en la vista
                Servicios = new List<ServicioInfo>();
                Promociones = new List<PromocionInfo>();
                ComerciosAfiliados = new List<ComercioInfo>();
                EntidadesFinancieras = new List<EntidadFinancieraInfo>();
                PreguntasFrecuentes = new List<PreguntaFrecuenteInfo>();
            }
        }

        private void InicializarServicios()
        {
            // Los servicios se mantienen como datos estáticos ya que son características del sistema
            Servicios = new List<ServicioInfo>
            {
                new ServicioInfo
                {
                    Titulo = "Pagos y Cobros",
                    Descripcion = "Transacciones seguras al instante.",
                    Icono = "bi-credit-card"
                },
                new ServicioInfo
                {
                    Titulo = "Promociones",
                    Descripcion = "Ofertas exclusivas para usuarios Yavi.",
                    Icono = "bi-percent"
                },
                new ServicioInfo
                {
                    Titulo = "Integración Bancaria",
                    Descripcion = "Conecta tus cuentas en un clic.",
                    Icono = "bi-bank"
                }
            };
        }

        private void InicializarPromociones()
        {
            try
            {
                var merchantPromotionManager = new MerchantPromotionManager();
                var financialPromotionManager = new FinancialPromotionManager();
                var merchantManager = new MerchantManager();
                var financialEntityManager = new FinancialEntityManager();

                Promociones = new List<PromocionInfo>();

                // Obtener promociones activas de comercios
                var merchantPromotions = merchantPromotionManager.RetrieveAllPromotions()
                    .Where(p => p.ValidationStatus == "Active" && p.EndDate > DateTime.Now)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(6) // Mostrar solo las 6 más recientes
                    .ToList();

                var merchants = merchantManager.RetrieveAllMerchants().ToDictionary(m => m.ID, m => m.MerchantName);

                foreach (var promo in merchantPromotions)
                {
                    var merchantName = merchants.ContainsKey(promo.MerchantID) ? merchants[promo.MerchantID] : "Comercio";
                    
                    Promociones.Add(new PromocionInfo
                    {
                        Titulo = promo.MerchantPromotionName,
                        Descripcion = promo.MerchantPromotionDescription,
                        ImagenUrl = !string.IsNullOrEmpty(promo.MerchantPromotionImage) 
                            ? promo.MerchantPromotionImage 
                            : $"https://picsum.photos/seed/{promo.ID}/400/200",
                        FechaVencimiento = promo.EndDate,
                        TipoDescuento = promo.PromotionType,
                        PorcentajeDescuento = (decimal)promo.DiscountPercentage,
                        ComercioNombre = merchantName
                    });
                }

                // Obtener promociones activas de entidades financieras
                var financialPromotions = financialPromotionManager.RetrieveAllPromotions()
                    .Where(p => p.ValidationStatus == "Active" && p.EndDate > DateTime.Now)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(3) // Complementar con promociones financieras
                    .ToList();

                var entities = financialEntityManager.RetrieveAllFinancialEntities().ToDictionary(e => e.ID, e => e.EntityName);

                foreach (var promo in financialPromotions)
                {
                    if (Promociones.Count >= 6) break; // Limitar a 6 promociones totales

                    var entityName = entities.ContainsKey(promo.FinancialEntityID) ? entities[promo.FinancialEntityID] : "Entidad Financiera";
                    
                    Promociones.Add(new PromocionInfo
                    {
                        Titulo = promo.FinancialPromotionName,
                        Descripcion = promo.FinancialPromotionDescription,
                        ImagenUrl = !string.IsNullOrEmpty(promo.FinancialPromotionImage) 
                            ? promo.FinancialPromotionImage 
                            : $"https://picsum.photos/seed/fin{promo.ID}/400/200",
                        FechaVencimiento = promo.EndDate,
                        TipoDescuento = promo.PromotionType,
                        PorcentajeDescuento = (decimal)promo.DiscountPercentage,
                        ComercioNombre = entityName
                    });
                }

                // Si no hay promociones activas, mostrar mensaje por defecto
                if (!Promociones.Any())
                {
                    Promociones.Add(new PromocionInfo
                    {
                        Titulo = "Próximamente nuevas promociones",
                        Descripcion = "Estamos trabajando para traerte las mejores ofertas exclusivas.",
                        ImagenUrl = "https://picsum.photos/seed/coming-soon/400/200",
                        FechaVencimiento = DateTime.Now.AddDays(30),
                        TipoDescuento = "Información",
                        PorcentajeDescuento = 0,
                        ComercioNombre = "Yavi"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar promociones desde la base de datos");
                // En caso de error, mostrar promociones de fallback
                InicializarPromocionesFallback();
            }
        }

        private void InicializarComerciosAfiliados()
        {
            try
            {
                var merchantManager = new MerchantManager();
                var comercios = merchantManager.RetrieveAllMerchants()
                    .Where(m => m.ValidationStatus == "Active")
                    .Take(6)
                    .ToList();

                ComerciosAfiliados = comercios.Select(m => new ComercioInfo
                {
                    ID = m.ID,
                    Nombre = m.MerchantName ?? "Comercio sin nombre",
                    Descripcion = "Este comercio acepta pagos con Yavi y te ofrece una experiencia digital segura.",
                    ImagenUrl = !string.IsNullOrEmpty(m.LogoImage) ? m.LogoImage : $"https://picsum.photos/seed/{m.ID}/400/200",
                    Categoria = DeterminarCategoriaComercio(m.MerchantName ?? ""),
                    Telefono = m.ContactPhone ?? "",
                    Email = m.Email ?? ""
                }).ToList();

                // Si no hay comercios activos, mostrar mensaje por defecto
                if (!ComerciosAfiliados.Any())
                {
                    ComerciosAfiliados.Add(new ComercioInfo
                    {
                        Nombre = "Próximamente más comercios",
                        Descripcion = "Estamos incorporando más comercios aliados para brindarte mejores beneficios.",
                        ImagenUrl = "https://picsum.photos/seed/merchants-coming/400/200",
                        Categoria = "Información"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar comercios desde la base de datos");
                // En caso de error, mostrar comercios de fallback
                InicializarComerciosFallback();
            }
        }

        private void InicializarEntidadesFinancieras()
        {
            try
            {
                var financialEntityManager = new FinancialEntityManager();
                var entidades = financialEntityManager.RetrieveAllFinancialEntities()
                    .Where(e => e.ValidationStatus == "Active")
                    .Take(6) // Mostrar solo 6 entidades destacadas
                    .ToList();

                EntidadesFinancieras = entidades.Select(e => new EntidadFinancieraInfo
                {
                    ID = e.ID,
                    Nombre = e.EntityName ?? "Entidad sin nombre",
                    Descripcion = "Entidad financiera aliada que ofrece servicios seguros con Yavi.",
                    ImagenUrl = !string.IsNullOrEmpty(e.LogoImage) ? e.LogoImage : $"https://picsum.photos/seed/bank{e.ID}/400/200",
                    Telefono = e.ContactPhone ?? "",
                    Email = e.Email ?? ""
                }).ToList();

                // Si no hay entidades activas, mostrar mensaje por defecto
                if (!EntidadesFinancieras.Any())
                {
                    EntidadesFinancieras.Add(new EntidadFinancieraInfo
                    {
                        Nombre = "Próximamente más entidades",
                        Descripcion = "Estamos estableciendo alianzas con más entidades financieras para mejorar tu experiencia.",
                        ImagenUrl = "https://picsum.photos/seed/banks-coming/400/200"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar entidades financieras desde la base de datos");
                // En caso de error, mostrar entidades de fallback
                InicializarEntidadesFallback();
            }
        }

        private void InicializarPreguntasFrecuentes()
        {
            // Las preguntas frecuentes se mantienen como datos estáticos ya que son información general del sistema
            PreguntasFrecuentes = new List<PreguntaFrecuenteInfo>
            {
                new PreguntaFrecuenteInfo
                {
                    Pregunta = "¿Qué necesito para registrarme en Yavi?",
                    Respuesta = "Solo necesitás un correo electrónico válido, una selfie y tus datos personales básicos para comenzar."
                },
                new PreguntaFrecuenteInfo
                {
                    Pregunta = "¿Yavi cobra alguna comisión?",
                    Respuesta = "El uso de Yavi es gratuito para los usuarios. Solo algunos comercios o entidades pueden aplicar comisiones según sus políticas."
                },
                new PreguntaFrecuenteInfo
                {
                    Pregunta = "¿Dónde puedo usar Yavi?",
                    Respuesta = "En los comercios afiliados y plataformas que acepten pagos digitales a través de Yavi."
                },
                new PreguntaFrecuenteInfo
                {
                    Pregunta = "¿Es seguro usar Yavi?",
                    Respuesta = "Sí, Yavi utiliza tecnología de encriptación avanzada y cumple con los más altos estándares de seguridad financiera."
                },
                new PreguntaFrecuenteInfo
                {
                    Pregunta = "¿Puedo usar Yavi sin conexión a internet?",
                    Respuesta = "No, Yavi requiere conexión a internet para procesar las transacciones y garantizar la seguridad de las operaciones."
                }
            };
        }

        // Métodos auxiliares de fallback en caso de error
        private void InicializarPromocionesFallback()
        {
            Promociones = new List<PromocionInfo>
            {
                new PromocionInfo
                {
                    Titulo = "Promociones disponibles próximamente",
                    Descripcion = "Estamos preparando ofertas exclusivas para ti. ¡Mantente atento!",
                    ImagenUrl = "https://picsum.photos/seed/promo-fallback/400/200",
                    FechaVencimiento = DateTime.Now.AddDays(30),
                    TipoDescuento = "Información",
                    PorcentajeDescuento = 0,
                    ComercioNombre = "Yavi"
                }
            };
        }

        private void InicializarComerciosFallback()
        {
            ComerciosAfiliados = new List<ComercioInfo>
            {
                new ComercioInfo
                {
                    Nombre = "Comercios aliados próximamente",
                    Descripcion = "Estamos incorporando comercios para ofrecerte más beneficios.",
                    ImagenUrl = "https://picsum.photos/seed/merchant-fallback/400/200",
                    Categoria = "Información"
                }
            };
        }

        private void InicializarEntidadesFallback()
        {
            EntidadesFinancieras = new List<EntidadFinancieraInfo>
            {
                new EntidadFinancieraInfo
                {
                    Nombre = "Entidades financieras próximamente",
                    Descripcion = "Estamos estableciendo alianzas con entidades financieras.",
                    ImagenUrl = "https://picsum.photos/seed/bank-fallback/400/200"
                }
            };
        }

        private string DeterminarCategoriaComercio(string nombreComercio)
        {
            // Lógica simple para determinar categoría basada en el nombre
            var nombre = nombreComercio.ToLower();
            
            if (nombre.Contains("super") || nombre.Contains("market") || nombre.Contains("tienda"))
                return "Supermercado";
            else if (nombre.Contains("café") || nombre.Contains("coffee") || nombre.Contains("cafetería"))
                return "Cafetería";
            else if (nombre.Contains("restaurante") || nombre.Contains("comida") || nombre.Contains("food"))
                return "Restaurante";
            else if (nombre.Contains("farmacia") || nombre.Contains("salud") || nombre.Contains("medicina"))
                return "Farmacia";
            else if (nombre.Contains("ropa") || nombre.Contains("moda") || nombre.Contains("fashion"))
                return "Ropa";
            else if (nombre.Contains("tecnología") || nombre.Contains("electrónica") || nombre.Contains("tech"))
                return "Tecnología";
            else
                return "Comercio General";
        }
    }

    // Clases auxiliares para la información (actualizadas con nuevos campos)
    public class ServicioInfo
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
    }

    public class PromocionInfo
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public string TipoDescuento { get; set; } = string.Empty;
        public decimal PorcentajeDescuento { get; set; }
        public string ComercioNombre { get; set; } = string.Empty;
    }

    public class ComercioInfo
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class EntidadFinancieraInfo
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class PreguntaFrecuenteInfo
    {
        public string Pregunta { get; set; } = string.Empty;
        public string Respuesta { get; set; } = string.Empty;
    }
}
