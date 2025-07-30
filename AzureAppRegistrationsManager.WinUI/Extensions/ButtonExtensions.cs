namespace Microsoft.UI.Xaml.Controls;

internal static class ButtonExtensions
{
    internal static List<TextBox> GetChildTextBoxes(this Button button)
    {
        if (button.Parent is Panel panel)
        {
            return panel.Children.OfType<TextBox>().ToList();
        }

        return [];
    }
}
