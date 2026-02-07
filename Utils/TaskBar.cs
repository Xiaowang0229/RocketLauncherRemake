
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static TaskbarIcon RootTaskBarIcon;
        public static ContextMenu TaskBarMenu = new ContextMenu();
    }
    public static class TaskBar
    {
        public static void IntializeTaskbar()
        {
            MainConfig config = new MainConfig();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            //本体初始化
            Variables.RootTaskBarIcon = new TaskbarIcon();

            Variables.RootTaskBarIcon.IconSource = Xiaowang0229.ImageLibrary.WPF.Image.ConvertByteArrayToImageSource(AppResource.ApplicationIcon);
            Variables.RootTaskBarIcon.ToolTipText = $"Rocket Launcher 主程序";

            //列表项初始化
            var tbcm = new ContextMenu();
            var OpenMainWindowItem = new MenuItem { Header = "显示主窗口" };
            var ControlGameProcess = new MenuItem { Header = "快捷管理游戏" };
            var SettingsItem = new MenuItem { Header = "打开设置页" };
            var ExitApplicationItem = new MenuItem { Header = "退出主程序" };



            tbcm.Items.Add(OpenMainWindowItem);


            //Variables.RootTaskBarIcon.ContextMenu.Items.Add(ForceQuitGameItem);
            tbcm.Items.Add(new Separator());
            tbcm.Items.Add(SettingsItem);
            tbcm.Items.Add(ExitApplicationItem);



            //绑定事件
            OpenMainWindowItem.Click += (s, e) =>
            {
                var win = Variables._MainWindow;
                var currentPage = win.RootFrame.Content;
                win.WindowState = Avalonia.Controls.WindowState.Normal;
                win.Show();
                if (currentPage is LaunchPage launchpage)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                    {
                        launchpage.BackgroundImageVisible(false);
                        launchpage.BackgroundVideoVisible(true);
                        launchpage.BackgroundVideoPlayStop(true);

                    }
                }
                win.Topmost = true;
                win.Topmost = false;
            };

            SettingsItem.Click += (s, e) =>
            {
                var win = Variables._MainWindow;
                win.Show();
                win.RootFrame.Navigate(typeof(SettingsPage));
            };
            ExitApplicationItem.Click += (s, e) =>
            {
                KillTaskBar();
                Environment.Exit(0);
            };

            Variables.RootTaskBarIcon.ContextMenu = tbcm;


            Variables.RootTaskBarIcon.TrayLeftMouseDown += (s, e) =>
            {
                var win = Variables._MainWindow;
                var currentPage = win.RootFrame.Content;


                if (Variables.MainWindowHideStatus)
                {
                    win.Show();
                    win.WindowState = Avalonia.Controls.WindowState.Normal;
                    Variables.MainWindowHideStatus = false;
                    win.Topmost = true;
                    win.Topmost = false;
                    if (currentPage is LaunchPage launchpage)
                    {
                        if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                        {
                            launchpage.BackgroundImageVisible(false);
                            launchpage.BackgroundVideoVisible(true);
                            launchpage.BackgroundVideoPlayStop(true);
                        }
                    }
                }
                else
                {
                    if (currentPage is LaunchPage launchpage)
                    {
                        if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                        {
                            launchpage.BackgroundImageVisible(false);
                            launchpage.BackgroundVideoVisible(true);
                            launchpage.BackgroundVideoPlayStop(false);
                        }
                    }
                    win.Hide();
                    Variables.MainWindowHideStatus = true;
                }

            };

        }
        public static void KillTaskBar()
        {
            Variables.RootTaskBarIcon?.Dispose();
        }
        public static void InitializeTaskBarContentMenu()
        {
            //列表项初始化
            MainConfig config = new MainConfig();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var tbcm = new ContextMenu();
            var OpenMainWindowItem = new MenuItem { Header = "显示主窗口" };
            var ControlGameProcess = new MenuItem { Header = "快捷管理游戏" };
            var SettingsItem = new MenuItem { Header = "打开设置页" };
            var ExitApplicationItem = new MenuItem { Header = "退出主程序" };

            for (int i = 0; i < Variables.GameProcess.Count; i++)
            {
                if (Variables.GameProcessStatus[i] == true)
                {
                    int index = i;
                    var subitem = new MenuItem
                    {
                        Header = $"结束 {config.GameInfos[i].ShowName}",
                        //Header = $"结束 {Variables.GameProcess[i].ProcessName}",

                    };
                    subitem.Click += async (s, e) =>
                    {


                        Variables.GameProcess[index].Kill();
                        await Task.Delay(100);
                        InitializeTaskBarContentMenu();


                    };
                    ControlGameProcess.Items.Add(subitem);
                }
            }

            tbcm.Items.Add(OpenMainWindowItem);
            try
            {
                for (int i = 0; i < Variables.GameProcess.Count; i++)
                {
                    if (Variables.GameProcessStatus[i] == true)
                    {
                        tbcm.Items.Add(ControlGameProcess);
                        break;
                    }
                }
            }
            catch { }

            //Variables.RootTaskBarIcon.ContextMenu.Items.Add(ForceQuitGameItem);
            tbcm.Items.Add(new Separator());
            tbcm.Items.Add(SettingsItem);
            tbcm.Items.Add(ExitApplicationItem);




            Variables.RootTaskBarIcon.ContextMenu = tbcm;
            //绑定事件
            OpenMainWindowItem.Click += (s, e) =>
            {
                var win = Variables._MainWindow;
                var currentPage = win.RootFrame.Content;

                win.Show();
                win.WindowState = Avalonia.Controls.WindowState.Normal;
                if (currentPage is LaunchPage launchpage)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                    {
                        launchpage.BackgroundImageVisible(false);
                        launchpage.BackgroundVideoVisible(true);
                        launchpage.BackgroundVideoPlayStop(true);
                    }
                }
                win.Topmost = true;
                win.Topmost = false;
            };

            SettingsItem.Click += (s, e) =>
            {
                var win = Variables._MainWindow;
                win.Show();
                win.RootFrame.Navigate(typeof(SettingsPage));
            };
            ExitApplicationItem.Click += (s, e) =>
            {
                KillTaskBar();
                Environment.Exit(0);
            };


        }
    }
}
