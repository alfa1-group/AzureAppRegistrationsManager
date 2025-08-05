using System.ComponentModel.DataAnnotations;
using Aspects.Notify;

namespace AzureAppRegistrationsManager.WinUI.Features.Owners;

internal class OwnerAddModel
{
    [Notify]
    [Required]
    public string Email { get; set; } = string.Empty;
}