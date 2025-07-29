using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public sealed partial class NewSecretDialog : ContentDialog
{
    public NewSecretDialog(string secret)
    {
        InitializeComponent();
        MessageTextBlock.Text = secret;
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(MessageTextBlock.Text);

        Clipboard.SetContent(dataPackage);
    }
}