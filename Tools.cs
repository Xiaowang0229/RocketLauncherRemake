global using Application = System.Windows.Application;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TsudaKageyu;
using Xiaowang0229.JsonLibrary;
using Color = System.Windows.Media.Color;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using TaskDialog = Ookii.Dialogs.Wpf.TaskDialog;
using TaskDialogButton = Ookii.Dialogs.Wpf.TaskDialogButton;

namespace RocketLauncherRemake
{
    public static class Variables //变量集
    {
        public readonly static string Version = "4.0.0-Alpha.1";
        public static string ApplicationTitle = $"Rocket Launcher {Version}";
        public readonly static string Configpath = Environment.CurrentDirectory + @"\Config.json";
        public static List<Process> GameProcess = new List<Process>();
        public static TaskbarIcon RootTaskBarIcon;
        public static ContextMenu TaskBarMenu = new ContextMenu();
        public static List<bool> GameProcessStatus = new List<bool>();
        public static List<DispatcherTimer> PlayingTimeRecorder = new List<DispatcherTimer>();
        public static List<long> PlayingTimeintList = new List<long>();
        public static string VersionLog;
        public static string EULAString;
        public static bool MainWindowHideStatus = false;
        public static CancellationTokenSource LaunchCTS = new CancellationTokenSource();
        public static CancellationTokenSource UpdateCTS = new CancellationTokenSource();
        public static MainWindow _MainWindow = new MainWindow();
    }

