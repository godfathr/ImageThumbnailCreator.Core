using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.RazorPages.Pages
{
    public class IndexModel : PageModel
    {
        private IConfigurationRoot _configuration;
        private IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;
        private static Thumbnailer _thumbnailer = new Thumbnailer();

        private static string _uploadFolder;// = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\uploads");
        //private static string _originalFileSaveFolder = Path.Combine(_uploadFolder, @"Thumbnails"); //if original files need to be in a separate directory

        public IndexModel(
            IConfiguration configuration,
            IWebHostEnvironment environment, 
            ILogger<IndexModel> logger)
        {
            _configuration = (IConfigurationRoot)configuration;
            _environment = environment;
            _logger = logger;
            _uploadFolder = Path.Combine(_environment.ContentRootPath, "wwwroot\\uploads");
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task OnPostAsync()
        {
            _thumbnailer.CheckAndCreateDirectory(_uploadFolder);

            var file = Path.Combine(_uploadFolder, Upload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);

                var thumbnail = await _thumbnailer.Create(200, _uploadFolder, $"{_uploadFolder}\\originals", Upload, 90L);

                _logger.LogInformation($"Successfully uploaded {Upload.FileName} to {thumbnail}");
            }
        }
    }
}