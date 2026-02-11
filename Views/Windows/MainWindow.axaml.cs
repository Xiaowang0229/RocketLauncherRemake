using Avalonia.Animation;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Windowing;
using RocketLauncherRemake.Utils;
using System;
using System.Threading.Tasks;


namespace RocketLauncherRemake
{
    public partial class MainWindow : AppWindow
    {
        MainConfig config = JsonConfig.ReadConfig();
        public MainWindow()
        {
            InitializeComponent();
            this.TitleBar.Height = 48;
            this.Icon = ImageIconHelper.ToAvaloniaImageSource(AppResource.ApplicationImage);
            this.ExtendClientAreaToDecorationsHint = true;
            this.TitleBar.ExtendsContentIntoTitleBar = true;
            TitleBarIcon.Source = ImageIconHelper.ToAvaloniaImageSource(AppResource.ApplicationImage);


        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(config.GameInfos.Count == 0)
            {
                RootFrame.Navigate(typeof(EmptyGame));
            }
            else
            {
                foreach(var i in config.GameInfos)
                {
                    RootNavi.MenuItems.Add(new NavigationViewItem
                    {
                        Content = i.ShowName,
                        IconSource = new ImageIconSource
                        {
                            Source = await ImageIconHelper.LoadFromFileAsync($"{Variables.BackgroundPath}\\{i.HashCode}\\Icon.png")
                        }
                    });
                }
                RootNavi.SelectedItem = RootNavi.MenuItems[0];
                
            }
        }

        private void RootNavi_SelectionChanged(object sender, NavigationViewSelectionChangedEventArgs e)
        {
            config = JsonConfig.ReadConfig();
            if (RootNavi.SelectedItem == RootNavi.FooterMenuItems[0])
            {
                RootFrame.Navigate(typeof(SettingsPage));
            }
            else if (RootNavi.SelectedItem == RootNavi.FooterMenuItems[1])
            {
                RootFrame.Navigate(typeof(AboutPage));
            }
            for (int i = 0; i < RootNavi.MenuItems.Count;i++)
            {
                if(RootNavi.SelectedItem == RootNavi.MenuItems[i])
                {
                    Variables.GameIndex = i;
                    RootFrame.Navigate(typeof(LaunchPage));
                    break;
                }
            }
            
        }

        private void Frame_Pre_Navigating(object sender, NavigationEventArgs e)
        {
            if(RootFrame.Content is LaunchPage L)
            {
                L.Page_Loaded(null,null);
            }
        }

        public async void RefreshNavigationList()
        {
            config = JsonConfig.ReadConfig();
            RootNavi.MenuItems.Clear();
            if(config.GameInfos.Count != 0)
            {
                foreach (var i in config.GameInfos)
                {
                    RootNavi.MenuItems.Add(new NavigationViewItem
                    {
                        Content = i.ShowName,
                        IconSource = new ImageIconSource
                        {
                            Source = await ImageIconHelper.LoadFromFileAsync($"{Variables.BackgroundPath}\\{i.HashCode}\\Icon.png")
                        }
                    });
                }
            }
        }

        public async void ShowInfoBar(string Title,string Message)
        {
            RootInfoBar.IsOpen = false;
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            RootInfoBar.Title = Title;
            RootInfoBar.Message = Message;
            RootInfoBar.IsOpen = true;
            
            

        }
    }
}