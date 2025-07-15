using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using GraphApplication = Microsoft.Graph.Models.Application;

namespace AzureAppRegistrationsManager.WinUI.Features;

public partial class BaseUserControl : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? OnSave;

    public static readonly DependencyProperty AppRegProperty =
        DependencyProperty.Register(
            nameof(AppReg),
            typeof(GraphApplication),
            typeof(BaseUserControl),
            new PropertyMetadata(null, (d, e) => (d as BaseUserControl)?.OnAppRegChanged(d, e)));

    public GraphApplication? AppReg
    {
        get => (GraphApplication?)GetValue(AppRegProperty);
        set => SetValue(AppRegProperty, value);
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

    protected virtual void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // This method can be overridden by derived classes
    }

    protected async Task UpdateAppRegAsync<T>(object sender, T? value, Func<string, T, Task> func)
    {
        if (AppReg?.Id == null || value == null)
        {
            return;
        }

        if (sender is not Button button)
        {
            return;
        }

        var controlsToDisable = new List<Control> { button };

        if (button.Parent is Panel panel)
        {
            controlsToDisable.AddRange(panel.Children.OfType<TextBox>());
        }

        controlsToDisable.ForEach(c => c.IsEnabled = false);

        try
        {
            await func(AppReg.Id, value);

            OnSave?.Invoke(this, EventArgs.Empty);
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