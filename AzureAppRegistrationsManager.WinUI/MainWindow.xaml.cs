using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AzureAppRegistrationsManager.WinUI.Models;
using AzureAppRegistrationsManager.WinUI.Services;
using CommunityToolkit.WinUI.UI.Controls;
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
            if (!Equals(value, _appRegInfos))
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

    // Track sorting state to toggle ascending/descending
    private DataGridColumn? _sortedColumn;
    private DataGridSortDirection _sortDirection = DataGridSortDirection.Ascending;

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

        selectedAppRegInfo.ApiPermissionModels = null;
        selectedAppRegInfo.Application = null;
        selectedAppRegInfo.ApplicationAsJson = string.Empty;

        var applicationTask = Task.Run(async () =>
        {
            var application = await AzureCommandsHandler.GetApplicationAsync(selectedAppRegInfo.ObjectId);
            var applicationAsJson = application != null ? JsonSerializer.Serialize(application, MyJsonContext.Default.Application) : string.Empty;

            return (application, applicationAsJson);
        });

        var apiPermissionModelsTask = AzureCommandsHandler.GetPermissionsAsync(selectedAppRegInfo.EnterpriseApplication?.Id);

        await Task.WhenAll(applicationTask, apiPermissionModelsTask);

        selectedAppRegInfo.Application = (await applicationTask).application;
        selectedAppRegInfo.ApplicationAsJson = (await applicationTask).applicationAsJson;
        selectedAppRegInfo.ApiPermissionModels = await apiPermissionModelsTask;

        RefreshAppProgress.IsActive = false;

        AppRegInfo = selectedAppRegInfo;
    }

    private void dataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        if (AppRegInfos == null)
        {
            return;
        }

        // Toggle direction if same column clicked; otherwise reset to ascending
        if (_sortedColumn == e.Column)
        {
            _sortDirection = _sortDirection == DataGridSortDirection.Ascending ? DataGridSortDirection.Descending : DataGridSortDirection.Ascending;
        }
        else
        {
            if (_sortedColumn != null && _sortedColumn != e.Column)
            {
                _sortedColumn.SortDirection = null;
            }

            _sortDirection = DataGridSortDirection.Ascending;
            _sortedColumn = e.Column;
        }

        e.Column.SortDirection = _sortDirection;

        var source = AppRegInfos;
        IEnumerable<AppRegInfo> sorted = e.Column.Tag switch
        {
            nameof(AppRegInfo.AppId) => _sortDirection == DataGridSortDirection.Ascending
                ? source.OrderBy(a => a.AppId)
                : source.OrderByDescending(a => a.AppId),

            nameof(AppRegInfo.ObjectId) => _sortDirection == DataGridSortDirection.Ascending
                ? source.OrderBy(a => a.ObjectId)
                : source.OrderByDescending(a => a.ObjectId),

            "Enterprise Application Object ID" => _sortDirection == DataGridSortDirection.Ascending
                ? source.OrderBy(a => a.EnterpriseApplication?.Id ?? string.Empty)
                : source.OrderByDescending(a => a.EnterpriseApplication?.Id ?? string.Empty),

            nameof(AppRegInfo.DisplayName) => _sortDirection == DataGridSortDirection.Ascending
                ? source.OrderBy(a => a.DisplayName)
                : source.OrderByDescending(a => a.DisplayName),

            nameof(AppRegInfo.CanEdit) => _sortDirection == DataGridSortDirection.Ascending
                ? source.OrderBy(a => a.CanEdit)
                : source.OrderByDescending(a => a.CanEdit),

            _ => source
        };

        AppRegInfos = sorted.ToList();
    }

    private void OnSave(object? sender, EventArgs e)
    {
        // NO-OP for now, but can be used to handle save events
    }
}