using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Interfaces
{
    /// <summary>
    /// Thumbnailer interface
    /// </summary>
    public interface IThumbnailer
    {
        /// <summary>
        /// Worker class that creates the thumbnail.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="thumbnailImageFolder"></param>
        /// <param name="originalFileFolder"></param>
        /// <param name="photo"></param>
        /// <param name="compressionLevel"></param>
        /// <returns></returns>
        Task<string> Create(float width, string thumbnailImageFolder, string originalFileFolder, IFormFile photo, long compressionLevel);

        /// <summary>
        /// Ensure that uploaded files are oriented the way the photo was taken or image was created.
        /// Guards against images taken with a rear facing camera or landscape/portrait image from being displayed or saved
        /// with the wrong corner as the origin or in the wrong orientation.
        /// </summary>
        /// <param name="propertyIdList"></param>
        /// <param name="srcImage"></param>
        /// <returns></returns>
        RotateFlipType OrientUpright(List<int> propertyIdList, Image srcImage);

        /// <summary>
        /// Set the height and width of the images
        /// </summary>
        /// <param name="width"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        Tuple<float, float> SetDimensions(float width, ref float imageWidth, ref float imageHeight);
    }
}
