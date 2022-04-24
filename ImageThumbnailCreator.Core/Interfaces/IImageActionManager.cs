using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.Interfaces
{
    /// <summary>
    /// ImageAction interface
    /// </summary>
    public interface IImageActionManager
    {
        /// <summary>
        /// Worker class that creates the thumbnail.
        /// </summary>
        /// <param name="width">Desired width in pixels.</param>
        /// <param name="thumbnailImageFolder">Destination directory where the thumbnail will be saved.</param>
        /// <param name="originalFileFolder">Destination directory where the original file will be saved.</param>
        /// <param name="photo">IFormFile for image sent from the HTTP request.</param>
        /// <param name="compressionLevel">
        /// A 64-bit integer that specifies the value stored in the System.Drawing.Imaging.EncoderParameter
        /// object. Must be nonnegative. This parameter is converted to a 32-bit integer
        /// before it is stored in the System.Drawing.Imaging.EncoderParameter object.</param>
        /// <returns><see cref="Task"/>&lt;<see cref="string"/>&gt; File path to where the image was saved.</returns>
        Task<string> CreateAsync(float width, string thumbnailImageFolder, string originalFileFolder, IFormFile photo, long compressionLevel);

        /// <summary>
        /// Ensure that uploaded files are oriented the way the photo was taken or image was created.
        /// Guards against images taken with a rear facing camera or landscape/portrait image from being displayed or saved
        /// with the wrong corner as the origin or in the wrong orientation.
        /// </summary>
        /// <param name="propertyIdList">List of integers representing all EXIF properties for the image file.</param>
        /// <param name="srcImage">The <see cref="Image"/> object to be oriented.</param>
        /// <returns><see cref="RotateFlipType"/></returns>
        RotateFlipType OrientUpright(List<int> propertyIdList, Image srcImage);

        /// <summary>
        /// Calculates the dimensions of the thumbnail and returns them in a tuple with height as the first item and width as the second item.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns><see cref="Tuple"/>&lt;<see cref="float"/>, <see cref="float"/>&gt; Item1 = height, Item2 = width.</returns>
        void SetDimensions(float width, ref float imageWidth, ref float imageHeight);
    }
}
