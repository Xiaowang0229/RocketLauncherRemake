using Avalonia.Media;
using System;
using System.Diagnostics;
using System.Reflection;

namespace RocketLauncherRemake.Utils
{

    public static class ColorHelper
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

        



        /// <summary>
        /// 把 WPF Brush 转换为 Avalonia IBrush（目前主要支持 SolidColorBrush）
        /// </summary>
        public static IBrush? ToAvaloniaBrush(this System.Windows.Media.Brush? wpfBrush)
        {
            if (wpfBrush == null) return null;

            return wpfBrush switch
            {
                System.Windows.Media.SolidColorBrush solid =>
                    new Avalonia.Media.SolidColorBrush(solid.Color.ToAvaloniaColor()),

                // 如果以后需要支持更多类型，可以继续扩展
                // System.Windows.Media.LinearGradientBrush linear => ...,
                // System.Windows.Media.ImageBrush image => ...,

                _ => throw new NotSupportedException(
                    $"暂不支持将 {wpfBrush.GetType().Name} 转换为 Avalonia IBrush")
            };
        }

        /// <summary>
        /// WPF Color → Avalonia Color 的快捷转换
        /// </summary>
        public static Avalonia.Media.Color ToAvaloniaColor(this System.Windows.Media.Color color)
        {
            return new Avalonia.Media.Color(color.A, color.R, color.G, color.B);
        }
    }
}
