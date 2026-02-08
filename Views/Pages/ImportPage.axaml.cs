using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Win32;
using RocketLauncherRemake.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Threading;
using Windows.Media.Capture;
using Xiaowang0229.JsonLibrary;
using HashCode = RocketLauncherRemake.Utils.HashCode;

namespace RocketLauncherRemake;

public partial class ImportPage : UserControl
{
    private LaunchConfig GameConfig = new LaunchConfig();
    private MainConfig config;
    public ImportPage()
    {
        InitializeComponent();

    }


    private async void Page_Loaded(object sender,RoutedEventArgs e)
    {
        config = JsonConfig.ReadConfig();
        

        RootImage.Source = ImageIconHelper.ToAvaloniaImageSource(AppResource.DefaultGameIcon);
       
            GameConfig.HashCode = HashCode.RandomHashGenerate();
        
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

            Variables._MainWindow.Tip.IsVisible = true;
            if (!Directory.Exists($"{Variables.BackgroundPath}\\{GameConfig.HashCode}"))
                {
                    Directory.CreateDirectory($"{Variables.BackgroundPath}\\{GameConfig.HashCode}");
                }
                if(BackgroundPath.IsVisible)
                {
                    if(Path.GetExtension(BackgroundPath.Text.Substring(6)) == ".mp4")
                    {
                        await FileHelper.CopyFileAsync(BackgroundPath.Text.Substring(6), $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.mp4");
                    }
                    else
                    {
                        await FileHelper.CopyFileAsync(BackgroundPath.Text.Substring(6), $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Background.png");
                    }
                }
                if(CostumIcons.IsVisible)
                {
                    await FileHelper.CopyFileAsync(BackgroundPath.Text.Substring(6), $"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Icon.png");
                }
                else
                {
                    ImageIconHelper.ExtractAndSave256Icon(GameConfig.Launchpath,$"{Variables.BackgroundPath}\\{GameConfig.HashCode}\\Icon.png");
                }
                    config.GameInfos.Add(GameConfig);
                config.WriteConfig();

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = GameConfig.Launchpath,
                Arguments = GameConfig.Arguments,
                UseShellExecute = true
            };
            Variables.GameProcess.Add(proc);
            Variables.GameProcessStatus.Add(false);
            Variables.PlayingTimeintList.Add(0);
            var dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMinutes(1);
            dt.Tick += async (s, e) =>
            {
                Variables.PlayingTimeintList[Variables.PlayingTimeintList.Count - 1] += 1;
            };
            Variables.PlayingTimeRecorder.Add(dt);


            Variables._MainWindow.Tip.IsVisible = false;
            Page_Unloaded();
                Variables._MainWindow.RootFrame.Navigate(typeof(SettingsPage));
                
            
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
            if(string.IsNullOrEmpty(ProgramName.Text))
            {
                ProgramName.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }
            if (string.IsNullOrEmpty(TitleContent.Text))
            {
                TitleContent.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
            }

            
        }
    }

    private void ProgramName_TextChanged(object sender,RoutedEventArgs e)
    {
        GameConfig.ShowName = ProgramName.Text;
    }

    private void TitleContent_TextChanged(object sender,RoutedEventArgs e)
    {
        GameConfig.MainTitle = TitleContent.Text;
    }

    private void LaunchArgs_TextChanged(object sender,RoutedEventArgs e)
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
            RootImage.Source =await ImageIconHelper.LoadFromFileAsync(dialog.FileName);
            //substring取6
        }
    }

    private async void Background_Click(object sender,RoutedEventArgs e)
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
}