using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public sealed partial class NewSecretDialog : ContentDialog
{
    public NewSecretDialog(string secret)
    {
        InitializeComponent();
        SecretTextBox.Text = secret;
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(SecretTextBox.Text);

        Clipboard.SetContent(dataPackage);
    }
}