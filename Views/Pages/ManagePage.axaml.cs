using Avalonia.Controls;
using Avalonia.Interactivity;
using RocketLauncherRemake.Utils;
using System;
using System.Diagnostics;
using System.IO;
using Xiaowang0229.JsonLibrary;
using HashCode = RocketLauncherRemake.Utils.HashCode;

namespace RocketLauncherRemake;

public partial class ManagePage : UserControl
{
    private LaunchConfig GameConfig = null;
    private MainConfig config;
    private bool Validate = false;
    public ManagePage()
    {
        InitializeComponent();

    }


    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {


        config =  JsonConfig.ReadConfig();

        var ct = new StackPanel { Margin = new Avalonia.Thickness(5) };
        ct.Children.Add(new TextBlock { Text="请选择要管理的项目:"});
        var rbsp = new StackPanel { Margin = new Avalonia.Thickness(5) };

        DelCostumIcon.IsVisible = true;
        UseAppIcon.IsVisible = true;
        for(int i = 0;i<config.GameInfos.Count;i++)
        {
            int index = i;
            var rb = new RadioButton
            {
                Content = $"{config.GameInfos[i].ShowName}"
            };
            rb.Click += (s, e) =>
            {
                GameConfig = config.GameInfos[index];
                Validate = true;
            };
            rbsp.Children.Add(rb);
        }
        ct.Children.Add(rbsp);

        var result = await Variables._MainWindow.ShowMessageAsync("提示", ct);

        if(result == false || Validate == false)
        {
            Page_Unloaded();
            Variables._MainWindow.RootFrame.Navigate(typeof(SettingsPage));
        }

        if (result == true)
        {

            RootTitle.Text = "管理";
            RootImage.Source = await ImageIconHelper.LoadFromFileAsync($"{Environment.CurrentDirectory}\\Backgrounds\\{GameConfig.HashCode}\\Icon.png");
            LaunchPath.Text = $"已选中文件:{GameConfig.Launchpath}";
            ProgramName.Text = GameConfig.ShowName;
            TitleContent.Text = GameConfig.MainTitle;
            LaunchArgs.Text = GameConfig.Arguments;

            if(File.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4") || File.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png"))
            {
                DelBackground.IsVisible = true;
            }
            else
            {
                DelBackground.IsVisible = false;
            }
        }
        
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Page_Unloaded();
        Variables._MainWindow.RootFrame.Navigate(typeof(SettingsPage));
    }

    private async void OK_Click(object sender, RoutedEventArgs e)
    {
        if (GameConfig.MainTitle != null && GameConfig.ShowName != null && GameConfig.Launchpath != null)
        {
            
            {
                Variables._MainWindow.Tip.IsVisible = true;
                if (BackgroundPath.IsVisible)
                {
                    if (File.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4"))
                    {
                        File.Delete($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4");
                    }
                    if (File.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png"))
                    {
                        File.Delete($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png");
                    }
                    if (Path.GetExtension(BackgroundPath.Text.Substring(6)) == ".mp4")
                    {
                        await FileHelper.CopyFileAsync(BackgroundPath.Text.Substring(6), $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4");
                    }
                    else
                    {
                        await FileHelper.CopyFileAsync(BackgroundPath.Text.Substring(6), $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png");
                    }
                }
                if (CostumIcons.IsVisible)
                {
                    await FileHelper.CopyFileAsync(CostumIcons.Text.Substring(6), $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Icon.png");
                }
                else
                {
                    ImageIconHelper.ConvertToPngAndSave(AppResource.DefaultGameIcon, $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Icon.png");
                }
                config.GameInfos[Utils.HashCode.FindHashcodeinGameinfosint(config, GameConfig.HashCode)] = GameConfig;
                config.WriteConfig();
                

                try
                {
                    Variables.GameProcess[HashCode.FindHashcodeinGameinfosint(config, GameConfig.HashCode)].Kill();
                    Variables.GameProcess[HashCode.FindHashcodeinGameinfosint(config, GameConfig.HashCode)].Close();
                }
                catch { }
                Variables.GameProcess[HashCode.FindHashcodeinGameinfosint(config, GameConfig.HashCode)].StartInfo = new ProcessStartInfo
                {
                    FileName = GameConfig.Launchpath,
                    Arguments = GameConfig.Arguments,
                    UseShellExecute = true
                };
                Variables.GameProcessStatus[HashCode.FindHashcodeinGameinfosint(config, GameConfig.HashCode)] = false;
                Variables._MainWindow.Tip.IsVisible = false;
                Page_Unloaded();
                Variables._MainWindow.RefreshNavigationList();
                Variables._MainWindow.RootFrame.Navigate(typeof(SettingsPage));
            }
        }
        else
        {
            await Variables._MainWindow.ShowMessageAsync("提示", "请将必填项内的所有内容填写完整");
        }
    }

    private void LaunchPath_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择可执行文件",
            Filter = "可执行文件(*.exe)|*.exe",
            Multiselect = false
        };
        if (dialog.ShowDialog() == true)
        {
            LaunchPath.IsVisible = true;
            LaunchPath.Text = $"已选中文件:{dialog.FileName}";
            GameConfig.Launchpath = dialog.FileName;
            if (string.IsNullOrEmpty(ProgramName.Text))
            {
                ProgramName.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
            if (string.IsNullOrEmpty(TitleContent.Text))
            {
                TitleContent.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
        }
    }

    private void ProgramName_TextChanged(object sender, RoutedEventArgs e)
    {
        GameConfig.ShowName = ProgramName.Text;
    }

    private void TitleContent_TextChanged(object sender, RoutedEventArgs e)
    {
        GameConfig.MainTitle = TitleContent.Text;
    }

    private void LaunchArgs_TextChanged(object sender, RoutedEventArgs e)
    {
        GameConfig.Arguments = LaunchArgs.Text;
    }

    private void Font_Click(object sender, RoutedEventArgs e)
    {
        var fontdialog = new System.Windows.Forms.FontDialog();
        var colorDialog = new System.Windows.Forms.ColorDialog();

        if (fontdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            GameConfig.MaintitleFontName = new System.Windows.Media.FontFamily(fontdialog.Font.FontFamily.Name);

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameConfig.MainTitleFontColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(
                colorDialog.Color.A,
                colorDialog.Color.R,
                colorDialog.Color.G,
                colorDialog.Color.B));






            }


        }
    }

    private async void CostumIcons_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择文件",
            Filter = "图标文件(*.png;*.webp;*.bmp;*.jpg;*.jpeg;*.ico)|*.png;*.webp;*.bmp;*.jpg;*.jpeg;*.ico",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Multiselect = false
        };
        if (dialog.ShowDialog() == true)
        {
            CostumIcons.IsVisible = true;
            CostumIcons.Text = $"已选中文件:{dialog.FileName}";
            RootImage.Source = await ImageIconHelper.LoadFromFileAsync(dialog.FileName);
            //substring取6
        }
    }

    private async void DelBackground_Click(object sender,RoutedEventArgs e)
    {
        if(await Variables._MainWindow.ShowMessageAsync("提示","请问是否删除背景? 此操作不可逆!"))
        {
            if(File.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4"))
            {
                File.Delete($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4");
            }
            if (File.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png"))
            {
                File.Delete($"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png");
            }
            Variables._MainWindow.ShowMessageAsync("提示", "背景已经恢复为默认");
            DelBackground.IsVisible = false;
        }
    }

    private async void Background_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择文件",
            Filter = "背景文件(*.png;*.webp;*.bmp;*.jpg;*.jpeg;*.mp4)|*.png;*.webp;*.bmp;*.jpg;*.jpeg;*.mp4",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Multiselect = false
        };
        if (dialog.ShowDialog() == true)
        {
            BackgroundPath.IsVisible = true;
            BackgroundPath.Text = $"已选中文件:{dialog.FileName}";
            //RootImage.Source = await ImageIconHelper.LoadFromFileAsync(dialog.FileName);
            //substring取6
        }

    }

    public void Page_Unloaded()
    {
        LaunchPath.IsVisible = false;
        LaunchPath.Text = "";
        ProgramName.Text = "";
        TitleContent.Text = "";
        LaunchArgs.Text = "";
        CostumIcons.IsVisible = false;
        CostumIcons.Text = "";
        BackgroundPath.IsVisible = false;
        BackgroundPath.Text = "";
        GameConfig = new LaunchConfig();
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if(await Variables._MainWindow.ShowMessageAsync("警告","操作不可逆,请问是否删除该项目?"))
        {
            var deleteitemindex = GameConfig.HashCode;
            config.GameInfos.RemoveAt(HashCode.FindHashcodeinGameinfosint(config, deleteitemindex));
            Variables.GameProcess.RemoveAt(HashCode.FindHashcodeinGameinfosint(config, deleteitemindex));
            Variables.GameProcessStatus.RemoveAt(HashCode.FindHashcodeinGameinfosint(config, deleteitemindex));
            Variables.PlayingTimeintList.RemoveAt(HashCode.FindHashcodeinGameinfosint(config, deleteitemindex));
            Variables.PlayingTimeRecorder.RemoveAt(HashCode.FindHashcodeinGameinfosint(config, deleteitemindex));
            Json.WriteJson(Variables.Configpath, config);
            if(Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{GameConfig.HashCode}\\"))
            {
                Directory.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{GameConfig.HashCode}\\", true);
            }
            Page_Unloaded();
            Variables._MainWindow.RefreshNavigationList();
            Variables._MainWindow.RootFrame.Navigate(typeof(SettingsPage));
        }

    }

    private async void DelCostumIcon_Click(object sender, RoutedEventArgs e)
    {
        if (await Variables._MainWindow.ShowMessageAsync("警告", "确定要恢复默认图标吗?此操作不可逆"))
        {
            if (File.Exists($"{Environment.CurrentDirectory}\\{GameConfig.HashCode}\\Icon.png"))
            {
                File.Delete($"{Environment.CurrentDirectory}\\{GameConfig.HashCode}\\Icon.png");
            }
            ImageIconHelper.ExtractAndSave256Icon(GameConfig.Launchpath, $"{Environment.CurrentDirectory}\\{GameConfig.HashCode}\\Icon.png");
            Variables._MainWindow.ShowMessageAsync("提示", "图标已经恢复为默认");
            DelCostumIcon.IsVisible = false;
        }
    }

    private async void UseAppIcon_Click(object sender, RoutedEventArgs e)
    {
        if (await Variables._MainWindow.ShowMessageAsync("警告", "确定要使用程序内的图标吗?此操作不可逆"))
        {
            if(File.Exists($"{Environment.CurrentDirectory}\\{GameConfig.HashCode}\\Icon.png"))
            {
                File.Delete($"{Environment.CurrentDirectory}\\{GameConfig.HashCode}\\Icon.png");
            }
            ImageIconHelper.ExtractAndSave256Icon(GameConfig.Launchpath, $"{Environment.CurrentDirectory}\\{GameConfig.HashCode}\\Icon.png");
            UseAppIcon.IsVisible = false;
        }
    }
}