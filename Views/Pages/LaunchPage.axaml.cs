using Avalonia.Controls;

namespace RocketLauncherRemake;

public partial class LaunchPage : UserControl
{

    public LaunchPage()
    {
        InitializeComponent();
    }

    public void BackgroundImageVisible(bool value)
    {
        BackgroundImage.IsVisible = value;
    }

    public void BackgroundVideoVisible(bool value)
    {
        BackgroundImage.IsVisible = value;
    }

    public void BackgroundVideoPlayStop(bool value)
    {
        BackgroundImage.IsVisible = value;
    }
}