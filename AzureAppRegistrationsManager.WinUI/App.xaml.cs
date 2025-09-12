using Azure.Identity;
using Microsoft.Graph;
using Microsoft.UI.Xaml;

namespace AzureAppRegistrationsManager.WinUI;

public partial class App : Application
{
    public static readonly DefaultAzureCredential AzureCredential = new();
    public static readonly GraphServiceClient GraphClient = new(AzureCredential);

    private Window? _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}