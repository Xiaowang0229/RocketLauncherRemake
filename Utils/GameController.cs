using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static List<bool> GameProcessStatus = new List<bool>();
        public static List<Process> GameProcess = new List<Process>();
        public static List<DispatcherTimer> PlayingTimeRecorder = new List<DispatcherTimer>();
        public static List<long> PlayingTimeintList = new List<long>();
        public static bool MainWindowHideStatus = false;
        public static CancellationTokenSource LaunchCTS = new CancellationTokenSource();
    }
    public static class GameController
    {
        public async static void StartMonitingGameStatus(int index)
        {
            
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
            TaskBar.InitializeTaskBarContentMenu();
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
            Variables._MainWindow.RootNavi.SelectedItem = Variables._MainWindow.RootNavi.MenuItems[index];
            var win = Variables._MainWindow;
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            Variables.PlayingTimeRecorder[index].Stop();
            var time = Variables.PlayingTimeintList[index];
            var totaltime = config.GameInfos[index].GamePlayedMinutes + time;
            config.GameInfos[index].GamePlayedMinutes = totaltime;
            Json.WriteJson(Variables.Configpath, config);
            
                var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"游戏时长：{time} 分钟,退出码：{Variables.GameProcess[index].ExitCode} ").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
                toast0.Show();







            TaskBar.InitializeTaskBarContentMenu();
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
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Background.mp4"))
                {
                    launchpage.BackgroundImageVisible(false);
                    launchpage.BackgroundVideoVisible(true);
                    launchpage.BackgroundVideoPlayStop(true);
                }

            }
            win.Topmost = true;
            win.Topmost = false;

        }
    }
}
