using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Windowing;
using Ookii.Dialogs.Wpf;
using RocketLauncherRemake.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xiaowang0229.JsonLibrary;




namespace RocketLauncherRemake
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {

                desktop.MainWindow = OnLaunch();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private AppWindow OnLaunch()
        {
            if (!File.Exists($"{Environment.CurrentDirectory}\\Config.json"))
            {
                var config2 = new MainConfig
                {
                    Username = "Administrator",
                    StartUpCheckUpdate = true,
                    LaunchWithMinize = true,
                    GameInfos = new List<LaunchConfig>()
                };
                Json.WriteJson($"{Environment.CurrentDirectory}\\Config.json", config2);
            }
            else
            {
                Variables.config = Json.ReadJson<MainConfig>(Variables.Configpath);
            }

            RegisterGlobalExceptionHandlers();
 

            Variables.VersionLog = FileHelper.ReadEmbeddedMarkdown("RocketLauncherRemake.LocalLog.md");
            Variables.EULAString = FileHelper.ReadEmbeddedMarkdown("RocketLauncherRemake.EULA.md");


            var config = Json.ReadJson<MainConfig>(Variables.Configpath);

            //读取逻辑

            if (!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds");
            }

            for (int i = 0; i < config.GameInfos.Count; i++)
            {
                int index = i;
                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = config.GameInfos[i].Launchpath,
                    Arguments = config.GameInfos[i].Arguments,
                    UseShellExecute = true
                };
                Variables.GameProcess.Add(proc);
                Variables.PlayingTimeintList.Add(0);
                Variables.GameProcessStatus.Add(false);

                var dt = new DispatcherTimer();
                dt.Interval = TimeSpan.FromMinutes(1);

                dt.Tick += async (s, e) =>
                {
                    Variables.PlayingTimeintList[index] += 1;
                };
                Variables.PlayingTimeRecorder.Add(dt);

            }


            if (config.StartUpCheckUpdate)
            {
                Update.CheckUpdate();

            }
            TaskBar.IntializeTaskbar();
            return Variables._MainWindow;
        }

        public static void RegisterGlobalExceptionHandlers()
        {
            // 捕获 UI 线程未处理的异常
            Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (s, e) =>
            {


                //MessageBox.Show($"发生错误：{e.Exception}","错误",MessageBoxButton.OK,MessageBoxImage.Error);
                var mb = new TaskDialog
                {
                    WindowTitle = "错误",
                    MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Error,
                    MainInstruction = "程序发生错误，您可将下方内容截图并上报错误",

                    Content = $"{e.Exception}",
                    ButtonStyle = TaskDialogButtonStyle.CommandLinks,


                };
                var mbb1 = new TaskDialogButton
                {

                    Text = "打开错误报告页面(推荐)",
                    CommandLinkNote = "将会自动复制错误信息到剪贴板,可能需要启动网络代理以进入Github",

                };
                mb.Buttons.Add(mbb1);
                var mbb2 = new TaskDialogButton
                {
                    Text = "退出程序",
                    CommandLinkNote = "退出程序以保证错误不再发生",

                };
                mb.Buttons.Add(mbb2);
                var mbb3 = new TaskDialogButton
                {
                    Text = "继续运行程序(不推荐)",
                    CommandLinkNote = "程序可能随时崩溃或内存泄漏",

                };
                mb.Buttons.Add(mbb3);
                var mbb4 = new TaskDialogButton
                {
                    ButtonType = ButtonType.Close
                };
                mb.Buttons.Add(mbb4);
                var res = mb.ShowDialog();
                if (res == mbb1)
                {
                    System.Windows.Forms.Clipboard.SetText($"{e.Exception}");
                    OpenIssue();
                    TaskBar.KillTaskBar();
                    Environment.Exit(0);
                }
                else if (res == mbb2)
                {
                    TaskBar.KillTaskBar();
                    Environment.Exit(0);
                }

            };

            // 捕获非 UI 线程未处理的异常
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {



                //MessageBox.Show($"发生错误：{e.ExceptionObject}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                var mb = new TaskDialog
                {
                    WindowTitle = "错误",
                    MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Error,
                    MainInstruction = "程序发生错误，您可将下方内容截图并上报错误",
                    Content = $"{e.ExceptionObject}",
                    ButtonStyle = TaskDialogButtonStyle.CommandLinks,


                };
                var mbb1 = new TaskDialogButton
                {

                    Text = "打开错误报告页面(推荐)",
                    CommandLinkNote = "将会自动复制错误信息到剪贴板,可能需要启动网络代理以进入Github",

                };
                mb.Buttons.Add(mbb1);
                var mbb2 = new TaskDialogButton
                {
                    Text = "退出程序",
                    CommandLinkNote = "退出程序以保证错误不再发生",

                };
                mb.Buttons.Add(mbb2);
                var mbb3 = new TaskDialogButton
                {
                    Text = "继续运行程序(不推荐)",
                    CommandLinkNote = "程序可能随时崩溃或内存泄漏",

                };
                mb.Buttons.Add(mbb3);
                var mbb4 = new TaskDialogButton
                {
                    ButtonType = ButtonType.Close
                };
                mb.Buttons.Add(mbb4);
                var res = mb.ShowDialog();
                if (res == mbb1)
                {
                    System.Windows.Forms.Clipboard.SetText($"{e.ExceptionObject}");
                    OpenIssue();
                    TaskBar.KillTaskBar();
                    Environment.Exit(0);
                }
                else if (res == mbb2)
                {
                    TaskBar.KillTaskBar();
                    Environment.Exit(0);
                }
                //KillTaskBar();
                //Environment.Exit(0);
            };

            // 捕获 Task 线程未处理的异常
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {


                var mb = new TaskDialog
                {
                    WindowTitle = "错误",
                    MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Error,
                    MainInstruction = "程序发生错误，您可将下方内容截图并上报错误",
                    Content = $"{e.Exception}",
                    ButtonStyle = TaskDialogButtonStyle.CommandLinks,


                };
                var mbb1 = new TaskDialogButton
                {

                    Text = "打开错误报告页面(推荐)",
                    CommandLinkNote = "将会自动复制错误信息到剪贴板,可能需要启动网络代理以进入Github",

                };
                mb.Buttons.Add(mbb1);
                var mbb2 = new TaskDialogButton
                {
                    Text = "退出程序",
                    CommandLinkNote = "退出程序以保证错误不再发生",

                };
                mb.Buttons.Add(mbb2);
                var mbb3 = new TaskDialogButton
                {
                    Text = "继续运行程序(不推荐)",
                    CommandLinkNote = "程序可能随时崩溃或内存泄漏",

                };
                mb.Buttons.Add(mbb3);
                var mbb4 = new TaskDialogButton
                {
                    ButtonType = ButtonType.Close
                };
                mb.Buttons.Add(mbb4);
                var res = mb.ShowDialog();
                if (res == mbb1)
                {
                    System.Windows.Forms.Clipboard.SetText($"{e.Exception}");
                    OpenIssue();
                    TaskBar.KillTaskBar();
                    Environment.Exit(0);
                }
                else if (res == mbb2)
                {
                    TaskBar.KillTaskBar();
                    Environment.Exit(0);
                }
            };
        }

        private static void OpenIssue()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Xiaowang0229/RocketLauncherRemake/issues/new",
                UseShellExecute = true
            });
        }
    }
}