using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core
{
    /// <summary>
    /// Enum of supported image types.
    /// </summary>
    public static class ImageTypeEnum
    {
        /// <summary>
        /// Enum of supported image types.
        /// </summary>
        public static Dictionary<string, string> ImageTypes = new Dictionary<string, string>()
        {
            { "Jpeg", "image/jpeg" },
            { "Jpe", "image/jpe" },
            { "Jpg", "image/jpg" },
            { "Tif", "image/tif" },
            { "Tiff", "image/tiff" },
            { "Png", "image/png" },
            { "Bmp", "image/bmp" },
            { "Gif", "image/gif" }
        };
    }
}
