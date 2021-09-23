using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Interfaces
{
    /// <summary>
    /// Interface for file management
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// If directory does not exist, create it.
        /// </summary>
        /// <param name="imageFolderPath">Path where directory should be created</param>
        void CheckAndCreateDirectory(string imageFolderPath);

        /// <summary>
        /// Save the original file that is being uploaded and thumbnailed.
        /// </summary>
        /// <param name="imageFolder">Path to folder where file will be saved</param>
        /// <param name="photo">Image from HttpRequest</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<string> SaveOriginalAsync(string imageFolder, IFormFile photo, CancellationToken cancellationToken);

        /// <summary>
        /// Save the smaller resoluation, and if applicable, compress the thumbnail with the desired compression level.
        /// </summary>
        /// <param name="thumbnail">Bitmap object</param>
        /// <param name="imagePath">Path to directory</param>
        /// <param name="thumbnailFileName">Name of thumbnail</param>
        /// <param name="compressionLevel">Desired compression level</param>
        /// <returns></returns>
        string SaveThumbnail(Bitmap thumbnail, string imagePath, string thumbnailFileName, long compressionLevel);
    }
}
