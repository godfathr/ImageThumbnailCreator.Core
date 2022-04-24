namespace ImageThumbnailCreator.Core.Interfaces
{
    /// <summary>
    /// Interface for inheritence chain for IThumbnailer
    /// </summary>
    public interface IThumbnailer: IFileManager, IImageActionManager
    {
    }
}
