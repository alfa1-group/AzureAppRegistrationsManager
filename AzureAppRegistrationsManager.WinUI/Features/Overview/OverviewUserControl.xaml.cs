using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;

namespace AzureAppRegistrationsManager.WinUI.Features.Overview;

public sealed partial class OverviewUserControl : BaseUserControl
{
    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as OverviewUserControl)?.OnPropertyChanged(nameof(ApplicationIdUri));
    }

    public string ApplicationIdUri
    {
        get => AppReg?.ApplicationIdUri ?? string.Empty;
        set
        {
            if (AppReg != null)
            {
                AppReg.ApplicationIdUri = value;
                OnPropertyChanged(nameof(AppReg));
            }
        }
    }

    public OverviewUserControl()
    {
        InitializeComponent();
    }

    private async void SaveDisplayName_Click(object sender, RoutedEventArgs e)
    {
        await UpdateAppRegAsync(sender, AppReg?.DisplayName, AzureCommandsHandler.UpdateDisplayNameAsync);
    }

    private async void SaveApplicationIdUri_Click(object sender, RoutedEventArgs e)
    {
        await UpdateAppRegAsync(sender, AppReg?.IdentifierUris, AzureCommandsHandler.UpdateIdentifierUrisAsync);
    }
}