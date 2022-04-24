using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.Interfaces
{
    /// <summary>
    /// Interface for file management
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Create the directory for storing image files if it doesn't already exist.
        /// </summary>
        /// <param name="imageFolderPath">Path where directory should be created</param>
        void CheckAndCreateDirectory(string imageFolderPath);

        /// <summary>
        /// Save the original file that is being uploaded and thumbnailed.
        /// </summary>
        /// <param name="imageFolder">Destination directory where the original file will be saved.</param>
        /// <param name="photo">IFormFile for image sent from the HTTP request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task"/>&lt;<see cref="string"/>&gt; File path to where the image was saved.</returns>
        Task<string> SaveOriginalAsync(string imageFolder, IFormFile photo, CancellationToken cancellationToken);

        /// <summary>
        /// Save the thumbnail to a specified file path.
        /// </summary>
        /// <param name="thumbnail"><see cref="Bitmap"/> object of the actual thumbnail to be saved.</param>
        /// <param name="imagePath">File path where the thumbnail should be saved.</param>
        /// <param name="thumbnailFileName">Name of the thumbnail file to save.</param>
        /// <param name="compressionLevel">Optional: <see cref="long"/> value for image compression. Default value is 85.</param>
        /// <returns><see cref="string"/> Path of the thumbnail including the filename.</returns>
        string SaveThumbnail(Bitmap thumbnail, string imagePath, string thumbnailFileName, long compressionLevel);
    }
}
