using System.Text.Json.Serialization;
using AzureAppRegistrationsManager.WinUI.Extensions;

namespace Microsoft.Graph.Models;

internal static class AppRegExtensions
{
    extension(Application app)
    {
        [JsonIgnore]
        public string ApplicationIdUri
        {
            get => app.IdentifierUris?.FirstOrDefault() ?? string.Empty;
            set => app.IdentifierUris = !string.IsNullOrWhiteSpace(value) ? [value] : [];
        }

        [JsonIgnore]
        public string WebRedirectUrisText
        {
            get => (app.Web?.RedirectUris).JoinToString();
            set
            {
                app.Web ??= new WebApplication();
                app.Web.RedirectUris = value.SplitToList();
            }
        }

        [JsonIgnore]
        public string SpaRedirectUrisText
        {
            get => (app.Spa?.RedirectUris).JoinToString();
            set
            {
                app.Spa ??= new SpaApplication();
                app.Spa.RedirectUris = value.SplitToList();
            }
        }

        [JsonIgnore]
        public List<PermissionScope> Oauth2PermissionScopes
        {
            get => app?.Api?.Oauth2PermissionScopes ?? [];
            set
            {
                app.Api ??= new ApiApplication();
                app.Api.Oauth2PermissionScopes = value;
            }
        }
    }
}