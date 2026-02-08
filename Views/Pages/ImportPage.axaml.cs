using Avalonia.Controls;
using RocketLauncherRemake.Utils;

namespace RocketLauncherRemake;

public partial class ImportPage : UserControl
{
    public ImportPage()
    {
        InitializeComponent();
        RootImage.Source = ImageIconHelper.ToAvaloniaImageSource(AppResource.DefaultGameIcon);
    }
}