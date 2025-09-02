using System.ComponentModel;
using System.Runtime.CompilerServices;
using AzureAppRegistrationsManager.WinUI.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AzureAppRegistrationsManager.WinUI.Features;

public partial class BaseUserControl : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? OnSave;

    public static readonly DependencyProperty AppRegInfoProperty =
        DependencyProperty.Register(
            nameof(AppRegInfo),
            typeof(AppRegInfo),
            typeof(BaseUserControl),
            new PropertyMetadata(null, (d, e) => (d as BaseUserControl)?.OnAppRegInfoChanged(d, e)));

    public AppRegInfo? AppRegInfo
    {
        get => (AppRegInfo?)GetValue(AppRegInfoProperty);
        set => SetValue(AppRegInfoProperty, value);
    }

    public static readonly DependencyProperty CanEditProperty =
        DependencyProperty.Register(
            nameof(CanEdit),
            typeof(bool),
            typeof(BaseUserControl),
            new PropertyMetadata(null, null));

    public bool CanEdit
    {
        get => (bool)GetValue(CanEditProperty);
        set => SetValue(CanEditProperty, value);
    }

    public bool Not(bool value) => !value;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnAppRegInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // This method can be overridden by derived classes
    }

    protected async Task UpdateAppRegAsync<T>(object sender, T? value, Func<string, T, Task> func)
    {
        await UpdateAppRegAsync(sender, value, async (id, val) =>
        {
            await func(id, val);

            return true;
        });
    }

    protected async Task<TResult?> UpdateAppRegAsync<T, TResult>(object sender, T? value, Func<string, T, Task<TResult>> func)
    {
        if (AppRegInfo?.Application?.Id == null || value == null)
        {
            return default;
        }

        if (sender is not Button button)
        {
            return default;
        }

        var controlsToDisable = new List<Control> { button };
        controlsToDisable.AddRange(button.GetChildControls());

        controlsToDisable.ForEach(c => c.IsEnabled = false);

        try
        {
            var result = await func(AppRegInfo.Application.Id, value);

            OnSave?.Invoke(this, EventArgs.Empty);

            return result;
        }
        catch (Exception ex)
        {
            await new ErrorDialog(ex.Message)
            {
                XamlRoot = Content.XamlRoot
            }.ShowAsync();

            return default;
        }
        finally
        {
            controlsToDisable.ForEach(c => c.IsEnabled = true);
        }
    }

    protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var buttons = (textBox.Parent is Panel panel) ? panel.Children.OfType<Button>().ToList() : [];

        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            textBox.BorderThickness = new Thickness(1);

            buttons.ForEach(c => c.IsEnabled = false);
        }
        else
        {
            textBox.ClearValue(BorderBrushProperty);
            textBox.ClearValue(BorderThicknessProperty);

            buttons.ForEach(c => c.IsEnabled = true);
        }
    }
}