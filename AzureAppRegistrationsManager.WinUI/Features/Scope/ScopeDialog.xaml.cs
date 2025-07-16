using Microsoft.UI.Xaml.Controls;
using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public sealed partial class ScopeDialog : BaseDialog
{
    public ScopeEditModel PermissionScope { get; set; }

    public string ApiUri => $"{PermissionScope.ApiUriOriginal}/{ScopeNameTextBox.Text}";

    public ScopeDialog(string apiUri)
    {
        InitializeComponent();

        PermissionScope = new ScopeEditModel
        {
            Id = Guid.NewGuid(),
            ApiUriOriginal = apiUri
        };

        Title = "Add Scope";
    }

    public ScopeDialog(string apiUri, ScopeEditModel existingScope)
    {
        InitializeComponent();

        PermissionScope = existingScope;
        PermissionScope.ApiUriOriginal = apiUri;

        Title = "Edit Scope";
    }

    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }

    private void ScopeNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ApiUri));
    }
}