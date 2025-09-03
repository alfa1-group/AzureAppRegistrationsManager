using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace AzureAppRegistrationsManager.WinUI.Features.Overview;

public sealed partial class OverviewUserControl : BaseUserControl
{
    protected override void OnAppRegInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OverviewUserControl overviewUserControl)
        {
            overviewUserControl.OnPropertyChanged(nameof(ApplicationIdUri));

            _isEnterpriseApplication = !string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplicationObjectId);

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
        await UpdateAppRegAsync(sender, AppRegInfo?.Application?.DisplayName, AzureCommandsHandler.UpdateDisplayNameAsync);
    }

    private async void SaveApplicationIdUri_Click(object sender, RoutedEventArgs e)
    {
        await UpdateAppRegAsync(sender, AppRegInfo?.Application?.IdentifierUris, AzureCommandsHandler.UpdateIdentifierUrisAsync);
    }

    private async void SaveEnterpriseApplication_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplicationObjectId) && IsEnterpriseApplication)
        {
            var request = new ServicePrincipal
            {
                AppId = AppRegInfo!.AppId,
                DisplayName = AppRegInfo.DisplayName
            };
            AppRegInfo?.EnterpriseApplicationObjectId = await UpdateAppRegAsync(sender, request, AzureCommandsHandler.ConvertToEnterpriseApplication);
        }

        if (!IsEnterpriseApplication && !string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplicationObjectId))
        {
            await UpdateAppRegAsync(sender, AppRegInfo?.EnterpriseApplicationObjectId, AzureCommandsHandler.RemoveEnterpriseApplication);
            AppRegInfo?.EnterpriseApplicationObjectId = null;
        }

        _isEnterpriseApplication = !string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplicationObjectId);
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