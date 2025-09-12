namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public class ApiPermissionModel
{
    public string ApplicationName { get; set; } = null!;

    public string Scope { get; set; } = null!;

    public string ConsentType { get; set; } = null!;
}