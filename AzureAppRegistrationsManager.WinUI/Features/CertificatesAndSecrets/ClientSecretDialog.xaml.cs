using Microsoft.UI.Xaml.Controls;
using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

internal sealed partial class ClientSecretDialog : ContentDialog
{
    public ClientSecretAddModel ClientSecret { get; set; }

    public ClientSecretDialog()
    {
        ClientSecret = new ClientSecretAddModel
        {
            DisplayName = string.Empty,
            EndDateTime = DateTimeOffset.UtcNow.AddYears(1)
        };

        Title = "Add a client secret";
        InitializeComponent();
    }

    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }
}