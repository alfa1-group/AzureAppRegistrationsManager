using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public class ClientSecretViewModel
{
    public PermissionScope Scope { get; set; } = null!;

    public bool CanEdit { get; set; }
}