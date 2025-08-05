using System.ComponentModel.DataAnnotations;
using Aspects.Notify;
using Mapster;
using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

[AdaptTo(typeof(PasswordCredential))]
internal class ClientSecretAddModel
{
    [Notify]
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public DateTimeOffset EndDateTime { get; set; } = DateTimeOffset.UtcNow.AddYears(1);
}