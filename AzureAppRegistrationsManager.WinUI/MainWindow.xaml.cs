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

namespace AzureAppRegistrationsManager.WinUI;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    private const string Loading = "loading...";

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
                OnPropertyChanged(nameof(CanEdit));
            }
        }
    }

    public bool CanEdit => AppRegInfo?.CanEdit ?? false;

    private string? _currentLoadedType;
    public string? CurrentLoadedType
    {
        get => _currentLoadedType;
        set
        {
            if (value != _currentLoadedType)
            {
                _currentLoadedType = value;
                OnPropertyChanged();
            }
        }
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

        appWindow.SetPresenter(AppWindowPresenterKind.Overlapped); // Ensure window is overlapped
        (appWindow.Presenter as OverlappedPresenter)?.Maximize(); // Try to maximize the window
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
            await RefreshAppRegInfosAsync(false);
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAppAsync(null);

        await RefreshAppRegInfosAsync(false);
    }

    private async void RefreshAllButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAppAsync(null);

        await RefreshAppRegInfosAsync(true);
    }

    private async void ApplicationsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ApplicationsGrid.SelectedItem is AppRegInfo selectedAppRegInfo)
        {
            await RefreshAppAsync(selectedAppRegInfo);
        }
    }

    private async Task RefreshAppRegInfosAsync(bool all)
    {
        CurrentLoadedType = all ? "All Applications" : "Own Applications";

        RefreshButton.IsEnabled = false;
        RefreshAllButton.IsEnabled = false;
        RefreshProgress.IsActive = true;

        ApplicationsGrid.ItemsSource = new[] { new AppRegInfo
        {
            AppId = Loading,
            ObjectId = Loading,
            DisplayName = Loading
        }};

        AppRegInfos = all ? await AzureCommandsHandler.GetAllApplicationsAsync() : await AzureCommandsHandler.GetOwnApplicationsAsync();

        RefreshProgress.IsActive = false;
        RefreshAllButton.IsEnabled = true;
        RefreshButton.IsEnabled = true;
    }

    private async Task RefreshAppAsync(AppRegInfo? selectedAppRegInfo)
    {
        if (selectedAppRegInfo == null)
        {
            AppRegInfo = null;
            return;
        }

        RefreshAppProgress.IsActive = true;

        selectedAppRegInfo.Application = null;
        selectedAppRegInfo.ApplicationAsJson = string.Empty;
        selectedAppRegInfo.Application = await AzureCommandsHandler.GetApplicationAsync(selectedAppRegInfo.ObjectId);
        if (selectedAppRegInfo.Application != null)
        {
            selectedAppRegInfo.ApplicationAsJson = JsonSerializer.Serialize(selectedAppRegInfo.Application, MyJsonContext.Default.Application);
        }

        RefreshAppProgress.IsActive = false;

        AppRegInfo = selectedAppRegInfo;
    }

    private void OnSave(object? sender, EventArgs e)
    {
        // NO-OP for now, but can be used to handle save events
    }
}