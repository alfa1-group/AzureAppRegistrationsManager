using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public sealed partial class RequestResourceAccessDialog
{
    public RequestResourceAccessModel RequestResourceAccess { get; set; }

    public RequestResourceAccessDialog()
    {
        InitializeComponent();

        RequestResourceAccess = new RequestResourceAccessModel();

        Title = "Request Resource Access";
    }
    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }
}