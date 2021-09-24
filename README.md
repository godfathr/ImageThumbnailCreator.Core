# Image Thumbnail Creator .NET 5

## Short description
- Create thumbnails from images
- Save the original with the thumbnail version for easy links to full resolution version
- Downsize large resolution images and compress thumbnails to your desired level

Excellent addition for custom blog sites, local file management, and even a tool to quickly downsize images for your custom content.

- *Original uploaded images can be saved to the file system.* 

- *Thumbnails will be saved to the file system automatically.*

## Repository 
[https://github.com/godfathr/ImageThumbnailCreator.Core](https://github.com/godfathr/ImageThumbnailCreator.Core).


### Sample projects
- [Razor pages](https://github.com/godfathr/ImageThumbnailCreator.Core/tree/main/ImageThumbnailCreator.Core.RazorPages)

### Sample usage
```
private static string _thumbnailAndOriginalSaveFolder = Path.Combine(_testImageFolder, @"ProcessedImages");

private Thumbnailer _thumbnailer = new Thumbnailer();

```

```
//Save the original photo at its original resolution
string imageLocation = Path.Combine(_testImageFolder, fileName);

IFormFile formFile = _fixture.ConvertFileToStream(imageLocation, _fixture.GetImageTypeEnum(fileName), fileName);
```

Specify a compression level:
```
//With specified compression level of 75
string thumbnailSaveLocation = await _thumbnailer.Create(250, _thumbnailAndOriginalSaveFolder, _thumbnailAndOriginalSaveFolder, formFile, 75L);

```

OR if a compression value is not provided, the default 85 will be used:
```
string thumbnailSaveLocation = await _thumbnailer.Create(250, _thumbnailAndOriginalSaveFolder, _thumbnailAndOriginalSaveFolder, formFile);
```
### Link to .NET Framework version
[https://dotnet.microsoft.com/download/dotnet/5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

### Link to report issues
[https://github.com/godfathr/ImageThumbnailCreator.Core/issues](https://github.com/godfathr/ImageThumbnailCreator.Core/issues)

### License
[https://github.com/godfathr/ImageThumbnailCreator.Core/blob/main/LICENSE](https://github.com/godfathr/ImageThumbnailCreator.Core/blob/main/LICENSE)
