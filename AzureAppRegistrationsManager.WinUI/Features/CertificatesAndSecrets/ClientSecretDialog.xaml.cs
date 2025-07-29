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
            DisplayName = string.Empty
        };

        Title = "Add a client secret";
        InitializeComponent();
    }

    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }

    private void ExpirationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = ExpirationComboBox.SelectedItem as ComboBoxItem;
        if (selected == null)
        {
            return;
        }

        var tag = selected.Tag?.ToString();
        if (int.TryParse(tag, out int days))
        {
            ClientSecret.EndDateTime = DateTimeOffset.UtcNow.AddDays(days);
        }
    }
}