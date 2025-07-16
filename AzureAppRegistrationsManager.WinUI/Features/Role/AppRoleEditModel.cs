using System.ComponentModel.DataAnnotations;
using Aspects.Notify;
using Mapster;
using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Role;

[AdaptTo(typeof(AppRole))]
internal class AppRoleEditModel
{
    public Guid Id { get; set; } = new();

    [Notify]
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Notify]
    [MinLength(1)]
    public List<string> AllowedMemberTypes { get; set; } = [];

    public bool IsUsersGroupsSelected
    {
        get => AllowedMemberTypes.Contains("User") && !AllowedMemberTypes.Contains("Application");
        set
        {
            AllowedMemberTypes = ["User"];
        }
    }

    public bool IsApplicationsSelected
    {
        get => AllowedMemberTypes.Contains("Application") && !AllowedMemberTypes.Contains("User");

        set
        {
            AllowedMemberTypes = ["Application"];
        }
    }

    public bool IsBothSelected
    {
        get => AllowedMemberTypes.Contains("User") && AllowedMemberTypes.Contains("Application");
        set
        {
            AllowedMemberTypes = ["User", "Application"];
        }
    }

    [Notify]
    [Required]
    public string Value { get; set; } = string.Empty;

    [Notify]
    [Required]
    public string Description { get; set; } = string.Empty;

    [Notify]
    public bool IsEnabled { get; set; } = true;
}
