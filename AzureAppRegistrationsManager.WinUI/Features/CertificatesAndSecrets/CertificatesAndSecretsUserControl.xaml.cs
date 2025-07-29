using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public sealed partial class CertificatesAndSecretsUserControl : BaseUserControl
{
    public ClientSecretViewModel[] ClientSecretsSorted
    {
        get
        {
            return AppReg?.PasswordCredentials?
                .OrderBy(p => p.DisplayName)
                .Select(p => new ClientSecretViewModel { PasswordCredential = p, CanEdit = CanEdit })
                .ToArray() ?? [];
        }
    }

    public CertificatesAndSecretsUserControl()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(CanEditProperty, OnCanEditChanged);
    }

    private void OnCanEditChanged(DependencyObject sender, DependencyProperty dp)
    {
        OnPropertyChanged(nameof(ClientSecretsSorted));
    }

    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as CertificatesAndSecretsUserControl)?.OnPropertyChanged(nameof(ClientSecretsSorted));
    }

    private async void AddClientSecret_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null || string.IsNullOrWhiteSpace(AppReg.ApplicationIdUri))
        {
            return;
        }

        //var dialog = new ScopeDialog(AppReg.ApplicationIdUri)
        //{
        //    XamlRoot = Content.XamlRoot
        //};
        //var result = await dialog.ShowAsync();

        //if (result == ContentDialogResult.Primary)
        //{
        //    var scopes = (AppReg.Api ??= new ApiApplication()).Oauth2PermissionScopes ??= [];
        //    scopes.Add(dialog.PermissionScope.Adapt<PermissionScope>());

        //    await UpdateAppRegAsync(sender, scopes, AzureCommandsHandler.UpdateScopesAsync);
        //    OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
        //}
    }

    private async void ClientSecretAction_Click(object sender, RoutedEventArgs e)
    {
        //if (AppReg == null || string.IsNullOrWhiteSpace(AppReg.ApplicationIdUri))
        //{
        //    return;
        //}

        //if (sender is Button button && button.Tag is ScopeViewModel viewModel)
        //{
        //    var scope = viewModel.Scope;
        //    switch (button.Name)
        //    {
        //        case "ScopeEditButton":
        //            {
        //                var dialog = new ScopeDialog(AppReg.ApplicationIdUri, scope.Adapt<ScopeEditModel>())
        //                {
        //                    XamlRoot = Content.XamlRoot
        //                };
        //                var result = await dialog.ShowAsync();

        //                if (result == ContentDialogResult.Primary)
        //                {
        //                    dialog.PermissionScope.Adapt(scope);

        //                    await UpdateAppRegAsync(sender, AppReg.Api!.Oauth2PermissionScopes!, AzureCommandsHandler.UpdateScopesAsync);
        //                    OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
        //                }
        //                break;
        //            }

        //        case "ScopeDeleteButton":
        //            {
        //                var dialog = new ConfirmationDialog($"Are you sure you want to delete the scope '{scope.Value}'?")
        //                {
        //                    Title = "Delete Scope",
        //                    XamlRoot = Content.XamlRoot
        //                };
        //                var result = await dialog.ShowAsync();

        //                if (result == ContentDialogResult.Secondary)
        //                {
        //                    await UpdateAppRegAsync(sender, AppReg.Api!, (id, api) => AzureCommandsHandler.DeleteScopeAsync(id, api, scope));
        //                    OnPropertyChanged(nameof(Oauth2PermissionScopesSorted));
        //                }
        //                break;
        //            }
        //    }
        //}
    }
}