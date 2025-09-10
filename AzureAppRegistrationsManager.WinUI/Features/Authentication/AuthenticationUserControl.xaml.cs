using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;

namespace AzureAppRegistrationsManager.WinUI.Features.Authentication;

public sealed partial class AuthenticationUserControl : BaseUserControl
{
    public string WebRedirectUrisText
    {
        get => AppRegInfo?.Application?.WebRedirectUrisText ?? string.Empty;
        set
        {
            if (AppRegInfo != null)
            {
                AppRegInfo?.Application?.WebRedirectUrisText = value;
                OnPropertyChanged(nameof(AppRegInfo));
            }
        }
    }

    public string SpaRedirectUrisText
    {
        get => AppRegInfo?.Application?.SpaRedirectUrisText ?? string.Empty;
        set
        {
            if (AppRegInfo != null)
            {
                AppRegInfo?.Application?.SpaRedirectUrisText = value;
                OnPropertyChanged(nameof(AppRegInfo));
            }
        }
    }

    public AuthenticationUserControl()
    {
        InitializeComponent();
    }

    protected override void OnAppRegInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AuthenticationUserControl control)
        {
            control.OnPropertyChanged(nameof(WebRedirectUrisText));
            control.OnPropertyChanged(nameof(SpaRedirectUrisText));
        }
    }

    private async void SaveImplicitGrantSettings_Click(object sender, RoutedEventArgs e)
    {
        await CallMethodOnAppRegAsync(sender, AppRegInfo?.Application?.Web?.ImplicitGrantSettings, AzureCommandsHandler.UpdateImplicitGrantSettingsAsync);
    }

    private async void SaveWebRedirectUris_Click(object sender, RoutedEventArgs e)
    {
        await CallMethodOnAppRegAsync(sender, AppRegInfo?.Application?.Web?.RedirectUris, AzureCommandsHandler.UpdateWebRedirectUrisAsync);
    }

    private async void SaveSpaRedirectUris_Click(object sender, RoutedEventArgs e)
    {
        await CallMethodOnAppRegAsync(sender, AppRegInfo?.Application?.Spa?.RedirectUris, AzureCommandsHandler.UpdateSpaRedirectUrisAsync);
    }
}