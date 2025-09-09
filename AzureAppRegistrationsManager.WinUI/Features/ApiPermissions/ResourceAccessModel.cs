namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public class ResourceAccessModel
{
    public required string Type { get; set; }

    public required Guid Id { get; set; }
}