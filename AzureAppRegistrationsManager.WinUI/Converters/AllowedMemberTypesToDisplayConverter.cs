using Microsoft.UI.Xaml.Data;

namespace AzureAppRegistrationsManager.WinUI.Converters;

internal partial class AllowedMemberTypesToDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is List<string> allowedMemberTypes)
        {
            if (allowedMemberTypes.Contains("User") && allowedMemberTypes.Contains("Application"))
            {
                return "Users / Groups, Applications";
            }
            
            if (allowedMemberTypes.Contains("User"))
            {
                return "Users / Groups";
            }
            
            if (allowedMemberTypes.Contains("Application"))
            {
                return "Applications";
            }
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Not needed for one-way binding
        throw new NotImplementedException();
    }
}
