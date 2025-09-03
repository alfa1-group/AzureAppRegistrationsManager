using System.Runtime.InteropServices.WindowsRuntime;
using AzureAppRegistrationsManager.WinUI.Services;
using Mapster;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.Role;

public sealed partial class AppRoleUserControl : BaseUserControl
{
    public AppRoleViewModel[] AppRolesSorted
    {
        get
        {
            return AppRegInfo?.Application?.AppRoles?
                .OrderBy(r => r.Value)
                .Select(r => new AppRoleViewModel { AppRole = r, CanEdit = CanEdit })
                .ToArray() ?? [];
        }
    }

    public AppRoleUserControl()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(CanEditProperty, OnCanEditChanged);
    }

    private void OnCanEditChanged(DependencyObject sender, DependencyProperty dp)
    {
        OnPropertyChanged(nameof(AppRolesSorted));
    }

    protected override void OnAppRegInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as AppRoleUserControl)?.OnPropertyChanged(nameof(AppRolesSorted));
    }

    private async void AddAppRole_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo == null)
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
            var app = AppRegInfo?.Application;
            if (app == null)
            {
                return;
            }
            if (app.AppRoles == null)
            {
                app.AppRoles = new List<AppRole>();
            }
            app.AppRoles.Add(dialog.AppRole.Adapt<AppRole>());

            await UpdateAppRegAsync(sender, app.AppRoles, AzureCommandsHandler.UpdateAppRolesAsync);
            OnPropertyChanged(nameof(AppRolesSorted));
        }
    }

    private async void AppRoleAction_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo == null)
        {
            return;
        }

        if (sender is Button button && button.Tag is AppRoleViewModel viewModel)
        {
            var appRole = viewModel.AppRole;
            switch (button.Name)
            {
                case "AppRoleEditButton":
                    {
                        var dialog = new AppRoleDialog(appRole.Adapt<AppRoleEditModel>())
                        {
                            XamlRoot = Content.XamlRoot
                        };
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            dialog.AppRole.Adapt(appRole);

                            await UpdateAppRegAsync(sender, AppRegInfo.Application!.AppRoles!, AzureCommandsHandler.UpdateAppRolesAsync);
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
                            await UpdateAppRegAsync(sender, AppRegInfo.Application!.AppRoles!, async (id, roles) =>
                            {
                                await AzureCommandsHandler.DeleteAppRoleAsync(id, roles, appRole);

                                AppRegInfo.Application = await AzureCommandsHandler.GetApplicationAsync(id);
                            });
                            OnPropertyChanged(nameof(AppRolesSorted));
                        }
                        break;
                    }
            }
        }
    }
}