using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKBS_SUPPORT_API.Helpers
{
    public class FileHelper
    {
        private static readonly string[] ImageExtensions = { "jpg", "jpeg", "png" };

        public static byte[] GetProcessedFileBytes(byte[] fileBytes, string extension)
        {
            string ext = extension.Replace(".", "").ToLower();

            if (!ImageExtensions.Contains(ext))
            {
                return fileBytes;
            }

            try
            {
                using (var inputStream = new MemoryStream(fileBytes))
                using (var outputStream = new MemoryStream())
                {
                    using (var image = Image.Load(inputStream))
                    {
                        if (ext == "jpg" || ext == "jpeg")
                        {
                            image.Save(outputStream, new JpegEncoder { Quality = 70 });
                        }
                        else if (ext == "png")
                        {
                            image.Save(outputStream, new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression });
                        }
                        return outputStream.ToArray();
                    }
                }
            }
            catch
            {
                return fileBytes;
            }
        }
    }
}