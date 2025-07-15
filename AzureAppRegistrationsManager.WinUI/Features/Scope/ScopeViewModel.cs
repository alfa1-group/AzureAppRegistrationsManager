using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public class ScopeViewModel
{
    public PermissionScope Scope { get; set; } = null!;

    public bool CanEdit { get; set; }
}