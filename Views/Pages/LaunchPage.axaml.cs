using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;

namespace RocketLauncherRemake;

public partial class LaunchPage : UserControl
{
    private int GameIndex;
    public LaunchPage()
    {
        InitializeComponent();

    }
    private async void Page_Loaded(object sender,RoutedEventArgs e)
    {
        BackgroundVideo.MediaFailed += (s, e) =>
        {
            System.Windows.MessageBox.Show($"{e.ErrorException}");
        };
        BackgroundVideo.Open(new System.Uri("C:\\Users\\wangj\\Videos\\【补档】858路-百鬼夜行终将被孤勇者征服.mp4",System.UriKind.Absolute));
        await BackgroundVideo.Play();

        
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
        BackgroundImage.IsVisible = value;
    }

    
}