using Microsoft.UI.Xaml.Data;

namespace AzureAppRegistrationsManager.WinUI.Converters;

internal class ConsentTypeToDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string consentType)
        {
            return consentType switch
            {
                "Admin" => "Admins only",
                "User" => "Admins and Users",
                _ => string.Empty
            };
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Not needed for one-way binding
        throw new NotImplementedException();
    }
}
