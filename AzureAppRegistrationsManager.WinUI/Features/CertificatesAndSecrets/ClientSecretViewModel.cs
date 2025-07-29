using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public class ClientSecretViewModel
{
    public PasswordCredential PasswordCredential { get; set; } = null!;

    public bool CanEdit { get; set; }
}