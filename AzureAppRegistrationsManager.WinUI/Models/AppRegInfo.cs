namespace AzureAppRegistrationsManager.WinUI.Models;

public class AppRegInfo
{
    public string AppId { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string ObjectId { get; set; } = null!;

    public bool CanEdit { get; set; }
}