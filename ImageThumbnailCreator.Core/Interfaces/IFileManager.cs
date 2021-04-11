using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace ImageThumbnailCreator.Interfaces
{
    public interface IFileManager
    {
        void CheckAndCreateDirectory(string imageFolderPath);

        string SaveOriginal(string imageFolder, IFormFile photo); //the photo should be an IFormFile

        string SaveThumbnail(Bitmap thumbnail, string imagePath, string thumbnailFileName, long compressionLevel);
    }
}
