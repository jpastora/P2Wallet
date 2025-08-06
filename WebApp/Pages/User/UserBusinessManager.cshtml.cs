using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using CoreApp;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages.User
{
    [Authorize]
    public class UserBusinessManagerModel : PageModel
    {
        public DTOs.User CurrentUser { get; set; } = new();
        public List<DTOs.Merchant> UserMerchants { get; set; } = new();
        public List<DTOs.FinancialEntity> UserFinancialEntities { get; set; } = new();
        public bool HasMerchantAccess { get; set; } = false;
        public bool HasFinancialEntityAccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public void OnGet([FromQuery] string? message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }

            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var userManager = new UserManager();
                CurrentUser = userManager.RetrieveUserByEmail(new DTOs.User { Email = email });
                
                if (CurrentUser != null)
                {
                    LoadUserMerchants();
                    LoadUserFinancialEntities();
                }
            }
        }

        private void LoadUserMerchants()
        {
            try
            {
                // Verificar si el usuario es Admin (tiene acceso a todos los comercios)
                if (CurrentUser.Role == "Admin")
                {
                    HasMerchantAccess = true;
                    var merchantManager = new MerchantManager();
                    UserMerchants = merchantManager.RetrieveAllMerchants()
                        .Where(m => m.ValidationStatus == "Active")
                        .OrderBy(m => m.MerchantName)
                        .ToList();
                }
                else
                {
                    // Verificar comercios asignados al usuario
                    var userMerchantManager = new UserMerchantManager();
                    var userMerchantRelations = userMerchantManager.RetrieveAllUserMerchants()
                        .Where(um => um.UserID == CurrentUser.ID)
                        .ToList();

                    if (userMerchantRelations.Any())
                    {
                        HasMerchantAccess = true;
                        var merchantManager = new MerchantManager();
                        
                        foreach (var relation in userMerchantRelations)
                        {
                            var merchant = merchantManager.RetrieveMerchantById(relation.MerchantID);
                            if (merchant != null && merchant.ValidationStatus == "Active")
                            {
                                UserMerchants.Add(merchant);
                            }
                        }
                        
                        // Ordenar por nombre
                        UserMerchants = UserMerchants.OrderBy(m => m.MerchantName).ToList();
                    }
                }
            }
            catch (System.Exception)
            {
                HasMerchantAccess = false;
                UserMerchants = new List<DTOs.Merchant>();
            }
        }

        private void LoadUserFinancialEntities()
        {
            try
            {
                // Verificar si el usuario es Admin (tiene acceso a todas las entidades)
                if (CurrentUser.Role == "Admin")
                {
                    HasFinancialEntityAccess = true;
                    var financialEntityManager = new FinancialEntityManager();
                    UserFinancialEntities = financialEntityManager.RetrieveAllFinancialEntities()
                        .Where(e => e.ValidationStatus == "Active")
                        .OrderBy(e => e.EntityName)
                        .ToList();
                }
                else
                {
                    // Verificar entidades financieras asignadas al usuario
                    var userEntityManager = new UserEntityManager();
                    var userEntityRelations = userEntityManager.RetrieveAllUserEntities()
                        .Where(ue => ue.UserID == CurrentUser.ID)
                        .ToList();

                    if (userEntityRelations.Any())
                    {
                        HasFinancialEntityAccess = true;
                        var financialEntityManager = new FinancialEntityManager();
                        
                        foreach (var relation in userEntityRelations)
                        {
                            var entity = financialEntityManager.RetrieveFinancialEntityById(relation.FinancialEntityID);
                            if (entity != null && entity.ValidationStatus == "Active")
                            {
                                UserFinancialEntities.Add(entity);
                            }
                        }
                        
                        // Ordenar por nombre
                        UserFinancialEntities = UserFinancialEntities.OrderBy(e => e.EntityName).ToList();
                    }
                }
            }
            catch (System.Exception)
            {
                HasFinancialEntityAccess = false;
                UserFinancialEntities = new List<DTOs.FinancialEntity>();
            }
        }

        // Métodos auxiliares para obtener información adicional
        public string GetMerchantStatusBadgeClass(string status)
        {
            return status switch
            {
                "Active" => "success",
                "Inactive" => "secondary",
                "Pending" => "warning",
                "Rejected" => "danger",
                _ => "secondary"
            };
        }

        public string GetMerchantStatusText(string status)
        {
            return status switch
            {
                "Active" => "Activo",
                "Inactive" => "Inactivo",
                "Pending" => "Pendiente",
                "Rejected" => "Rechazado",
                _ => status
            };
        }

        public string GetEntityStatusBadgeClass(string status)
        {
            return status switch
            {
                "Active" => "success",
                "Inactive" => "secondary",
                "Pending" => "warning",
                "Rejected" => "danger",
                _ => "secondary"
            };
        }

        public string GetEntityStatusText(string status)
        {
            return status switch
            {
                "Active" => "Activo",
                "Inactive" => "Inactivo",
                "Pending" => "Pendiente",
                "Rejected" => "Rechazado",
                _ => status
            };
        }
    }
}