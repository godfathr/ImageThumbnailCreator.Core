using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Interfaces
{
    public interface IThumbnailer
    {
        Task<string> Create(float width, string thumbnailImageFolder, string originalFileFolder, IFormFile photo, long compressionLevel);

        RotateFlipType OrientUpright(List<int> propertyIdList, Image srcImage);

        Tuple<float, float> SetDimensions(float width, ref float imageWidth, ref float imageHeight);
    }
}
