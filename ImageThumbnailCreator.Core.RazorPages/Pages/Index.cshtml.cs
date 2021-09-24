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

            var file = Path.Combine(_uploadFolder, Upload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                var thumbnailPath = await _thumbnailer.Create(200, _uploadFolder, $"{_uploadFolder}\\originals", Upload, 90L);

                _logger.LogInformation($"Successfully uploaded {Upload.FileName} to {thumbnailPath}");
            }
        }
    }
}