    public static class Tools //函数集
    {

        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Environment.Exit(0);
        }
        public static ImageSource ConvertByteArrayToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;

            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
        public static string GetColorName(Color color)
        {
            foreach (PropertyInfo prop in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (prop.PropertyType == typeof(Color))
                {
                    Color namedColor = (Color)prop.GetValue(null);
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

        public static string RandomHashGenerate(int byteLength = 16)
        {
            byte[] bytes = new byte[byteLength];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes);
        }

        public static void InitalizeConfig(bool Confirm)
        {
            var config = new MainConfig
            {
                Username = "Administrator",
                AutoStartUp = false,
                StartUpCheckUpdate = true,
                ChangeThemeWithSystem = false,
                LaunchWithMinize = true,
                GameInfos = new List<LaunchConfig>()
            };
            Json.WriteJson(Variables.Configpath, config);
            //ConvertToPngAndSave(ApplicationResources.UserIcon, Environment.CurrentDirectory+@"\Head.png");
        }
        public static string ReadEmbeddedMarkdown(string resourceName)
        {
            // 获取当前执行的程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 获取嵌入资源的流
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Resource {resourceName} not found.");

                // 使用 StreamReader 读取流中的文本
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static int FindHashcodeinGameinfosint(MainConfig config, string hashcode)
        {
            for (int i = 0; i < config.GameInfos.Count; i++)
            {
                if (config.GameInfos[i].HashCode == hashcode)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void IntializeTaskbar()
        {
            MainConfig config = new MainConfig();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            //本体初始化
            Variables.RootTaskBarIcon = new TaskbarIcon();
            Variables.RootTaskBarIcon.IconSource = ConvertByteArrayToImageSource(AppResource.ApplicationIcon);
            Variables.RootTaskBarIcon.ToolTipText = $"Rocket Launcher 主程序";

            //列表项初始化
            var tbcm = new System.Windows.Controls.ContextMenu();
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
            var tbcm = new System.Windows.Controls.ContextMenu();
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
        public async static void StartMonitingGameStatus(int index)
        {
            ToastNotificationManagerCompat.History.Clear();
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var proc = Variables.GameProcess[index];
            proc.Start();
            Variables.GameProcessStatus[index] = true;
            if (config.LaunchWithMinize)
            {
                var win = Variables._MainWindow;
                win.Hide();
                Variables.MainWindowHideStatus = true;
            }
            Tools.InitializeTaskBarContentMenu();
            Variables.PlayingTimeRecorder[index].Start();
            var toast = new ToastContentBuilder().AddText("程序已启动").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"进程监测已开启").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
            toast.Show();




        }
        public static async Task WaitMonitingGameExitAsync(int index)
        {
            Variables.LaunchCTS.Cancel();
            Variables.LaunchCTS = new CancellationTokenSource();
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var proc = Variables.GameProcess[index];




            //await proc.WaitForExitAsync(Variables.LaunchCTS.Token);
            while (!proc.HasExited)
            {
                await Task.Delay(100);
            }
            StopMonitingGameStatus(index);








        }
        private static void StopMonitingGameStatus(int index)
        {
            var win = Variables._MainWindow;
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            Variables.PlayingTimeRecorder[index].Stop();
            var time = Variables.PlayingTimeintList[index];
            var totaltime = config.GameInfos[index].GamePlayedMinutes + time;
            config.GameInfos[index].GamePlayedMinutes = totaltime;
            Json.WriteJson(Variables.Configpath, config);
            if (Variables.GameProcess[index].ExitCode == 0)
            {
                var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"游戏时长：{time} 分钟,退出码：{Variables.GameProcess[index].ExitCode} (正常退出)").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
                toast0.Show();

            }
            else if (Variables.GameProcess[index].ExitCode == -1)
            {
                var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"游戏时长：{time} 分钟,退出码：{Variables.GameProcess[index].ExitCode} (强制退出)").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
                toast0.Show();
            }
            else
            {

                var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"游戏时长：{time} 分钟,退出码：{Variables.GameProcess[index].ExitCode} (可能为异常退出)").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
                toast0.Show();
            }





            Tools.InitializeTaskBarContentMenu();
            Variables.GameProcessStatus[index] = false;
            Variables.PlayingTimeintList[index] = 0;
            Variables.MainWindowHideStatus = false;
            var currentPage = win.RootFrame.Content;
            win.Show();
            win.WindowState = Avalonia.Controls.WindowState.Normal;
            if (currentPage is LaunchPage launchpage)
            {
                config = Json.ReadJson<MainConfig>(Variables.Configpath);
                if (win.RootNavi.MenuItems[index] == win.RootNavi.SelectedItem)
                {
                    //launchpage.NewGameTimeBlock.Content = $"游戏总时长：{totaltime}分钟";
                }
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                {
                    launchpage.BackgroundImageVisible(false);
                    launchpage.BackgroundVideoVisible(true);
                    launchpage.BackgroundVideoPlayStop(true);
                }

            }
            win.Topmost = true;
            win.Topmost = false;

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
        public static void ExtractExeIconToPng(string exePath, string pngPath)
        {
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                throw new FileNotFoundException("EXE 文件不存在", exePath);

            if (string.IsNullOrWhiteSpace(pngPath))
                throw new ArgumentException("PNG 输出路径不能为空");

            // 使用 IconExtractor.dll 提取图标资源
            var extractor = new IconExtractor(exePath);

            if (extractor.Count == 0)
                throw new InvalidOperationException($"指定的 EXE 文件不包含任何图标: {exePath}");

            // 通常第 0 个图标就是主图标（最大、最清晰的那个）
            // GetIcon(0) 返回的是包含所有尺寸变体（包括 256x256）的完整 Icon 对象
            using (System.Drawing.Icon fullIcon = extractor.GetIcon(0))
            {
                // 确保输出目录存在
                string dir = Path.GetDirectoryName(pngPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // 优先尝试提取 256x256 高清尺寸（现代 Windows 程序几乎都支持）
                try
                {
                    using (System.Drawing.Icon largeIcon = new System.Drawing.Icon(fullIcon, 256, 256))
                    using (Bitmap bmp = largeIcon.ToBitmap())  // 自动保留透明度
                    {
                        bmp.Save(pngPath, ImageFormat.Png);
                        return;  // 成功提取 256x256，直接返回
                    }
                }
                catch
                {
                    // 如果没有 256x256 尺寸，回退到图标自带的最大尺寸
                    // ToBitmap() 会选择最佳可用尺寸并保留 Alpha 通道
                }

                using (Bitmap bmp = fullIcon.ToBitmap())
                {
                    bmp.Save(pngPath, ImageFormat.Png);
                }
            }
        }
        public static void RefreshAllImageCaches(DependencyObject parent)
        {
            if (parent == null) return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);





                if (child is System.Windows.Controls.Image imageControl)
                {
                    if (imageControl.Source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
                    {
                        // 创建新 BitmapImage，忽略缓存并立即加载
                        BitmapImage newBitmap = new BitmapImage();
                        newBitmap.BeginInit();
                        newBitmap.UriSource = bitmapImage.UriSource;
                        newBitmap.CacheOption = BitmapCacheOption.OnLoad;           // 立即加载以释放文件锁（如需）
                        newBitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // 关键：忽略现有缓存
                        newBitmap.EndInit();

                        // 可选：Freeze 以提高性能（多线程安全）
                        if (newBitmap.CanFreeze)
                        {
                            newBitmap.Freeze();
                        }

                        imageControl.Source = newBitmap;
                    }
                }

                // 递归处理子控件
                RefreshAllImageCaches(child);
            }
        }
        public static bool CheckTime(string hhmm)
        {
            if (hhmm == DateTime.Now.ToString("HH:mm"))
            {
                return true;
            }

            return false;
        }

        public async static Task CheckUpdate(bool IsSameVersionShowDialog = false, bool ShowException = false)
        {
            var client = new HttpClient();
            try
            {
                var content = await client.GetStringAsync("https://gitee.com/xiaowangupdate/update-service/raw/master/MultiGameLauncher", Variables.UpdateCTS.Token);
                var updcfg = Json.ReadJson<UpdateConfig>(content);
                if (updcfg.UpdateVersion != Variables.Version)
                {



                    var filname = Tools.RandomHashGenerate();
                    var win = Variables._MainWindow;

                    //var results = await win.ShowMessageAsync("更新可用", $"当前版本:{Variables.Version},最新版本:{updcfg.UpdateVersion},请问是否更新？", MessageDialogStyle.AffirmativeAndNegative, settings);
                    var results = await win.ShowMessageAsync("更新可用", $"当前版本:{Variables.Version},最新版本:{updcfg.UpdateVersion},请问是否更新？");
                    if (results == true)
                    {
                        if (File.Exists(Path.GetTempPath() + "\\" + filname + ".exe"))
                        {
                            File.Delete(Path.GetTempPath() + "\\" + filname + ".exe");
                        }
                        try
                        {

                            File.Copy($"{Environment.CurrentDirectory}\\UpdateAPI.exe", Path.GetTempPath() + "\\" + filname + ".exe");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = $"{Path.GetTempPath()}\\{filname}.exe",
                                Arguments = $"\"{updcfg.UpdateLink}\" \"{Environment.ProcessPath}\"",
                                UseShellExecute = true
                            });
                            Tools.KillTaskBar();
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            if (ShowException)
                            {
                                await Variables._MainWindow.ShowMessageAsync("更新时发现错误", $"{ex.Message}");
                            }
                            Environment.Exit(0);
                        }

                    }




                }
                else if (updcfg.UpdateVersion == Variables.Version && IsSameVersionShowDialog)
                {
                    Variables._MainWindow.ShowMessageAsync("提示", $"当前版本已是最新版本:{updcfg.UpdateVersion}！");
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                if (ShowException)
                {
                    Variables._MainWindow.ShowMessageAsync("检测更新时发现错误", $"{ex.Message}");
                }
            }

        }


        public static void RegisterGlobalExceptionHandlers()
        {
            // 捕获 UI 线程未处理的异常
            Application.Current.DispatcherUnhandledException += (s, e) =>
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
                    KillTaskBar();
                    Environment.Exit(0);
                }
                else if (res == mbb2)
                {
                    KillTaskBar();
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
                    KillTaskBar();
                    Environment.Exit(0);
                }
                else if (res == mbb2)
                {
                    KillTaskBar();
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
                    KillTaskBar();
                    Environment.Exit(0);
                }
                else if (res == mbb2)
                {
                    KillTaskBar();
                    Environment.Exit(0);
                }
            };
        }

