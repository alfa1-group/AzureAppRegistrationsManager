using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AzureAppRegistrationsManager.WinUI.Features;

internal partial class BaseDialog : ContentDialog, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isValid = false;
    protected bool IsValid
    {
        get => _isValid;
        set
        {
            var newValue = value;
            if (newValue != _isValid)
            {
                _isValid = newValue;
                OnPropertyChanged();
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var thisTextBoxIsValid = ValidateTextBox(textBox);
    }

    protected void ValidateTextBoxes(object sender)
    {
        var valids = (sender as Panel)?
            .Children.OfType<Panel>()
            .SelectMany(p => p.Children.OfType<TextBox>())
            .Select(ValidateTextBox);

        if (valids != null)
        {
            IsValid = valids.All(v => v);
        }
        else
        {
            IsValid = true;
        }
    }

    protected virtual bool ValidateTextBox(TextBox textBox)
    {
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            textBox.BorderThickness = new Thickness(1);
            return false;
        }

        textBox.ClearValue(BorderBrushProperty);
        textBox.ClearValue(BorderThicknessProperty);
        return true;
    }
}