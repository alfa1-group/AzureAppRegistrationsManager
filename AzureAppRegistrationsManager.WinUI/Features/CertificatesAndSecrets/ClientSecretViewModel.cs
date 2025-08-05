using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.CertificatesAndSecrets;

public class ClientSecretViewModel
{
    public PasswordCredential PasswordCredential { get; set; } = null!;

    public string KeyId => PasswordCredential.KeyId!.Value.ToString("D");

    public string Hint => PasswordCredential.Hint + "******************";

    public string ExpiresOn => PasswordCredential.EndDateTime?.ToString("yyyy-dd-MM") ?? "Never";

    public bool CanEdit { get; set; }
}