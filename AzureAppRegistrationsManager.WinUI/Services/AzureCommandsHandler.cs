using AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;
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

        if (applications?.Value == null)
        {
            return [];
        }

        var appRegInfoList = new List<AppRegInfo>();

        var applicationsPageIterator = PageIterator<Application, ApplicationCollectionResponse>.CreatePageIterator(
            App.GraphClient,
            applications,
            app =>
            {
                if (app?.AppId == null || app.Id == null || string.IsNullOrWhiteSpace(app.DisplayName))
                {
                    return false;
                }

                appRegInfoList.Add(new AppRegInfo
                {
                    AppId = app.AppId,
                    DisplayName = app.DisplayName,
                    ObjectId = app.Id,
                    CanEdit = UserIsOwnerFromAppReg(app, userId)
                });
                return true;
            });

        await applicationsPageIterator.IterateAsync();

        await UpdateAppRegInfoListWithServicePrincipalsAsync(appRegInfoList);

        return appRegInfoList
            .OrderBy(app => app.DisplayName)
            .ToArray();
    }

    internal static async Task<IReadOnlyList<AppRegInfo>> GetOwnApplicationsAsync()
    {
        var ownedObjects = await App.GraphClient.Me.OwnedObjects.GetAsync(q =>
        {
            q.QueryParameters.Select = ["appId", "displayName", "id"];
        });

        if (ownedObjects?.Value == null)
        {
            return [];
        }

        var appRegInfoList = new List<AppRegInfo>();

        var pageIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>.CreatePageIterator(
            App.GraphClient,
            ownedObjects,
            obj =>
            {
                if (obj is Application { AppId: not null, Id: not null } app && !string.IsNullOrWhiteSpace(app.DisplayName))
                {
                    appRegInfoList.Add(new AppRegInfo
                    {
                        AppId = app.AppId,
                        DisplayName = app.DisplayName,
                        ObjectId = app.Id!,
                        CanEdit = true
                    });
                }

                return true;
            });

        await pageIterator.IterateAsync();

        await UpdateAppRegInfoListWithServicePrincipalsAsync(appRegInfoList);

        return appRegInfoList
            .OrderBy(app => app.DisplayName)
            .ToArray();
    }

    internal static async Task<Application?> GetApplicationAsync(string id)
    {
        var app = await App.GraphClient.Applications[id].GetAsync();
        app?.Owners = await GetOwnersAsync(id);

        return app;
    }

    internal static async Task<List<DirectoryObject>> GetOwnersAsync(string id)
    {
        var owners = await App.GraphClient.Applications[id].Owners.GetAsync();
        if (owners?.Value == null)
        {
            return [];
        }

        var directoryObjects = new List<DirectoryObject>();

        var pageIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>.CreatePageIterator(
            App.GraphClient,
            owners,
            obj =>
            {
                if (obj is User user)
                {
                    directoryObjects.Add(user);
                }
                return true;
            });

        await pageIterator.IterateAsync();

        return directoryObjects
            .OrderBy(d => d.Id)
            .ToList();
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
        await ExecuteAzRestPatchOnApplicationAsync(appId, request);
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
        await ExecuteAzRestPatchOnApplicationAsync(appId, request);
    }

    internal static async Task UpdateIdentifierUrisAsync(string appId, List<string> uris)
    {
        var request = new Application
        {
            IdentifierUris = uris
        };

        await ExecuteAzRestPatchOnApplicationAsync(appId, request);
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

        await ExecuteAzRestPatchOnApplicationAsync(appId, request);
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

        await ExecuteAzRestPatchOnApplicationAsync(appId, request);
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
        await ExecuteAzRestPatchOnApplicationAsync(appId, request);
    }

    internal static async Task UpdateRequestedAccessTokenVersionAsync(string id, int requestedAccessTokenVersion)
    {
        var request = new Application
        {
            Api = new ApiApplication
            {
                RequestedAccessTokenVersion = requestedAccessTokenVersion
            }
        };
        await ExecuteAzRestPatchOnApplicationAsync(id, request);
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
        await ExecuteAzRestPatchOnApplicationAsync(id, request);
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
        await ExecuteAzRestPatchOnApplicationAsync(id, disableRequest);

        api.Oauth2PermissionScopes = api.Oauth2PermissionScopes.Except([scopeToDelete]).ToList();
        var deleteRequest = new Application
        {
            Api = api
        };
        await ExecuteAzRestPatchOnApplicationAsync(id, deleteRequest);
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
        await ExecuteAzRestPatchOnApplicationAsync(id, request);
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
        await ExecuteAzRestPatchOnApplicationAsync(id, disableRequest);

        var deleteRequest = new Application
        {
            AppRoles = existingRoles.Except([roleToDelete]).ToList()
        };
        await ExecuteAzRestPatchOnApplicationAsync(id, deleteRequest);
    }

    internal static async Task<string?> AddClientSecretAsync(string id, PasswordCredential passwordCredential)
    {
        var addedSecret = await App.GraphClient.Applications[id]
            .AddPassword
            .PostAsync(new AddPasswordPostRequestBody
            {
                PasswordCredential = passwordCredential
            });

        return addedSecret?.SecretText;
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

    internal static async Task<string> GetUserIdByEmailAsync(string emailAddress)
    {
        var users = await App.GraphClient.Users
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Filter = $"userPrincipalName eq '{emailAddress}' or mail eq '{emailAddress}'";
                requestConfiguration.QueryParameters.Select = ["id"];
            });

        var userId = users?.Value?.FirstOrDefault()?.Id;
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception($"User with email '{emailAddress}' not found.");
        }

        return userId;
    }

    internal static async Task AddAppOwnerByEmailAsync(string applicationId, string ownerEmail)
    {
        var userId = await GetUserIdByEmailAsync(ownerEmail);
        var directoryObject = new ReferenceCreate
        {
            OdataId = $"https://graph.microsoft.com/v1.0/users/{userId}"
        };

        await App.GraphClient.Applications[applicationId].Owners.Ref.PostAsync(directoryObject);
    }

    internal static async Task RemoveOwnerFromAppRegistrationByEmailAsync(string applicationId, string ownerEmail)
    {
        var userId = await GetUserIdByEmailAsync(ownerEmail);

        await App.GraphClient.Applications[applicationId].Owners[userId].Ref.DeleteAsync();
    }

    internal static async Task<ServicePrincipal?> ConvertToEnterpriseApplication(string _, ServicePrincipal value)
    {
        var servicePrincipal = new ServicePrincipal
        {
            AppId = value.AppId,
            DisplayName = value.DisplayName,
            AccountEnabled = true
        };

        return await App.GraphClient.ServicePrincipals.PostAsync(servicePrincipal);
    }

    internal static async Task RemoveEnterpriseApplication(string _, string? servicePrincipalId)
    {
        if (string.IsNullOrWhiteSpace(servicePrincipalId))
        {
            return;
        }

        await App.GraphClient.ServicePrincipals[servicePrincipalId].DeleteAsync();
    }

    internal static async Task<IReadOnlyList<ApiPermissionModel>> GetPermissionsAsync(string? servicePrincipalId)
    {
        if (string.IsNullOrEmpty(servicePrincipalId))
        {
            return [];
        }

        // Get OAuth2 permission grants (delegated permissions)
        var oauth2GrantsResponse = await App.GraphClient.ServicePrincipals[servicePrincipalId]
            .Oauth2PermissionGrants
            .GetAsync();
        var oauth2Grants = oauth2GrantsResponse?.Value ?? [];

        // Get app role assignments (application permissions)
        var appRoleAssignmentsResponse = await App.GraphClient.ServicePrincipals[servicePrincipalId]
            .AppRoleAssignments
            .GetAsync();
        var appRoleAssignments = appRoleAssignmentsResponse?.Value ?? [];

        var referencedServicePrincipalIds = oauth2Grants
            .Select(g => g.ResourceId)
            .OfType<string>()
            .Distinct()
            .ToArray();

        var referencedEnterpriseApplications = new List<ServicePrincipal>();
        foreach (var referencedServicePrincipalId in referencedServicePrincipalIds)
        {
            var found = _allEnterpriseApplications.FirstOrDefault(sp => sp.Id == referencedServicePrincipalId);
            if (found != null)
            {
                referencedEnterpriseApplications.Add(found);
            }
            else
            {
                var enterpriseApplication = await App.GraphClient.ServicePrincipals[referencedServicePrincipalId].GetAsync(q =>
                {
                    q.QueryParameters.Select = ["appId", "id", "displayName"];
                });

                if (enterpriseApplication != null)
                {
                    referencedEnterpriseApplications.Add(enterpriseApplication);
                }
            }
        }

        var list = new List<ApiPermissionModel>();
        foreach (var oauth2Grant in oauth2Grants)
        {
            list.Add(new ApiPermissionModel
            {
                ApplicationName = referencedEnterpriseApplications.FirstOrDefault(e => e.Id == oauth2Grant.ResourceId)?.DisplayName ?? oauth2Grant.ResourceId!,
                Scope = oauth2Grant.Scope!.Trim(),

                // Note: ConsentType is "AllPrincipals" or "Principal"
                // - AllPrincipals indicates authorization to impersonate all users.
                // - Principal indicates authorization to impersonate a specific user.
                ConsentType = oauth2Grant.ConsentType!
            });
        }

        return list;
    }

    private static List<ServicePrincipal> _allEnterpriseApplications = [];

    private static async Task UpdateAppRegInfoListWithServicePrincipalsAsync(List<AppRegInfo> appRegInfoList)
    {
        var enterpriseApplications = await App.GraphClient.ServicePrincipals.GetAsync(q =>
        {
            q.QueryParameters.Select = ["appId", "id", "displayName"];
        });

        if (enterpriseApplications?.Value == null)
        {
            return;
        }

        _allEnterpriseApplications = [];

        var enterpriseApplicationsPageIterator = PageIterator<ServicePrincipal, ServicePrincipalCollectionResponse>.CreatePageIterator(
            App.GraphClient,
            enterpriseApplications,
            sp =>
            {
                if (sp?.AppId == null || sp.Id == null)
                {
                    return false;
                }

                _allEnterpriseApplications.Add(sp);

                var application = appRegInfoList.FirstOrDefault(a => a.AppId == sp.AppId);
                application?.EnterpriseApplication = sp;

                return true;
            });

        await enterpriseApplicationsPageIterator.IterateAsync();
    }

    private static async Task ExecuteAzRestPatchOnApplicationAsync(string id, Application request)
    {
        await App.GraphClient.Applications[id].PatchAsync(request);
    }
}