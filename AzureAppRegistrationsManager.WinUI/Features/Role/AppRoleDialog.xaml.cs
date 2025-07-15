using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Role;

internal sealed partial class AppRoleDialog : BaseDialog
{
    public AppRoleDialog()
    {
        AppRole = new AppRole
        {
            Id = Guid.NewGuid(),
            DisplayName = string.Empty,
            Value = string.Empty,
            Description = string.Empty,
            IsEnabled = true,
            AllowedMemberTypes = ["User"]
        };

        Title = "Add Role";
        InitializeComponent();
    }

    public AppRoleDialog(AppRole existingAppRole) : this()
    {
        AppRole = existingAppRole;
        Title = "Edit Role";
    }

    public AppRole AppRole { get; set; }

    public bool IsUsersGroupsSelected
    {
        get => AppRole.AllowedMemberTypes != null &&
               AppRole.AllowedMemberTypes.Contains("User") &&
               !AppRole.AllowedMemberTypes.Contains("Application");
        set
        {
            AppRole.AllowedMemberTypes = ["User"];
        }
    }

    public bool IsApplicationsSelected
    {
        get => AppRole.AllowedMemberTypes != null &&
               AppRole.AllowedMemberTypes.Contains("Application") &&
               !AppRole.AllowedMemberTypes.Contains("User");

        set
        {
            AppRole.AllowedMemberTypes = ["Application"];
        }
    }

    public bool IsBothSelected
    {
        get => AppRole.AllowedMemberTypes != null &&
               AppRole.AllowedMemberTypes.Contains("User") &&
               AppRole.AllowedMemberTypes.Contains("Application");
        set
        {
            AppRole.AllowedMemberTypes = ["User", "Application"];
        }
    }
}