# Image Thumbnail Creator .NET Standard 2.0

[![.NET](https://github.com/godfathr/ImageThumbnailCreator.Core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/godfathr/ImageThumbnailCreator.Core/actions/workflows/dotnet.yml)

## Short description
- Create simple thumbnails from images
- Save the original with the thumbnail version for easy links to full resolution version
- Downsize large resolution images and compress thumbnails to your desired level

Excellent addition for custom blog sites, local file management, and even a tool to quickly downsize images for your custom content.

- *Original uploaded images can be saved to the file system.* 

- *Thumbnails will be saved to the file system automatically.*

## Repository 
[https://github.com/godfathr/ImageThumbnailCreator.Core](https://github.com/godfathr/ImageThumbnailCreator.Core)


## Sample projects
- [Razor pages](https://github.com/godfathr/ImageThumbnailCreator.Core/tree/main/ImageThumbnailCreator.Core.RazorPages)
- *More to come...*

## Sample usage
### Razor Pages
*Index.cshtml*
```
@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Short example to show an implementation of the ImageThumbnailCreator.Core NuGet package.</p>

    <form method="post" enctype="multipart/form-data">
        <input type="file" asp-for="Upload" />
        <input type="submit" />
    </form>
</div>

```
Index.cshtml.cs
```
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.RazorPages.Pages
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;
        private static Thumbnailer _thumbnailer = new Thumbnailer();
        private static string _uploadFolder;

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IndexModel(IWebHostEnvironment environment, ILogger<IndexModel> logger)
        {
            _environment = environment;
            _logger = logger;
            _uploadFolder = Path.Combine(_environment.ContentRootPath, "wwwroot\\uploads");
        }

        public async Task OnPostAsync()
        {
            // if upload directory doesn't exist, create it
            _thumbnailer.CheckAndCreateDirectory(_uploadFolder);

            // Create the thumbnail and save original upload

            // Compression level 90 as optional parameter
            var thumbnailPath = await _thumbnailer.Create(200, _uploadFolder, $"{_uploadFolder}\\originals", Upload, 90L);

            // Compression level 85 if no compression parameter supplied
            //var thumbnailPath = await _thumbnailer.Create(200, _uploadFolder, $"{_uploadFolder}\\originals", Upload);

            _logger.LogInformation($"Successfully uploaded {Upload.FileName} to {thumbnailPath}");
        }
    }
}
```

### Link to .NET Framework version
[https://dotnet.microsoft.com/download/visual-studio-sdks](https://dotnet.microsoft.com/download/visual-studio-sdks)

### Link to report issues
[https://github.com/godfathr/ImageThumbnailCreator.Core/issues](https://github.com/godfathr/ImageThumbnailCreator.Core/issues)

### License
[https://github.com/godfathr/ImageThumbnailCreator.Core/blob/main/LICENSE](https://github.com/godfathr/ImageThumbnailCreator.Core/blob/main/LICENSE)
