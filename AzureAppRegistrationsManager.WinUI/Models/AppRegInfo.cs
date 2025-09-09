using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Models;

public class AppRegInfo
{
    public string AppId { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string ObjectId { get; set; } = null!;

    public ServicePrincipal? EnterpriseApplication { get; set; }

    public bool CanEdit { get; set; }

    public Application? Application { get; set; }

    public string ApplicationAsJson { get; set; } = string.Empty;
}