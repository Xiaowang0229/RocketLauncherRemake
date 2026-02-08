using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static string VersionLog;
        public static string EULAString;
        public static string BackgroundPath = $"{Environment.CurrentDirectory}\\Backgrounds";
    }
    public static class FileHelper
    {
        public static string ReadEmbeddedMarkdown(string resourceName)
        {
            // 获取当前执行的程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 获取嵌入资源的流
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Resource {resourceName} not found.");

                // 使用 StreamReader 读取流中的文本
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public async static Task CopyFileAsync(string source, string dest)
        {
            if (Variables._MainWindow != null)
            {

                Variables._MainWindow.Tip.IsVisible = true;

            }

            await Task.Run(() => File.Copy(source, dest, overwrite: true));
            if (Variables._MainWindow != null)
            {
                Variables._MainWindow.Tip.IsVisible = false;

            }

        }
    }
}
