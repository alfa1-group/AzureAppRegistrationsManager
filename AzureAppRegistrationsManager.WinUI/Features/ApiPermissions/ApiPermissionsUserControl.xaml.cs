using AzureAppRegistrationsManager.WinUI.Features.Scope;
using AzureAppRegistrationsManager.WinUI.Services;
using Mapster;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public sealed partial class ApiPermissionsUserControl  : BaseUserControl
{
    public ApiPermissionModel[] ApiPermissionsSorted
    {
        get
        {
            return [];
        }
    }

    public ApiPermissionsUserControl()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(CanEditProperty, OnCanEditChanged);
    }

    private void OnCanEditChanged(DependencyObject sender, DependencyProperty dp)
    {
        OnPropertyChanged(nameof(ApiPermissionsSorted));
    }

    protected override void OnAppRegInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as ApiPermissionsUserControl)?.OnPropertyChanged(nameof(ApiPermissionsSorted));
    }

    //private async void AddScope_Click(object sender, RoutedEventArgs e)
    //{
    //    if (AppRegInfo?.Application == null || string.IsNullOrWhiteSpace(AppRegInfo?.Application?.ApplicationIdUri))
    //    {
    //        return;
    //    }

    //    var dialog = new ScopeDialog(AppRegInfo.Application.ApplicationIdUri)
    //    {
    //        XamlRoot = Content.XamlRoot
    //    };
    //    var result = await dialog.ShowAsync();

    //    if (result == ContentDialogResult.Primary)
    //    {
    //        var api = AppRegInfo.Application.Api;
    //        if (api == null)
    //        {
    //            api = new ApiApplication();
    //            AppRegInfo.Application.Api = api;
    //        }

    //        api.Oauth2PermissionScopes ??= [];
    //        api.Oauth2PermissionScopes.Add(dialog.PermissionScope.Adapt<PermissionScope>());

    //        await UpdateAppRegAsync(sender, api.Oauth2PermissionScopes, AzureCommandsHandler.UpdateScopesAsync);
    //        OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
    //    }
    //}

    //private async void ScopeAction_Click(object sender, RoutedEventArgs e)
    //{
    //    if (AppRegInfo == null || string.IsNullOrWhiteSpace(AppRegInfo?.Application?.ApplicationIdUri))
    //    {
    //        return;
    //    }

    //    if (sender is Button button && button.Tag is ScopeViewModel viewModel)
    //    {
    //        var scope = viewModel.Scope;
    //        switch (button.Name)
    //        {
    //            case "ScopeEditButton":
    //                {
    //                    var dialog = new ScopeDialog(AppRegInfo.Application.ApplicationIdUri, scope.Adapt<ScopeEditModel>())
    //                    {
    //                        XamlRoot = Content.XamlRoot
    //                    };
    //                    var result = await dialog.ShowAsync();

    //                    if (result == ContentDialogResult.Primary)
    //                    {
    //                        dialog.PermissionScope.Adapt(scope);

    //                        await UpdateAppRegAsync(sender, AppRegInfo.Application.Api!.Oauth2PermissionScopes!, AzureCommandsHandler.UpdateScopesAsync);
    //                        OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
    //                    }
    //                    break;
    //                }

    //            case "ScopeDeleteButton":
    //                {
    //                    var dialog = new ConfirmationDialog($"Are you sure you want to delete the scope '{scope.Value}'?")
    //                    {
    //                        Title = "Delete Scope",
    //                        XamlRoot = Content.XamlRoot
    //                    };
    //                    var result = await dialog.ShowAsync();

    //                    if (result == ContentDialogResult.Secondary)
    //                    {
    //                        await UpdateAppRegAsync(sender, AppRegInfo.Application.Api!, (id, api) => AzureCommandsHandler.DeleteScopeAsync(id, api, scope));
    //                        OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //}
}