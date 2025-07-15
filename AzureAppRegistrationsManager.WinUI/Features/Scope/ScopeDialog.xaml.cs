using System.ComponentModel;
using Microsoft.Graph.Models;
using Microsoft.UI.Xaml.Controls;

namespace AzureAppRegistrationsManager.WinUI.Features.Scope;

public sealed partial class ScopeDialog : ContentDialog, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ScopeDialog(string apiUri)
    {
        InitializeComponent();

        PermissionScope = new PermissionScope
        {
            Id = Guid.NewGuid(),
            Value = string.Empty,
            AdminConsentDisplayName = string.Empty,
            AdminConsentDescription = string.Empty,
            UserConsentDisplayName = string.Empty,
            UserConsentDescription = string.Empty,
            Type = "User",
            IsEnabled = true
        };

        IsAdminsAndUsers = true;
        IsScopeEnabled = true;
        ApiUriOriginal = apiUri;

        Title = "Add Scope";
        PrimaryButtonClick += AddScopeDialog_PrimaryButtonClick;
    }

    public ScopeDialog(string apiUri, PermissionScope existingScope) : this(apiUri)
    {
        PermissionScope = existingScope;
        Title = "Edit Scope";
        PopulateRadioButtons();
    }

    public PermissionScope PermissionScope { get; set; }
    
    public string ApiUriOriginal { get; private set; }
    
    public string ApiUri => ApiUriOriginal + "/" + ScopeNameTextBox.Text;

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

    private bool _isAdminsOnly = false;
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
    public bool IsScopeEnabled
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

    private bool _isDisabled = false;
    public bool IsScopeDisabled
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

    private void PopulateRadioButtons()
    {
        IsAdminsAndUsers = PermissionScope.Type?.ToLower() != "admin";
        IsScopeEnabled = PermissionScope.IsEnabled ?? true;
    }

    private void AddScopeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Update PermissionScope with radio button selections
        PermissionScope.Type = IsAdminsAndUsers ? "User" : "Admin";
        PermissionScope.IsEnabled = IsScopeEnabled;
    }

    private void ScopeNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ApiUri)));
    }
}