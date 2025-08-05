using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Owners;

public class OwnerViewModel
{
    private const string DefaultValue = "?";

    public DirectoryObject DirectoryObject { get; set; } = null!;

    public string Name
    {
        get
        {
            switch (DirectoryObject)
            {
                case User user:
                    return user.DisplayName ?? user.UserPrincipalName ?? DefaultValue;

                case Group group:
                    return group.DisplayName ?? DefaultValue;

                default:
                    return DirectoryObject.Id?.ToString() ?? DefaultValue;
            }
        }
    }

    public string? Mail
    {
        get
        {
            switch (DirectoryObject)
            {
                case User user:
                    return user.Mail;

                case Group group:
                    return group.Mail;

                default:
                    return DefaultValue;
            }
        }
    }

    public string Type
    {
        get
        {
            switch (DirectoryObject)
            {
                case User:
                    return "User";

                case Group:
                    return "Group";

                default:
                    return "Other";
            }
        }
    }

    public bool CanEdit { get; set; }
}