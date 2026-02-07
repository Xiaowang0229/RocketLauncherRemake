using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static string Version = "4.0.0-Alpha.1";
        public static string ShowVersion = $"版本:{Version}";
        public static CancellationTokenSource UpdateCTS = new CancellationTokenSource();
    }
    public class Update
    {
        public async static Task CheckUpdate(bool IsSameVersionShowDialog = false, bool ShowException = false)
        {
            var client = new HttpClient();
            try
            {
                var content = await client.GetStringAsync("https://gitee.com/xiaowangupdate/update-service/raw/master/MultiGameLauncher", Variables.UpdateCTS.Token);
                var updcfg = Json.ReadJson<UpdateConfig>(content);
                if (updcfg.UpdateVersion != Variables.Version)
                {



                    var filname = HashCode.RandomHashGenerate();
                    var win = Variables._MainWindow;

                    //var results = await win.ShowMessageAsync("更新可用", $"当前版本:{Variables.Version},最新版本:{updcfg.UpdateVersion},请问是否更新？", MessageDialogStyle.AffirmativeAndNegative, settings);
                    var results = await win.ShowMessageAsync("更新可用", $"当前版本:{Variables.Version},最新版本:{updcfg.UpdateVersion},请问是否更新？");
                    if (results == true)
                    {
                        if (File.Exists(Path.GetTempPath() + "\\" + filname + ".exe"))
                        {
                            File.Delete(Path.GetTempPath() + "\\" + filname + ".exe");
                        }
                        try
                        {

                            File.Copy($"{Environment.CurrentDirectory}\\UpdateAPI.exe", Path.GetTempPath() + "\\" + filname + ".exe");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = $"{Path.GetTempPath()}\\{filname}.exe",
                                Arguments = $"\"{updcfg.UpdateLink}\" \"{Environment.ProcessPath}\"",
                                UseShellExecute = true
                            });
                            TaskBar.KillTaskBar();
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            if (ShowException)
                            {
                                await Variables._MainWindow.ShowMessageAsync("更新时发现错误", $"{ex.Message}");
                            }
                            Environment.Exit(0);
                        }

                    }




                }
                else if (updcfg.UpdateVersion == Variables.Version && IsSameVersionShowDialog)
                {
                    Variables._MainWindow.ShowMessageAsync("提示", $"当前版本已是最新版本:{updcfg.UpdateVersion}！");
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                if (ShowException)
                {
                    Variables._MainWindow.ShowMessageAsync("检测更新时发现错误", $"{ex.Message}");
                }
            }

        }
    }
}
