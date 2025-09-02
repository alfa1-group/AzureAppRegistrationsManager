using AzureAppRegistrationsManager.WinUI.Models;
using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace AzureAppRegistrationsManager.WinUI.Features.Overview;

public sealed partial class OverviewUserControl : BaseUserControl
{
    protected override void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is OverviewUserControl overviewUserControl)
        {
            overviewUserControl.OnPropertyChanged(nameof(ApplicationIdUri));
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
        get => _isEnterpriseApplication; //string.IsNullOrEmpty(AppRegInfo?.EnterpriseApplicationObjectId);
        set
        {
            _isEnterpriseApplication = value;
            //if (AppRegInfo != null)
            //{
            //    AppRegInfo.WebRedirectUrisText = value;
                OnPropertyChanged(nameof(AppRegInfo));
            //}
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