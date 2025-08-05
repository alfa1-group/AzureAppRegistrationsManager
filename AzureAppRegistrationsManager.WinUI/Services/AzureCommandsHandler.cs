using System.Data;
using AzureAppRegistrationsManager.WinUI.Models;
using Microsoft.Graph;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Applications.Item.RemovePassword;
using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Services;

internal static class AzureCommandsHandler
{
    internal static async Task<IReadOnlyList<AppRegInfo>> GetAllApplicationsAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        var applications = await App.GraphClient.Applications.GetAsync(q =>
        {
            q.QueryParameters.Select = ["appId", "displayName", "id", "owners"];
            q.QueryParameters.Expand = ["owners($select=id)"];
        });

        if (applications == null || applications.Value == null)
        {
            return [];
        }

        var allApplications = new List<AppRegInfo>();

        var pageIterator = PageIterator<Application, ApplicationCollectionResponse>.CreatePageIterator(
            App.GraphClient,
            applications,
            app =>
            {
                if (app == null || app.AppId == null || app.Id == null || string.IsNullOrWhiteSpace(app.DisplayName))
                {
                    return false;
                }

                allApplications.Add(new AppRegInfo
                {
                    AppId = app.AppId,
                    DisplayName = app.DisplayName,
                    ObjectId = app.Id,
                    CanEdit = UserIsOwnerFromAppReg(app, userId)
                });
                return true;
            });

        await pageIterator.IterateAsync();

        return allApplications
            .OrderBy(app => app.DisplayName)
            .ToArray();
    }

    internal static async Task<IReadOnlyList<AppRegInfo>> GetOwnApplicationsAsync()
    {
        var ownedObjects = await App.GraphClient.Me.OwnedObjects.GetAsync(q =>
        {
            q.QueryParameters.Select = ["appId", "displayName", "id"];
        });

        if (ownedObjects == null || ownedObjects.Value == null)
        {
            return [];
        }

        var ownApplications = new List<AppRegInfo>();

        var pageIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>.CreatePageIterator(
            App.GraphClient,
            ownedObjects,
            obj =>
            {
                if (obj is Application app && app.AppId != null && app.Id != null && !string.IsNullOrWhiteSpace(app.DisplayName))
                {
                    ownApplications.Add(new AppRegInfo
                    {
                        AppId = app.AppId,
                        DisplayName = app.DisplayName,
                        ObjectId = app.Id,
                        CanEdit = true
                    });
                }
                return true;
            });

        await pageIterator.IterateAsync();

        return ownApplications
            .OrderBy(app => app.DisplayName)
            .ToArray();
    }

    internal static Task<Application?> GetApplicationAsync(string id)
    {
        return App.GraphClient.Applications[id].GetAsync();
    }

    internal static async Task<string> GetCurrentUserIdAsync()
    {
        var user = await App.GraphClient.Me.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = ["id"];
        });

        return user?.Id ?? string.Empty;
    }

    internal static bool UserIsOwnerFromAppReg(Application? application, string userId)
    {
        return application?.Owners?.Any(owner => string.Equals(owner.Id, userId, StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    internal static async Task UpdateWebRedirectUrisAsync(string appId, List<string> uris)
    {
        var request = new Application
        {
            Web = new WebApplication
            {
                RedirectUris = uris
            }
        };
        await ExecuteAzRestPatchAsync(appId, request);
    }

    internal static async Task UpdateSpaRedirectUrisAsync(string appId, List<string> uris)
    {
        var request = new Application
        {
            Spa = new SpaApplication
            {
                RedirectUris = uris
            }
        };
        await ExecuteAzRestPatchAsync(appId, request);
    }

    internal static async Task UpdateIdentifierUrisAsync(string appId, List<string> uris)
    {
        var request = new Application
        {
            IdentifierUris = uris
        };

        await ExecuteAzRestPatchAsync(appId, request);
    }

    internal static async Task UpdateImplicitGrantSettingsAsync(string appId, ImplicitGrantSettings? settings)
    {
        if (settings == null)
        {
            return;
        }

        var request = new Application
        {
            Web = new WebApplication
            {
                ImplicitGrantSettings = settings
            }
        };

        await ExecuteAzRestPatchAsync(appId, request);
    }

    internal static async Task UpdateLogoutUrlAsync(string appId, string? logoutUrl)
    {
        if (string.IsNullOrWhiteSpace(logoutUrl))
        {
            return;
        }

        var request = new Application
        {
            Web = new WebApplication
            {
                LogoutUrl = logoutUrl
            }
        };

        await ExecuteAzRestPatchAsync(appId, request);
    }

    internal static async Task UpdateDisplayNameAsync(string appId, string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return;
        }

        var request = new Application
        {
            DisplayName = displayName
        };
        await ExecuteAzRestPatchAsync(appId, request);
    }

    internal static async Task UpdateScopesAsync(string id, List<PermissionScope> scopes)
    {
        if (scopes.Any(s => string.IsNullOrWhiteSpace(s.Value)))
        {
            return;
        }

        var request = new Application
        {
            Api = new ApiApplication
            {
                Oauth2PermissionScopes = scopes
            }
        };
        await ExecuteAzRestPatchAsync(id, request);
    }

    internal static async Task DeleteScopeAsync(string id, ApiApplication api, PermissionScope scopeToDelete)
    {
        if (api.Oauth2PermissionScopes == null || api.Oauth2PermissionScopes.Count == 0)
        {
            return;
        }

        scopeToDelete.IsEnabled = false;
        var disableRequest = new Application
        {
            Api = api
        };
        await ExecuteAzRestPatchAsync(id, disableRequest);

        api.Oauth2PermissionScopes = api.Oauth2PermissionScopes.Except([scopeToDelete]).ToList();
        var deleteRequest = new Application
        {
            Api = api
        };
        await ExecuteAzRestPatchAsync(id, deleteRequest);
    }

    internal static async Task UpdateAppRolesAsync(string id, List<AppRole> roles)
    {
        if (roles.Any(s => string.IsNullOrWhiteSpace(s.Value)))
        {
            return;
        }

        var request = new Application
        {
            AppRoles = roles
        };
        await ExecuteAzRestPatchAsync(id, request);
    }

    internal static async Task DeleteAppRoleAsync(string id, List<AppRole> existingRoles, AppRole roleToDelete)
    {
        if (existingRoles.Count == 0)
        {
            return;
        }

        roleToDelete.IsEnabled = false;
        var disableRequest = new Application
        {
            AppRoles = existingRoles
        };
        await ExecuteAzRestPatchAsync(id, disableRequest);

        var deleteRequest = new Application
        {
            AppRoles = existingRoles.Except([roleToDelete]).ToList()
        };
        await ExecuteAzRestPatchAsync(id, deleteRequest);
    }

    internal static async Task<string> AddClientSecretAsync(string id, PasswordCredential passwordCredential)
    {
        var addedSecret = await App.GraphClient.Applications[id]
            .AddPassword
            .PostAsync(new AddPasswordPostRequestBody
            {
                PasswordCredential = passwordCredential
            });

        return addedSecret!.SecretText!;
    }

    internal static async Task DeleteClientSecretAsync(string id, Guid keyId)
    {
        await App.GraphClient.Applications[id]
            .RemovePassword
            .PostAsync(new RemovePasswordPostRequestBody
            {
                KeyId = keyId
            });
    }

    private static async Task ExecuteAzRestPatchAsync(string id, Application request)
    {
        await App.GraphClient.Applications[id].PatchAsync(request);
    }
}