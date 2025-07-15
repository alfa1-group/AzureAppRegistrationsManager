using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Role;

public class AppRoleViewModel
{
    public AppRole AppRole { get; set; } = null!;

    public bool CanEdit { get; set; }
}