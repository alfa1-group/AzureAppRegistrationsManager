using System.ComponentModel.DataAnnotations;
using Aspects.Notify;
using Mapster;
using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions.NotUsed;

[AdaptTo(typeof(OAuth2PermissionGrant))]
public class OAuth2PermissionGrantEditModel
{
    /// <summary>
    /// service principal objectId of the client app
    /// </summary>
    [Notify]
    [Required]
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// service principal objectId of the resource API
    /// </summary>
    [Notify]
    [Required]
    public string ResourceId { get; set; } = string.Empty;

    public string ConsentType
    {
        get => _isAllPrincipals ? "AllPrincipals" : "Principal";
        set
        {
            if (value == "AllPrincipals")
            {
                _isAllPrincipals = true;
            }

            _isPrincipal = !_isAllPrincipals;
        }
    }

    /// <summary>
    /// space-separated delegated scopes (values)
    /// </summary>
    [Notify]
    [Required]
    public string Scope { get; set; } = string.Empty;


    private bool _isAllPrincipals = true;
    public bool IsAllPrincipals
    {
        get => _isAllPrincipals;
        set
        {
            _isAllPrincipals = value;
            _isPrincipal = !_isAllPrincipals;
        }
    }

    private bool _isPrincipal;
    public bool IsPrincipal
    {
        get => _isPrincipal;
        set
        {
            _isPrincipal = value;
            _isAllPrincipals = !_isPrincipal;
        }
    }
}