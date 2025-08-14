using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreApp;
using DTOs;

namespace WebApp.Pages.Directory
{
    public class BusinessDirectoryModel : PageModel
    {
        public List<BusinessInfo> AllBusinesses { get; set; } = new();
        public List<BusinessInfo> FilteredBusinesses { get; set; } = new();
        public string Message { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; } = "all"; // all, merchant, financial

        [BindProperty(SupportsGet = true)]
        public string CategoryFilter { get; set; } = "all";

        public List<string> Categories { get; set; } = new();

        public void OnGet()
        {
            LoadBusinesses();
            ApplyFilters();
        }

        private void LoadBusinesses()
        {
            AllBusinesses = new List<BusinessInfo>();

            try
            {
                // Cargar comercios
                var merchantManager = new MerchantManager();
                var merchants = merchantManager.RetrieveAllMerchants()
                    .Where(m => m.ValidationStatus == "Active")
                    .ToList();

                foreach (var merchant in merchants)
                {
                    AllBusinesses.Add(new BusinessInfo
                    {
                        ID = merchant.ID,
                        Name = merchant.MerchantName ?? "Sin nombre",
                        Email = merchant.Email ?? "",
                        Phone = merchant.ContactPhone ?? "",
                        TaxID = merchant.TaxID ?? "",
                        Commission = merchant.CommissionPercentage,
                        Type = "Comercio",
                        Category = DetermineCategory(merchant.MerchantName ?? ""),
                        ImageUrl = !string.IsNullOrEmpty(merchant.LogoImage) ? merchant.LogoImage : $"https://picsum.photos/seed/{merchant.ID}/400/200",
                        DetailUrl = $"/Merchant/MerchantDetail?merchantId={merchant.ID}",
                        Status = merchant.ValidationStatus ?? "Inactive"
                    });
                }

                // Cargar entidades financieras
                var entityManager = new FinancialEntityManager();
                var entities = entityManager.RetrieveAllFinancialEntities()
                    .Where(e => e.ValidationStatus == "Active")
                    .ToList();

                foreach (var entity in entities)
                {
                    AllBusinesses.Add(new BusinessInfo
                    {
                        ID = entity.ID,
                        Name = entity.EntityName ?? "Sin nombre",
                        Email = entity.Email ?? "",
                        Phone = entity.ContactPhone ?? "",
                        TaxID = entity.TaxID ?? "",
                        Commission = entity.CommissionPercentage,
                        Type = "Entidad Financiera",
                        Category = DetermineFinancialCategory(entity.EntityName ?? ""),
                        ImageUrl = !string.IsNullOrEmpty(entity.LogoImage) ? entity.LogoImage : $"https://picsum.photos/seed/bank{entity.ID}/400/200",
                        DetailUrl = $"/FinancialEntity/FinancialEntityDetail?entityId={entity.ID}",
                        Status = entity.ValidationStatus ?? "Inactive"
                    });
                }

                // Obtener categorías únicas
                Categories = AllBusinesses
                    .Select(b => b.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }
            catch (Exception ex)
            {
                Message = $"Error al cargar negocios: {ex.Message}";
                AllBusinesses = new List<BusinessInfo>();
            }
        }

        private void ApplyFilters()
        {
            FilteredBusinesses = AllBusinesses.ToList();

            // Filtro por tipo
            if (!string.IsNullOrEmpty(TypeFilter) && TypeFilter != "all")
            {
                if (TypeFilter == "merchant")
                    FilteredBusinesses = FilteredBusinesses.Where(b => b.Type == "Comercio").ToList();
                else if (TypeFilter == "financial")
                    FilteredBusinesses = FilteredBusinesses.Where(b => b.Type == "Entidad Financiera").ToList();
            }

            // Filtro por categoría
            if (!string.IsNullOrEmpty(CategoryFilter) && CategoryFilter != "all")
            {
                FilteredBusinesses = FilteredBusinesses.Where(b => b.Category == CategoryFilter).ToList();
            }

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                FilteredBusinesses = FilteredBusinesses.Where(b =>
                    b.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Category.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Ordenar por nombre
            FilteredBusinesses = FilteredBusinesses.OrderBy(b => b.Name).ToList();
        }

        private string DetermineCategory(string name)
        {
            var lowerName = name.ToLower();

            if (lowerName.Contains("super") || lowerName.Contains("market") || lowerName.Contains("tienda"))
                return "Supermercado";
            else if (lowerName.Contains("café") || lowerName.Contains("coffee") || lowerName.Contains("cafetería"))
                return "Cafetería";
            else if (lowerName.Contains("restaurante") || lowerName.Contains("comida") || lowerName.Contains("food"))
                return "Restaurante";
            else if (lowerName.Contains("farmacia") || lowerName.Contains("salud") || lowerName.Contains("medicina"))
                return "Farmacia";
            else if (lowerName.Contains("ropa") || lowerName.Contains("moda") || lowerName.Contains("fashion"))
                return "Moda";
            else if (lowerName.Contains("tecnología") || lowerName.Contains("electrónica") || lowerName.Contains("tech"))
                return "Tecnología";
            else
                return "General";
        }

        private string DetermineFinancialCategory(string name)
        {
            var lowerName = name.ToLower();

            if (lowerName.Contains("banco") || lowerName.Contains("bank"))
                return "Banco";
            else if (lowerName.Contains("cooperativa") || lowerName.Contains("coop"))
                return "Cooperativa";
            else if (lowerName.Contains("mutualista") || lowerName.Contains("mutual"))
                return "Mutualista";
            else if (lowerName.Contains("popular") || lowerName.Contains("ahorro"))
                return "Banco Popular";
            else
                return "Financiera";
        }
    }

    public class BusinessInfo
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string TaxID { get; set; } = string.Empty;
        public double Commission { get; set; }
        public string Type { get; set; } = string.Empty; // "Comercio" o "Entidad Financiera"
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string DetailUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}