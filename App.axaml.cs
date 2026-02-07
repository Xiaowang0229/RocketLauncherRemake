using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Windowing;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using Unosquare.FFME;
using Windows.UI.Core;
using Xiaowang0229.JsonLibrary;


namespace RocketLauncherRemake
{
    public partial class App : Avalonia.Application
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

            //Tools.RegisterGlobalExceptionHandlers();

            Library.FFmpegDirectory = Environment.CurrentDirectory + "\\FFmpeg";
            Library.LoadFFmpeg();
            Variables.VersionLog = Tools.ReadEmbeddedMarkdown("RocketLauncherRemake.LocalLog.md");
            Variables.EULAString = Tools.ReadEmbeddedMarkdown("RocketLauncherRemake.EULA.md");

            if (!File.Exists(Variables.Configpath))
            {
                Tools.InitalizeConfig(true);
            }
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            
            //¶ÁÈ¡Âß¼­

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
                Tools.CheckUpdate();

            }
            return Variables._MainWindow;
        }
    }
}