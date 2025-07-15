using System.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AzureAppRegistrationsManager.WinUI.Features;

internal partial class BaseDialog : ContentDialog, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            textBox.BorderThickness = new Thickness(1);
        }
        else
        {
            textBox.ClearValue(BorderBrushProperty);
            textBox.ClearValue(BorderThicknessProperty);
        }
    }
}