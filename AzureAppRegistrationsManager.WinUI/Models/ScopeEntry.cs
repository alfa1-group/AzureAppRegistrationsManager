namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public class ScopeEntry
{
    public required string Scope { get; set; }

    public required string WhoCanConsent { get; set; }

    public required string AdminConsentDisplay { get; set; }

    public required string UserConsentDisplay { get; set; }

    public required string State { get; set; }
}