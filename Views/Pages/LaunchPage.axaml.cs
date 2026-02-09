using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using FluentAvalonia.UI.Controls;
using RocketLauncherRemake.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

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
        config = JsonConfig.ReadConfig();
        GameIndex = Variables.GameIndex;
        var gi = config.GameInfos[GameIndex];
        Title.Text = gi.MainTitle;
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

        /*BackgroundVideo.MediaFailed += (s, e) =>
        {
            System.Windows.MessageBox.Show($"{e.ErrorException}");
        };
        BackgroundVideo.Open(new System.Uri("C:\\Users\\wangj\\Videos\\【补档】858路-百鬼夜行终将被孤勇者征服.mp4",System.UriKind.Absolute));
        await BackgroundVideo.Play();*/

        
    }
    public void BackgroundImageVisible(bool value)
    {
        BackgroundImage.IsVisible = value;
    }

    public void BackgroundVideoVisible(bool value)
    {
        //BackgroundVideo.IsVisible = value;
    }

    public void BackgroundVideoPlayStop(bool value)
    {
        if(value)
        {

        }
        else
        {

        }
    }

    private void Background_MediaEnded(object sender, EventArgs e)
    {

    }



}