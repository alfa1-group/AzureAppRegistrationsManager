using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.Owners;

public sealed partial class OwnersUserControl : BaseUserControl
{
    public OwnerViewModel[] OwnersSorted
    {
        get
        {
            var count = AppReg?.Owners?.Count ?? 0;
            return AppReg?.Owners?
                .OrderBy(u => u.Id)
                .Select(u => new OwnerViewModel { DirectoryObject = u, CanDelete = CanEdit && count > 1 })
                .ToArray() ?? [];
        }
    }

    public OwnersUserControl()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(CanEditProperty, OnCanEditChanged);
    }

    private void OnCanEditChanged(DependencyObject sender, DependencyProperty dp)
    {
        OnPropertyChanged(nameof(OwnersSorted));
    }

    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as OwnersUserControl)?.OnPropertyChanged(nameof(OwnersSorted));
    }

    private async void AddOwner_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null)
        {
            return;
        }

        var dialog = new AddOwnerDialog
        {
            XamlRoot = Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await UpdateAppRegAsync(sender, dialog.Owner.Email, async (id, ownerEmail) =>
            {
                await AzureCommandsHandler.AddAppOwnerByEmailAsync(id, ownerEmail);
                AppReg = await AzureCommandsHandler.GetApplicationAsync(id);
            });

            OnPropertyChanged(nameof(OwnersSorted));
        }
    }

    private async void RemoveOwner_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null)
        {
            return;
        }

        if (sender is Button button && button.Tag is OwnerViewModel viewModel)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to remove the owner '{viewModel.Mail}'?")
            {
                Title = "Remove Owner",
                XamlRoot = Content.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                await UpdateAppRegAsync(sender, viewModel.Mail, async (id, ownerEmail) =>
                {
                    await AzureCommandsHandler.RemoveOwnerFromAppRegistrationByEmailAsync(id, ownerEmail);

                    AppReg = await AzureCommandsHandler.GetApplicationAsync(id);
                });

                OnPropertyChanged(nameof(OwnersSorted));
            }
        }
    }
}