using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;

namespace AzureAppRegistrationsManager.WinUI.Features.Authentication;

public sealed partial class AuthenticationUserControl : BaseUserControl
{
    public string WebRedirectUrisText
    {
        get => AppReg?.WebRedirectUrisText ?? string.Empty;
        set
        {
            if (AppReg != null)
            {
                AppReg.WebRedirectUrisText = value;
                OnPropertyChanged(nameof(AppReg));
            }
        }
    }

    public string SpaRedirectUrisText
    {
        get => AppReg?.SpaRedirectUrisText ?? string.Empty;
        set
        {
            if (AppReg != null)
            {
                AppReg.SpaRedirectUrisText = value;
                OnPropertyChanged(nameof(AppReg));
            }
        }
    }

    public AuthenticationUserControl()
    {
        InitializeComponent();
    }

    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AuthenticationUserControl control)
        {
            control.OnPropertyChanged(nameof(WebRedirectUrisText));
            control.OnPropertyChanged(nameof(SpaRedirectUrisText));
        }
    }

    private async void SaveImplicitGrantSettings_Click(object sender, RoutedEventArgs e)
    {
        await UpdateAppRegAsync(sender, AppReg?.Web?.ImplicitGrantSettings, AzureCommandsHandler.UpdateImplicitGrantSettingsAsync);
    }

    private async void SaveWebRedirectUris_Click(object sender, RoutedEventArgs e)
    {
        await UpdateAppRegAsync(sender, AppReg?.Web?.RedirectUris, AzureCommandsHandler.UpdateWebRedirectUrisAsync);
    }

    private async void SaveSpaRedirectUris_Click(object sender, RoutedEventArgs e)
    {
        await UpdateAppRegAsync(sender, AppReg?.Spa?.RedirectUris, AzureCommandsHandler.UpdateSpaRedirectUrisAsync);
    }
}