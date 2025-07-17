using Microsoft.UI.Xaml.Controls;
using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.Role;

internal sealed partial class AppRoleDialog : ContentDialog
{
    public AppRoleEditModel AppRole { get; set; }

    public AppRoleDialog()
    {
        AppRole = new AppRoleEditModel
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

    public AppRoleDialog(AppRoleEditModel existingAppRole) : this()
    {
        AppRole = existingAppRole;
        Title = "Edit Role";
    }

    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }
}