using Microsoft.UI.Xaml.Controls;
using WinUI.Validation;

namespace AzureAppRegistrationsManager.WinUI.Features.Owners;

internal sealed partial class AddOwnerDialog : ContentDialog
{
    public OwnerAddModel Owner { get; set; }

    public AddOwnerDialog()
    {
        Owner = new OwnerAddModel
        {
            Email = string.Empty
        };

        Title = "Add an owner";
        InitializeComponent();
    }

    private void ValidationChanged(object sender, ValidationStateChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = e.ValidationState.IsValid;
    }
}