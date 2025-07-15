using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Kiota.Authentication.Azure;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzureAppRegistrationsManager.WinUI;

public partial class App : Application
{
    public static readonly GraphServiceClient GraphClient = new(new DefaultAzureCredential());
    // public static readonly GraphServiceClient GraphClient = Create();

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

    private static GraphServiceClient Create()
    {
        // 1. Create your credential as before.
        var credential = new DefaultAzureCredential();

        // 2. Create the Kiota authentication provider, passing it the credential and scopes.
        //    This is the crucial bridge between Azure.Identity and Kiota.
        var authProvider = new AzureIdentityAuthenticationProvider(credential, ["https://graph.microsoft.com/.default"]);

        // 3. Create the request adapter, which is the main engine for making calls.
        //    Pass it the authentication provider.
        var requestAdapter = new HttpClientRequestAdapter(authProvider);

        // 4. Now, you can create the GraphServiceClient, passing it the fully assembled request adapter.
        return new GraphServiceClient(requestAdapter);
    }
}