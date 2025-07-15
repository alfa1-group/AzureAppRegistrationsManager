using System.Text.Json;
using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.UI.Xaml;
using GraphApplication = Microsoft.Graph.Models.Application;

namespace AzureAppRegistrationsManager.WinUI.Features.Manifest;

public sealed partial class ManifestUserControl : BaseUserControl
{
    private string _appRegJson = string.Empty;
    public string AppRegJson
    {
        get => _appRegJson;
        set
        {
            var newValue = value ?? string.Empty;
            if (newValue != _appRegJson)
            {
                _appRegJson = newValue;
                OnPropertyChanged();
            }
        }
    }

    public ManifestUserControl()
    {
        InitializeComponent();
    }

    protected override async void OnAppRegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ManifestUserControl control)
        {
            if (e.NewValue is GraphApplication appReg)
            {
                AppRegJson = JsonSerializer.Serialize(appReg, MyJsonContext.Default.Application);
            }
            else
            {
                AppRegJson = string.Empty;
            }

            await UpdateMonacoAppRegJsonAsync();
        }
    }

    private async void MonacoAppRegJson_MonacoEditorLoaded(object? sender, EventArgs e)
    {
        MonacoAppRegJson.MonacoEditorLoaded -= MonacoAppRegJson_MonacoEditorLoaded;

        await LoadMonacoAppRegJsonContentAsync();
    }

    private async Task UpdateMonacoAppRegJsonAsync()
    {
        // Check if Monaco Editor is ready
        if (MonacoAppRegJson.IsLoaded)
        {
            await LoadMonacoAppRegJsonContentAsync();
        }
        else
        {
            MonacoAppRegJson.MonacoEditorLoaded += MonacoAppRegJson_MonacoEditorLoaded;
        }
    }

    private async Task LoadMonacoAppRegJsonContentAsync()
    {
        MonacoAppRegJson.ScrollToLine(0);
        await MonacoAppRegJson.LoadContentAsync(AppRegJson);
    }
}