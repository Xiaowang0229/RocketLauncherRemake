using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using FluentAvalonia.UI.Controls;
using RocketLauncherRemake.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

//using System.Windows;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake;

public partial class LaunchPage : UserControl
{
    private int GameIndex;
    private MainConfig config;

    public LaunchPage()
    {
        InitializeComponent();
        
    }
    public async void Page_Loaded(object sender,RoutedEventArgs e)
    {
        BackgroundVideo.IsVisible = false;
        BackgroundImage.IsVisible = false;
        config = JsonConfig.ReadConfig();
        GameIndex = Variables.GameIndex;
        var gi = config.GameInfos[GameIndex];
        Title.Text = gi.MainTitle;
        Title.Foreground = config.GameInfos[GameIndex].MainTitleFontColor.ToAvaloniaBrush() ;
        ShowName.Text = $"当前游戏:{gi.ShowName}";
        if (File.Exists($"{Variables.BackgroundPath}\\{config.GameInfos[GameIndex].HashCode}\\Background.mp4"))
        {
            BackgroundVideo.IsVisible = true;
            BackgroundVideo.Open($"{Variables.BackgroundPath}\\{config.GameInfos[GameIndex].HashCode}\\Background.mp4");
            BackgroundVideo.Play();
            
        }
        if(File.Exists($"{Variables.BackgroundPath}\\{config.GameInfos[GameIndex].HashCode}\\Background.png"))
        {
            BackgroundImage.IsVisible = true;
            BackgroundImage.Source = await ImageIconHelper.LoadFromFileAsync($"{Variables.BackgroundPath}\\{config.GameInfos[GameIndex].HashCode}\\Background.png");
        }
        if (!(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[GameIndex].HashCode}\\Background.png")) && !(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[GameIndex].HashCode}\\Background.mp4")))
        {
            try
            {
                BackgroundVideo.Stop();

                
            }
            catch { }
        }
        if (Variables.GameProcessStatus[GameIndex])
        {
            LaunchTile.Tag = "false";
            ChangeStartStopStatus(true);


            await GameController.WaitMonitingGameExitAsync(GameIndex);



            ChangeStartStopStatus(false);
            config = Json.ReadJson<MainConfig>(Variables.Configpath);

        }

        else if (Variables.GameProcessStatus[GameIndex] == false)
        {

            ChangeStartStopStatus(false);
        }

    }
    public void BackgroundImageVisible(bool value)
    {
        BackgroundImage.IsVisible = value;
    }

    public void BackgroundVideoVisible(bool value)
    {
        BackgroundVideo.IsVisible = value;
    }

    public void BackgroundVideoPlayStop(bool value)
    {
        if(value)
        {
            BackgroundVideo.Play();
            
        }
        else
        {
            BackgroundVideo.Pause();
        }
    }

    private async void Background_MediaEnded(object sender, EventArgs e)
    {
        BackgroundVideo.Pause();
        BackgroundVideo.Seek(0.0f);
        BackgroundVideo.Play();
    }

    private async void NewLaunchTile_Click(object sender, RoutedEventArgs e)
    {
        if (LaunchTile.Tag == "true")//处理结束逻辑
        {

            var proc = Variables.GameProcess[GameIndex];
            proc.Kill();
            ChangeStartStopStatus(false);
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
        }
        else if (LaunchTile.Tag == "false")//处理开始逻辑
        {
            try
            {
                ChangeStartStopStatus(true);
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[GameIndex].HashCode}\\Background.mp4"))
                {
                    BackgroundVideo.Pause();
                }
                GameController.StartMonitingGameStatus(GameIndex);
                await GameController.WaitMonitingGameExitAsync(GameIndex);
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[GameIndex].HashCode}\\Background.mp4"))
                {
                    BackgroundVideo.Play();
                }

                ChangeStartStopStatus(false);
            }
            catch (InvalidOperationException)
            {

            }
            catch (OperationCanceledException)
            {

            }
            /*catch (Exception ex)
            {
                Variables._MainWindow.ShowMessageAsync("游戏启动时错误", $"{ex.Message}");
            }*/
        }

    }

    private async void ChangeStartStopStatus(bool ChangeMode)
    {
        config = Json.ReadJson<MainConfig>(Variables.Configpath);
        if (ChangeMode)
        {

            LaunchTitle.Text = "结束游戏";
            LaunchTile.Tag = "true";
            await Task.Delay(TimeSpan.FromSeconds(0.5));
        }
        else
        {

            LaunchTitle.Text = "开始游戏";
            LaunchTile.Tag = "false";
        }
    }



}