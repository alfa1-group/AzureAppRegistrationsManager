using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

internal sealed partial class ClientSecretDialog : ContentDialog, INotifyPropertyChanged
{
    public ClientSecretAddModel ClientSecret { get; set; }

    private int _customDays = 180;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int CustomDays
    {
        get => _customDays;
        set
        {
            _customDays = value;
            OnPropertyChanged(nameof(CustomDays));
            if (_customDays > 0)
            {
                ClientSecret.EndDateTime = DateTimeOffset.UtcNow.AddDays(_customDays);
            }
        }
    }

    public ClientSecretDialog()
    {
        ClientSecret = new ClientSecretAddModel
        {
            DisplayName = string.Empty
        };

        Title = "Add a client secret";
        InitializeComponent();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        if (tag == null)
        {
            return;
        }

        if (tag == "Custom")
        {
            CustomDaysStackPanel.Visibility = Visibility.Visible;
        }
        else
        {
            CustomDaysStackPanel?.Visibility = Visibility.Collapsed;
            if (int.TryParse(tag, out var days))
            {
                ClientSecret.EndDateTime = DateTimeOffset.UtcNow.AddDays(days);
            }
        }
    }
}