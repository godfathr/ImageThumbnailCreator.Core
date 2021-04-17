using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Interfaces
{
    public interface IFileManager
    {
        void CheckAndCreateDirectory(string imageFolderPath);

        Task<string> SaveOriginalAsync(string imageFolder, IFormFile photo); //the photo should be an IFormFile

        string SaveThumbnail(Bitmap thumbnail, string imagePath, string thumbnailFileName, long compressionLevel);
    }
}
