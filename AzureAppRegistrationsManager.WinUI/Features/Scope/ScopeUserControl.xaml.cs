using AzureAppRegistrationsManager.WinUI.Services;
using Mapster;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public sealed partial class ScopeUserControl : BaseUserControl
{
    public ScopeViewModel[] Oauth2PermissionScopesSorted
    {
        get
        {
            return AppRegInfo?.Application?.Api?.Oauth2PermissionScopes?
                .OrderBy(s => s.Value)
                .Select(s => new ScopeViewModel { Scope = s, CanEdit = CanEdit })
                .ToArray() ?? [];
        }
    }

    public ScopeUserControl()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(CanEditProperty, OnCanEditChanged);
    }

    private void OnCanEditChanged(DependencyObject sender, DependencyProperty dp)
    {
        OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
    }

    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as ScopeUserControl)?.OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
    }

    private async void AddScope_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo == null || string.IsNullOrWhiteSpace(AppRegInfo?.Application?.ApplicationIdUri))
        {
            return;
        }

        var dialog = new ScopeDialog(AppRegInfo.Application.ApplicationIdUri)
        {
            XamlRoot = Content.XamlRoot
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var api = AppRegInfo?.Application?.Api ?? new ApiApplication();
            var scopes = api.Oauth2PermissionScopes ?? [];
            scopes.Add(dialog.PermissionScope.Adapt<PermissionScope>());

            await UpdateAppRegAsync(sender, scopes, AzureCommandsHandler.UpdateScopesAsync);
            OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
        }
    }

    private async void ScopeAction_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo == null || string.IsNullOrWhiteSpace(AppRegInfo?.Application?.ApplicationIdUri))
        {
            return;
        }

        if (sender is Button button && button.Tag is ScopeViewModel viewModel)
        {
            var scope = viewModel.Scope;
            switch (button.Name)
            {
                case "ScopeEditButton":
                    {
                        var dialog = new ScopeDialog(AppRegInfo.Application.ApplicationIdUri, scope.Adapt<ScopeEditModel>())
                        {
                            XamlRoot = Content.XamlRoot
                        };
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            dialog.PermissionScope.Adapt(scope);

                            await UpdateAppRegAsync(sender, AppRegInfo.Application.Api!.Oauth2PermissionScopes!, AzureCommandsHandler.UpdateScopesAsync);
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
                            await UpdateAppRegAsync(sender, AppRegInfo.Application.Api!, (id, api) => AzureCommandsHandler.DeleteScopeAsync(id, api, scope));
                            OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
                        }
                        break;
                    }
            }
        }
    }
}