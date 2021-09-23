using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.RazorPages.Pages
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;
        private static Thumbnailer _thumbnailer = new Thumbnailer();

        private static string _uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"OriginalFiles");
        private static string _originalFileSaveFolder = Path.Combine(_uploadFolder, @"Thumbnails"); //if original files need to be in a separate directory

        public IndexModel(IWebHostEnvironment environment, ILogger<IndexModel> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task OnPostAsync()
        {
            var file = Path.Combine(_uploadFolder, Upload.FileName);

            _thumbnailer.CheckAndCreateDirectory(_uploadFolder);
            _thumbnailer.CheckAndCreateDirectory(_originalFileSaveFolder);

            //IFormFile formFile = ConvertFileToStream(imageLocation, this.GetImageTypeEnum(fileName), fileName);
            //string originalFileSaveLocation = await _thumbnailer.SaveOriginalAsync(_originalFileSaveFolder, formFile);


            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }
        }
    }
}