using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using RocketLauncherRemake.Utils;

namespace RocketLauncherRemake
{
    public partial class MainWindow : AppWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            this.TitleBar.Height = 48;
            this.Icon = ImageIconHelper.ToAvaloniaImageSource(AppResource.ApplicationImage);




        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*for(int i = 0;i<=;i++)
            {

            }*/
        }

        private void RootNavi_SelectionChanged(object sender, NavigationViewSelectionChangedEventArgs e)
        {
            if (RootNavi.SelectedItem == RootNavi.FooterMenuItems[0])
            {
                RootFrame.Navigate(typeof(SettingsPage));
            }
            else if (RootNavi.SelectedItem == RootNavi.FooterMenuItems[1])
            {
                RootFrame.Navigate(typeof(AboutPage));
            }
        }
    }
}