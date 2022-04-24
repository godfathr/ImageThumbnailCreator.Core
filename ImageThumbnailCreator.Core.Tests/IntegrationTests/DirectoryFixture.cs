using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;

namespace ImageThumbnailCreator.Core.Tests.IntegrationTests
{
    public class DirectoryFixture : IDisposable
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailAndOriginalSaveFolder = Path.Combine(_testImageFolder, @"ProcessedImages");

        public DirectoryFixture()
        {
            SetupTestDirectory();
        }

        public void SetupTestDirectory()
        {
            try
            {
                if (!Directory.Exists(_thumbnailAndOriginalSaveFolder))
                {
                    Directory.CreateDirectory(_thumbnailAndOriginalSaveFolder);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void TearDownTestDirectory()
        {
            try
            {
                if (Directory.Exists(_thumbnailAndOriginalSaveFolder))
                {
                    Directory.Delete(_thumbnailAndOriginalSaveFolder, true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetImageTypeEnum(string fileName)
        {
            string fileExtension = fileName.Split('.').Last();

            return ImageTypeEnum.ImageTypes
                .Where(x => string.Equals(x.Key, fileExtension, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault()
                .Value;
        }

        public IFormFile ConvertFileToStream(string filePath, string fileType, string fileName)
        {
            var path = Path.Combine(_testImageFolder, filePath);
            //IFormFile file = new FormFile();
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            else
            {
                //It is important not to use a 'using' statement here. If we do, the
                //base stream is closed and we are unable to use it as a parameter in other methods.
                FileStream stream = File.Open(path, FileMode.OpenOrCreate);

                byte[] b = new byte[stream.Length];

                //Use the file type from the actual file and see if we can pass it forward
                FormFile formFile = new FormFile(stream, 0, stream.Length, fileName,
                    Path.GetFileName(filePath))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = fileType,
                    ContentDisposition = fileName
                };

                return formFile;
            }
        }

        public void Dispose()
        {
            TearDownTestDirectory();
        }
    }
}