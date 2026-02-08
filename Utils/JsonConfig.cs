using System;
using System.Collections.Generic;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public readonly static string Configpath = Environment.CurrentDirectory + @"\Config.json";

        public static MainConfig config;
    }
    public static class JsonConfig
    {
        public static void InitalizeConfig(bool Confirm)
        {
            var config = new MainConfig
            {
                Username = "Administrator",
                StartUpCheckUpdate = true,
                LaunchWithMinize = true,
                GameInfos = new List<LaunchConfig>()
            };
            Json.WriteJson(Variables.Configpath, config);
            //ConvertToPngAndSave(ApplicationResources.UserIcon, Environment.CurrentDirectory+@"\Head.png");
        }

        public static MainConfig ReadConfig()
        {
            return Json.ReadJson<MainConfig>(Variables.Configpath);
        }

        public static void WriteConfig(this MainConfig config)
        {
            Json.WriteJson(Variables.Configpath, config);
        }
    }

    public class LaunchConfig
    {

        public string HashCode { get; set; }
        public string ShowName { get; set; }
        public string MainTitle { get; set; }
        public System.Windows.Media.FontFamily MaintitleFontName { get; set; }
        public System.Windows.Media.Brush MainTitleFontColor { get; set; }
        public long GamePlayedMinutes { get; set; }
        public string Launchpath { get; set; }
        public string Arguments { get; set; }



    }
    public class MainConfig
    {

        //用户名
        public string Username { get; set; }

        //自动更新
        public bool StartUpCheckUpdate { get; set; }

        //游戏配置项，勿动
        public List<LaunchConfig> GameInfos { get; set; }

        //游戏启动时最小化窗口
        public bool LaunchWithMinize { get; set; }



    }

    public class UpdateConfig
    {
        public string UpdateVersion { get; set; }
        public string UpdateLog { get; set; }
        public string UpdateLink { get; set; }


    }
}
