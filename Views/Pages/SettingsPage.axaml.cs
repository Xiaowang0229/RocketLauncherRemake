using Avalonia.Controls;
using Avalonia.Interactivity;
using RocketLauncherRemake.Utils;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake;

public partial class SettingsPage : UserControl
{
    MainConfig config = JsonConfig.ReadConfig();
    public SettingsPage()
    {
        InitializeComponent();
        
    }
    private void Page_Loaded(object sender,RoutedEventArgs e)
    {
        config = JsonConfig.ReadConfig();
        LaunchWithMinize.IsChecked = config.LaunchWithMinize;
        StartUpCheckUpdate.IsChecked = config.StartUpCheckUpdate;
    }
    private void StartUpCheckUpdate_Toggled(object sender, RoutedEventArgs e)
    {

        if (StartUpCheckUpdate.IsChecked == true)
        {
            config.StartUpCheckUpdate = true;
            Json.WriteJson(Variables.Configpath, config);

        }
        else
        {
            config.StartUpCheckUpdate = false;
            Json.WriteJson(Variables.Configpath, config);

        }
    }

    private void LaunchWithMinize_Toggled(object sender, RoutedEventArgs e)
    {
        if (LaunchWithMinize.IsChecked == true)
        {
            config.LaunchWithMinize = true;
            Json.WriteJson(Variables.Configpath, config);
        }
        else
        {
            config.LaunchWithMinize = false;
            Json.WriteJson(Variables.Configpath, config);
        }
    }

    private async void ResetConfig_Click(object sender, RoutedEventArgs e)
    {
        var qdr = await Variables._MainWindow.ShowMessageAsync("警告", "确定要重置所有配置项（包含个性化设置，主题设置和所有已经添加的游戏等）吗？此操作不可逆");
        if (qdr)
        {
            JsonConfig.InitalizeConfig(true);
            WindowHelper.Restart();
        }
    }

    private void ImportGame_Click(object sender, RoutedEventArgs e)
    {
        Variables._MainWindow.RootFrame.Navigate(typeof(ImportPage));
    }

    private void ManageGame_Click(object sender, RoutedEventArgs e)
    {
        if (config.GameInfos.Count == 0)
        {
            Variables._MainWindow.RootFrame.Navigate(typeof(EmptyGame));
        }
        else
        {
            Variables._MainWindow.RootFrame.Navigate(typeof(ManagePage));
        }
    }
}