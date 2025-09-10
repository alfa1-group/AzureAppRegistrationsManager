using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using AzureAppRegistrationsManager.WinUI.Features;
using AzureAppRegistrationsManager.WinUI.Models;
using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;

namespace AzureAppRegistrationsManager.WinUI.Services;

internal static class RequestResourceAccessHandler
{
    private const string TenantIdClaimType = "tid";
    private static readonly string TenantId;

    static RequestResourceAccessHandler()
    {
        var accessToken = App.AzureCredential.GetToken(new TokenRequestContext(["https://graph.microsoft.com/.default"]));
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken.Token);

        TenantId = jwt.Claims.FirstOrDefault(c => c.Type == TenantIdClaimType)?.Value ?? throw new InvalidOperationException($"The {TenantIdClaimType} claim type is not found in token.");
    }

    internal static async Task RequestAsync(UIElement ui, AppRegInfo appRegInfo, IEnumerable<string> scopes)
    {
        if (appRegInfo.Application?.Id == null)
        {
            return;
        }

        var currentUrls = appRegInfo.Application.Web?.RedirectUris?.ToList() ?? [];
        var redirectUrl = $"http://localhost/{Guid.NewGuid()}";

        var updateWebRedirectUrisSuccess = false;
        try
        {
            var urls = new List<string>(currentUrls)
            {
                redirectUrl
            };

            await AzureCommandsHandler.UpdateWebRedirectUrisAsync(appRegInfo.Application.Id, urls);
            updateWebRedirectUrisSuccess = true;

            var pca = PublicClientApplicationBuilder
                .Create(appRegInfo.AppId)
                .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                .WithRedirectUri(redirectUrl)
                .Build();

            _ = await pca.AcquireTokenInteractive(scopes)
                //.WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            await new ErrorDialog(ex.Message)
            {
                XamlRoot = ui.XamlRoot
            }.ShowAsync();
        }
        finally
        {
            if (updateWebRedirectUrisSuccess)
            {
                await AzureCommandsHandler.UpdateWebRedirectUrisAsync(appRegInfo.Application.Id, currentUrls);
            }
        }
    }
}