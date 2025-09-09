namespace AzureAppRegistrationsManager.WinUI.Models;

internal class ApiPermissionModel
{
    public required string ApplicationName { get; set; }

    public required IReadOnlyList<ResourceAccessModel> ResourceAccesses { get; set; }
}