using AzureAppRegistrationsManager.WinUI.Extensions;
using AzureAppRegistrationsManager.WinUI.Services;
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

    private async void RequestResourceAccess_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo?.Application == null || string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplication?.Id))
        {
            return;
        }

        var dialog = new RequestResourceAccessDialog
        {
            XamlRoot = Content.XamlRoot
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            var scopes = dialog.RequestResourceAccess.Scopes.SplitToList();
            await RequestResourceAccessHandler.RequestAsync(this, AppRegInfo, scopes);

            OnPropertyChanged(nameof(ApiPermissionsSorted));
        }
    }
}