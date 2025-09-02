using AzureAppRegistrationsManager.WinUI.Services;
using Mapster;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public sealed partial class CertificatesAndSecretsUserControl : BaseUserControl
{
    public ClientSecretViewModel[] ClientSecretsSorted
    {
        get
        {
            return AppRegInfo?.Application?.PasswordCredentials?
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

    private async void NewClientSecret_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo == null)
        {
            return;
        }

        var dialog = new ClientSecretDialog
        {
            XamlRoot = Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var passwordCredential = dialog.ClientSecret.Adapt<PasswordCredential>();

            await UpdateAppRegAsync(sender, passwordCredential, async (id, passwordCredential) =>
            {
                var secret = await AzureCommandsHandler.AddClientSecretAsync(id, passwordCredential);
                if (secret != null)
                {
                    var newSecretDialog = new NewSecretDialog(secret)
                    {
                        XamlRoot = Content.XamlRoot
                    };
                    await newSecretDialog.ShowAsync();

                    AppRegInfo.Application = await AzureCommandsHandler.GetApplicationAsync(id);
                }
            });

            OnPropertyChanged(nameof(ClientSecretsSorted));
        }
    }

    private async void ClientSecretDelete_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo == null)
        {
            return;
        }

        if (sender is Button button && button.Tag is ClientSecretViewModel viewModel)
        {
            var passwordCredential = viewModel.PasswordCredential;
            var dialog = new ConfirmationDialog($"Are you sure you want to delete the secret '{passwordCredential.DisplayName}'?")
            {
                Title = "Delete Client Secret",
                XamlRoot = Content.XamlRoot
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Secondary)
            {
                await UpdateAppRegAsync(sender, passwordCredential.KeyId!.Value, async (id, keyId) =>
                {
                    await AzureCommandsHandler.DeleteClientSecretAsync(id, keyId);

                    AppRegInfo.Application = await AzureCommandsHandler.GetApplicationAsync(id);
                });

                OnPropertyChanged(nameof(ClientSecretsSorted));
            }
        }
    }
}