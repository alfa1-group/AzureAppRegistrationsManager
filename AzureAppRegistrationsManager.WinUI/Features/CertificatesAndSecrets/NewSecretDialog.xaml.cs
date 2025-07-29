using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public sealed partial class NewSecretDialog : ContentDialog
{
    public NewSecretDialog(string secret)
    {
        InitializeComponent();
        MessageTextBlock.Text = secret;
    }
}