        private static void OpenIssue()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Xiaowang0229/MultiGameLauncher/issues/new?template=%F0%9F%94%B4%E6%BC%8F%E6%B4%9E.md",
                UseShellExecute = true
            });
        }




        public async static Task CopyFileAsync(string source, string dest)
        {
            if (Variables._MainWindow != null)
            {

                Variables._MainWindow.Tip.IsVisible = true;

            }

            await Task.Run(() => File.Copy(source, dest, overwrite: true));
            if (Variables._MainWindow != null)
            {
                Variables._MainWindow.Tip.IsVisible = false;

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
            ;

        }


    }

    public class LaunchConfig
    {

        public string HashCode { get; set; }
        public string ShowName { get; set; }
        public string MainTitle { get; set; }
        public System.Windows.Media.FontFamily MaintitleFontName { get; set; }
        public System.Windows.Media.Brush MainTitleFontColor { get; set; }
        public long GamePlayedMinutes { get; set; }
        public string Launchpath { get; set; }
        public string Arguments { get; set; }



    }
    public class MainConfig
    {

        //用户名
        public string Username { get; set; }

        //自动更新
        public bool StartUpCheckUpdate { get; set; }

        //开机自启
        public bool AutoStartUp { get; set; }

        //主题跟随系统
        public bool ChangeThemeWithSystem { get; set; }

        //游戏配置项，勿动
        public List<LaunchConfig> GameInfos { get; set; }

        //游戏启动时最小化窗口
        public bool LaunchWithMinize { get; set; }



    }

    public class UpdateConfig
    {
        public string UpdateVersion { get; set; }
        public string UpdateLog { get; set; }
        public string UpdateLink { get; set; }


    }



}

