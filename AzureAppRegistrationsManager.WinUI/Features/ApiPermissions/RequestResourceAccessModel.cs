using System.ComponentModel.DataAnnotations;
using Aspects.Notify;

namespace AzureAppRegistrationsManager.WinUI.Features.ApiPermissions;

public class RequestResourceAccessModel
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

    /// <summary>
    /// space-separated delegated scopes (values)
    /// </summary>
    [Notify]
    [Required]
    public string Scopes { get; set; } = string.Empty;
}