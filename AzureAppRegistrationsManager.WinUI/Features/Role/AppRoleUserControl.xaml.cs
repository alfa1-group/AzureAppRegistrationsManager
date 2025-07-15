using System.Runtime.InteropServices.WindowsRuntime;
using AzureAppRegistrationsManager.WinUI.Features.Scope;
using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.Role;

public sealed partial class AppRoleUserControl : BaseUserControl
{
    public AppRole[]? AppRolesSorted
    {
        get => AppReg?.AppRoles?.OrderBy(r => r.Value).ToArray() ?? [];
    }


    public AppRoleUserControl()
    {
        InitializeComponent();
    }

    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // (d as AppRoleUserControl)?.OnPropertyChanged(nameof(AppRolesSorted));
    }

    private async void AddAppRole_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null)
        {
            return;
        }

        var dialog = new AppRoleDialog
        {
            XamlRoot = Content.XamlRoot
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var roles = AppReg.AppRoles ??= [];
            roles.Add(dialog.AppRole);

            await UpdateAppRegAsync(sender, roles, AzureCommandsHandler.UpdateAppRolesAsync);
            OnPropertyChanged(nameof(AppRolesSorted));
        }
    }

    private async void AppRoleAction_Click(object sender, RoutedEventArgs e)
    {
        if (AppReg == null)
        {
            return;
        }

        if (sender is Button button && button.Tag is AppRole appRole)
        {
            switch (button.Name)
            {
                case "AppRoleEditButton":
                    {
                        var dialog = new AppRoleDialog(appRole)
                        {
                            XamlRoot = Content.XamlRoot
                        };
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            await UpdateAppRegAsync(sender, AppReg.AppRoles, AzureCommandsHandler.UpdateAppRolesAsync);
                            OnPropertyChanged(nameof(AppRolesSorted));
                        }
                        break;
                    }

                case "AppRoleDeleteButton":
                    {
                        var dialog = new ConfirmationDialog($"Are you sure you want to delete the AppRole '{appRole.Value}'?")
                        {
                            Title = "Delete Role",
                            XamlRoot = Content.XamlRoot
                        };
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Secondary)
                        {
                            await UpdateAppRegAsync(sender, AppReg.AppRoles!, (id, roles) => AzureCommandsHandler.DeleteAppRoleAsync(id, roles, appRole));
                            OnPropertyChanged(nameof(AppRolesSorted));
                        }
                        break;
                    }
            }
        }
    }
}