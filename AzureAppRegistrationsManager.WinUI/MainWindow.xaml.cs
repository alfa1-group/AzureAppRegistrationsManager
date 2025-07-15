using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AzureAppRegistrationsManager.WinUI.Models;
using AzureAppRegistrationsManager.WinUI.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;
using GraphApplication = Microsoft.Graph.Models.Application;

namespace AzureAppRegistrationsManager.WinUI;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    private IReadOnlyList<AppRegInfo>? _appRegInfos;
    public IReadOnlyList<AppRegInfo>? AppRegInfos
    {
        get => _appRegInfos;
        set
        {
            if (value != _appRegInfos)
            {
                _appRegInfos = value;
                OnPropertyChanged();
            }
        }
    }

    private GraphApplication? _application;
    public GraphApplication? AppReg
    {
        get => _application;
        set
        {
            if (value != _application)
            {
                _application = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));

                if (value != null)
                {
                    AppRegJson = JsonSerializer.Serialize(value, MyJsonContext.Default.Application);
                }
            }
        }
    }

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

    private AppRegInfo? _appRegInfo;
    public AppRegInfo? AppRegInfo
    {
        get => _appRegInfo;
        set
        {
            if (value != _appRegInfo)
            {
                _appRegInfo = value;
                OnPropertyChanged();
            }
        }
    }

    public bool CanEdit
    {
        get => AppRegInfo?.CanEdit ?? false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    public MainWindow()
    {
        InitializeComponent();

        var hWnd = WindowNative.GetWindowHandle(this);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(wndId);

        appWindow.SetIcon(@"Assets\icon.ico"); // Set the application icon
        appWindow.MoveInZOrderAtTop(); // Ensure window is topmost in Z-order
        appWindow.SetPresenter(AppWindowPresenterKind.Overlapped); // Ensure window is full screen
        appWindow.Show(); // Brings window to foreground if not already
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool _isFirstActivation = true;
    private async void MainGrid_Loaded(object sender, RoutedEventArgs e)
    {
        if (_isFirstActivation)
        {
            _isFirstActivation = false;
            await RefreshDataAsync();
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        AppRegInfo = null;
        await RefreshAppAsync();

        await RefreshDataAsync();
    }

    private async void ApplicationsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ApplicationsGrid.SelectedItem is AppRegInfo selectedAppRegInfo)
        {
            AppRegInfo = selectedAppRegInfo;
            await RefreshAppAsync();
        }
    }

    private async Task RefreshDataAsync()
    {
        RefreshButton.IsEnabled = false;
        RefreshProgress.IsActive = true;

        ApplicationsGrid.ItemsSource = new[] { new AppRegInfo
        {
            AppId = "loading...",
            ObjectId = "loading...",
            DisplayName = "loading..."
        }};

        AppRegInfos = await AzureCommandsHandler.GetApplicationsAsync();

        RefreshProgress.IsActive = false;
        RefreshButton.IsEnabled = true;
    }

    private async Task RefreshAppAsync()
    {
        if (AppRegInfo == null)
        {
            AppReg = null;
            return;
        }

        RefreshAppProgress.IsActive = true;

        AppRegJson = string.Empty;
        AppReg = await AzureCommandsHandler.GetApplicationAsync(AppRegInfo.ObjectId);

        RefreshAppProgress.IsActive = false;
    }

    private void OnSave(object? sender, EventArgs e)
    {
        // NO-OP for now, but can be used to handle save events
    }
}