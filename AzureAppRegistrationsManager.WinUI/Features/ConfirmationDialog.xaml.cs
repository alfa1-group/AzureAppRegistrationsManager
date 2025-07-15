using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features;

public sealed partial class ConfirmationDialog : ContentDialog
{
    public ConfirmationDialog(string message)
    {
        InitializeComponent();
        MessageTextBlock.Text = message;
    }
}