using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Interfaces
{
    public interface IFileManager
    {
        void CheckAndCreateDirectory(string imageFolderPath);

        Task<string> SaveOriginalAsync(string imageFolder, IFormFile photo, CancellationToken cancellationToken);

        string SaveThumbnail(Bitmap thumbnail, string imagePath, string thumbnailFileName, long compressionLevel);
    }
}
