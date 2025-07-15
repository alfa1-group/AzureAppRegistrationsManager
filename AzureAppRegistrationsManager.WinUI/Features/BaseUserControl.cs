using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

        var button = sender as Button;
        button?.IsEnabled = false;

        await func(AppReg.Id, value);

        OnSave?.Invoke(this, EventArgs.Empty);

        button?.IsEnabled = true;
    }
}