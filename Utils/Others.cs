using Avalonia.Media;
using System.Diagnostics;
using System.Reflection;

namespace RocketLauncherRemake.Utils
{

    public static class Others
    {
        public static string GetColorName(System.Drawing.Color color)
        {
            foreach (PropertyInfo prop in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (prop.PropertyType == typeof(System.Drawing.Color))
                {
                    System.Drawing.Color namedColor = (System.Drawing.Color)prop.GetValue(null);
                    if (namedColor.R == color.R &&
                        namedColor.G == color.G &&
                        namedColor.B == color.B &&
                        namedColor.A == color.A)
                    {
                        return prop.Name;
                    }
                }
            }
            return color.ToString();
        }

        public static void OpenBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute =true
            });return;
        }
    }
}
