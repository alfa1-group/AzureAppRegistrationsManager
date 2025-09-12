using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions.Admin;

public sealed partial class OAuth2PermissionGrantDialog
{
    public OAuth2PermissionGrantEditModel OAuth2PermissionGrant { get; set; }

    public OAuth2PermissionGrantDialog(string servicePrincipalId)
    {
        InitializeComponent();

        OAuth2PermissionGrant = new OAuth2PermissionGrantEditModel
        {
            ClientId = servicePrincipalId
        };

        Title = "Add OAuth2Permission";
    }
    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }
}