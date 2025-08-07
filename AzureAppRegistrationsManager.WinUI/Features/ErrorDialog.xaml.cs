using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features;

public sealed partial class ErrorDialog : ContentDialog
{
    public ErrorDialog(string message)
    {
        InitializeComponent();
        MessageTextBlock.Text = message;
    }
}