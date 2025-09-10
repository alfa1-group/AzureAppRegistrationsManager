using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace AzureAppRegistrationsManager.WinUI.Features.Overview;

public sealed partial class OverviewUserControl
{
    protected override void OnAppRegInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OverviewUserControl overviewUserControl)
        {
            overviewUserControl.OnPropertyChanged(nameof(ApplicationIdUri));

            _isEnterpriseApplication = AppRegInfo?.EnterpriseApplication != null;

            overviewUserControl.OnPropertyChanged(nameof(IsEnterpriseApplication));
        }
    }

    public string ApplicationIdUri
    {
        get => AppRegInfo?.Application?.ApplicationIdUri ?? string.Empty;
        set
        {
            if (AppRegInfo != null)
            {
                AppRegInfo?.Application?.ApplicationIdUri = value;
                OnPropertyChanged(nameof(AppRegInfo));
            }
        }
    }

    private bool _isEnterpriseApplication;
    public bool IsEnterpriseApplication
    {
        get => _isEnterpriseApplication;
        set
        {
            _isEnterpriseApplication = value;
            OnPropertyChanged(nameof(AppRegInfo));
        }
    }

    public OverviewUserControl()
    {
        InitializeComponent();
    }

    private async void SaveDisplayName_Click(object sender, RoutedEventArgs e)
    {
        await CallMethodOnAppRegAsync(sender, AppRegInfo?.Application?.DisplayName, AzureCommandsHandler.UpdateDisplayNameAsync);
    }

    private async void SaveApplicationIdUri_Click(object sender, RoutedEventArgs e)
    {
        await CallMethodOnAppRegAsync(sender, AppRegInfo?.Application?.IdentifierUris, AzureCommandsHandler.UpdateIdentifierUrisAsync);
    }

    private async void SaveEnterpriseApplication_Click(object sender, RoutedEventArgs e)
    {
        if (AppRegInfo?.EnterpriseApplication == null && IsEnterpriseApplication)
        {
            var request = new ServicePrincipal
            {
                AppId = AppRegInfo!.AppId,
                DisplayName = AppRegInfo.DisplayName
            };
            AppRegInfo?.EnterpriseApplication = await CallMethodOnAppRegAsync(sender, request, AzureCommandsHandler.ConvertToEnterpriseApplication);
        }

        if (AppRegInfo?.EnterpriseApplication != null && !IsEnterpriseApplication)
        {
            var dialog = new ConfirmationDialog("Are you sure you want to remove the Service Principal for this App Registration?")
            {
                Title = "Remove Service Principal",
                XamlRoot = Content.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Secondary)
            {
                await CallMethodOnAppRegAsync(sender, AppRegInfo?.EnterpriseApplication?.Id, AzureCommandsHandler.RemoveEnterpriseApplication);
                AppRegInfo?.EnterpriseApplication = null;
            }
        }

        _isEnterpriseApplication = AppRegInfo?.EnterpriseApplication != null;
        OnPropertyChanged(nameof(IsEnterpriseApplication));
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var text = button.GetChildTextBoxes().FirstOrDefault()?.Text;
        if (text != null)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);

            Clipboard.SetContent(dataPackage);
        }
    }
}