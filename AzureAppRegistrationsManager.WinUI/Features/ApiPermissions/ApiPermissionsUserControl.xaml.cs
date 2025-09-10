using AzureAppRegistrationsManager.WinUI.Services;
using Mapster;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public sealed partial class ApiPermissionsUserControl
{
    public ApiPermissionModel[] ApiPermissionsSorted =>
        AppRegInfo?.ApiPermissionModels?
            .OrderBy(p => p.ConsentType)
            .ThenBy(p => p.ApplicationName)
            .ThenBy(p => p.Scope)
            .ToArray() ?? [];

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
        if (d is ApiPermissionsUserControl apiPermissionsUserControl)
        {
            apiPermissionsUserControl.OnPropertyChanged(nameof(ApiPermissionsSorted));
        }
    }

    private async void AddPermission_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo?.Application == null || string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplication?.Id))
        {
            return;
        }

        var dialog = new OAuth2PermissionGrantDialog(AppRegInfo.EnterpriseApplication.Id)
        {
            XamlRoot = Content.XamlRoot
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            var permissionGrant = dialog.OAuth2PermissionGrant.Adapt<OAuth2PermissionGrant>();

            await UpdateAppRegAsync(sender, permissionGrant, AzureCommandsHandler.AddDelegatedApiPermissionGrantAsync);
            OnPropertyChanged(nameof(ApiPermissionsSorted));
        }
    }

    //private async void ApiPermissionAction_Click(object sender, RoutedEventArgs e)
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