using Avalonia.Media;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TsudaKageyu;

namespace RocketLauncherRemake.Utils
{

    public static class ImageIconHelper
    {
        public static IImage? ToAvaloniaImageSource(byte[]? imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;  // 或 throw new ArgumentNullException(nameof(imageBytes));
            }

            try
            {
                using var memoryStream = new MemoryStream(imageBytes);
                // Avalonia 的 Bitmap 构造函数直接支持从 Stream 加载
                return new Avalonia.Media.Imaging.Bitmap(memoryStream);
            }
            catch (Exception ex)
            {
                // 常见原因：字节不是有效图像、格式不支持、损坏等
                Console.WriteLine($"转换为 Avalonia Bitmap 失败: {ex.Message}");
                return null;
            }
        }
        public static string ExtractAndSave256Icon(
        string exePath,
        string outputPngPath
        )
        {
            byte[] defaultIconBytes = AppResource.DefaultGameIcon;
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                // 文件不存在，直接用默认
                System.Windows.MessageBox.Show("TTEWHNAHAIUKKSKJ");
                return SaveDefaultIcon(defaultIconBytes, outputPngPath);
            }

            try
            {
                // 使用 TsudaKageyu.IconExtractor 提取所有图标变体
                var extractor = new IconExtractor(exePath);

                // 获取所有图标（Icon 对象列表）
                var icons = extractor.GetAllIcons();

                if (icons == null || icons.Length == 0)
                {
                    return SaveDefaultIcon(defaultIconBytes, outputPngPath);
                }

                // 优先找 256x256 的图标（宽度或高度为 256）
                Icon? targetIcon = null;
                foreach (var icon in icons)
                {
                    // Icon 可能包含多个大小，我们取第一个 256x256 的
                    using var bmp = icon.ToBitmap();
                    if (bmp.Width == 256 && bmp.Height == 256)
                    {
                        targetIcon = icon;
                        break;
                    }
                }

                // 如果没找到精确 256x256，取最大的那个（通常是 256 或 128 等）
                if (targetIcon == null)
                {
                    targetIcon = icons
                        .OrderByDescending(i => i.Width * i.Height)  // 按面积降序
                        .FirstOrDefault();
                }

                if (targetIcon == null)
                {
                    throw new Exception("未找到合适大小的图标");
                }

                // 保存为 PNG（支持透明）
                using var bitmap = targetIcon.ToBitmap();
                bitmap.Save(outputPngPath, ImageFormat.Png);

                return outputPngPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"提取图标失败: {ex.Message}，使用默认图标");
                // 出错时 fallback 到默认
                return SaveDefaultIcon(defaultIconBytes, outputPngPath);
            }
        }

        private static string SaveDefaultIcon(byte[] defaultBytes, string outputPath)
        {
            if (defaultBytes == null || defaultBytes.Length == 0)
            {
                throw new ArgumentException("默认图标字节为空，无法保存");
            }

            try
            {
                using var ms = new MemoryStream(defaultBytes);
                using var bitmap = new System.Drawing.Bitmap(ms);
                bitmap.Save(outputPath, ImageFormat.Png);
                return outputPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存默认图标失败: {ex.Message}");
                // 可选：返回一个空路径或抛异常，根据你的需求
                return string.Empty;
            }
        }

        public static async Task<IImage?> LoadFromFileAsync(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;

            return await Task.Run(() => new Avalonia.Media.Imaging.Bitmap(filePath));
        }

        public static void ConvertToPngAndSave(byte[] imageBytes, string savePath)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return;

            try
            {
                using (MemoryStream ms = new MemoryStream(imageBytes))
                using (Image image = Image.FromStream(ms))
                {
                    image.Save(savePath, ImageFormat.Png);
                }
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
