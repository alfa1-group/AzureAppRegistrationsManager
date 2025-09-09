namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public class ApiPermissionModel
{
    public required string ApplicationName { get; set; }

    public required IReadOnlyList<ResourceAccessModel> ResourceAccesses { get; set; }
}