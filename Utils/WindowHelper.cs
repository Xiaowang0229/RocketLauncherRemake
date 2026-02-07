using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static MainWindow _MainWindow = new MainWindow();
    }
    public static class WindowHelper
    {


        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Environment.Exit(0);
        }

        public static async Task<string?> OpenInputWindow(string Title)
        {
            var win = new InputWindow(Title);
            var result = await win.ShowDialog<string?>(Variables._MainWindow);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public async static Task<bool> ShowMessageAsync(this AppWindow owner, string Title, string Content)
        {
            var dialog = new ContentDialog
            {
                Title = Title,
                Content = Content,
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };
            var res = await dialog.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
