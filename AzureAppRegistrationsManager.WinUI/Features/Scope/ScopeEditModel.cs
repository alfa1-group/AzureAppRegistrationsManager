using System.ComponentModel.DataAnnotations;
using Aspects.Notify;
using Mapster;
using Microsoft.Graph.Models;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

[AdaptTo(typeof(PermissionScope))]
public class ScopeEditModel
{
    public Guid Id { get; set; }

    public string ApiUriOriginal { get; set; } = string.Empty;

    [Notify]
    [Required]
    public string Value { get; set; } = string.Empty;

    [Notify]
    [Required]
    public string AdminConsentDisplayName { get; set; } = string.Empty;

    [Notify]
    [Required]
    public string AdminConsentDescription { get; set; } = string.Empty;

    public string UserConsentDisplayName { get; set; } = string.Empty;

    public string UserConsentDescription { get; set; } = string.Empty;

    public string Type => IsAdminsAndUsers ? "User" : "Admin";

    private bool _isAdminsAndUsers = true;
    public bool IsAdminsAndUsers
    {
        get => _isAdminsAndUsers;
        set
        {
            _isAdminsAndUsers = value;
            // Update the opposite radio button
            if (value)
            {
                _isAdminsOnly = false;
            }
        }
    }

    private bool _isAdminsOnly;
    public bool IsAdminsOnly
    {
        get => _isAdminsOnly;
        set
        {
            _isAdminsOnly = value;
            // Update the opposite radio button
            if (value)
            {
                _isAdminsAndUsers = false;
            }
        }
    }

    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            // Update the opposite radio button
            if (value)
            {
                _isDisabled = false;
            }
        }
    }

    private bool _isDisabled;
    public bool IsDisabled
    {
        get => _isDisabled;
        set
        {
            _isDisabled = value;
            // Update the opposite radio button
            if (value)
            {
                _isEnabled = false;
            }
        }
    }
}