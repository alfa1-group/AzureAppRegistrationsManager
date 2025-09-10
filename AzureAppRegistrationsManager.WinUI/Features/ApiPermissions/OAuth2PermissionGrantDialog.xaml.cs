using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

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

    //public ScopeDialog(string apiUri, ScopeEditModel existingScope)
    //{
    //    InitializeComponent();

    //    PermissionScope = existingScope;
    //    PermissionScope.ApiUriOriginal = apiUri;

    //    Title = "Edit Scope";
    //}

    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }
}