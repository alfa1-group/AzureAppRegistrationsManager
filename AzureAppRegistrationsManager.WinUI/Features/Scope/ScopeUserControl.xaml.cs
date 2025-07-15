using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public sealed partial class ScopeUserControl : BaseUserControl
{
    public PermissionScope[] Oauth2PermissionScopesSorted => AppReg?.Api?.Oauth2PermissionScopes?.OrderBy(s => s.Value).ToArray() ?? [];

    public ScopeUserControl()
    {
        InitializeComponent();
    }

    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // (d as ScopeUserControl)?.OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
    }

    private async void AddScope_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null || string.IsNullOrWhiteSpace(AppReg.ApplicationIdUri))
        {
            return;
        }

        var dialog = new ScopeDialog(AppReg.ApplicationIdUri)
        {
            XamlRoot = Content.XamlRoot
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var scopes = (AppReg.Api ?? new ApiApplication()).Oauth2PermissionScopes ??= [];
            scopes.Add(dialog.PermissionScope);

            await UpdateAppRegAsync(sender, scopes, AzureCommandsHandler.UpdateScopesAsync);
            OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
        }
    }

    private async void ScopeAction_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null || string.IsNullOrWhiteSpace(AppReg.ApplicationIdUri))
        {
            return;
        }

        if (sender is Button button && button.Tag is PermissionScope scope)
        {
            switch (button.Name)
            {
                case "ScopeEditButton":
                    {
                        var dialog = new ScopeDialog(AppReg.ApplicationIdUri, scope)
                        {
                            XamlRoot = Content.XamlRoot
                        };
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            await UpdateAppRegAsync(sender, AppReg.Api!.Oauth2PermissionScopes!, AzureCommandsHandler.UpdateScopesAsync);
                            OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
                        }
                        break;
                    }

                case "ScopeDeleteButton":
                    {
                        var dialog = new ConfirmationDialog($"Are you sure you want to delete the scope '{scope.Value}'?")
                        {
                            Title = "Delete Scope",
                            XamlRoot = Content.XamlRoot
                        };
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Secondary)
                        {
                            await UpdateAppRegAsync(sender, AppReg.Api!, (id, api) => AzureCommandsHandler.DeleteScopeAsync(id, api, scope));
                            OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
                        }
                        break;
                    }
            }
        }
    }
}