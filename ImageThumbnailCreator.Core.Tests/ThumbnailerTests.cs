using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests
{
    public class ThumbnailerTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailFolder = Path.Combine(_testImageFolder, @"TestThumbnails");
        private static string _originalFileSaveFolder = Path.Combine(_testImageFolder, @"OriginalFiles");
        private Thumbnailer _thumbnailer = new Thumbnailer();
        private DirectoryFixture _fixture;

        public ThumbnailerTests(DirectoryFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void CreateNewThumbnailDirectoryCreatesDirectorySuccessfully()
        {
            _thumbnailer.CheckAndCreateDirectory(_thumbnailFolder);

            Assert.True(Directory.Exists(_thumbnailFolder));

            // Cleanup the unnecessary folder
            Directory.Delete(_thumbnailFolder);
        }

        [Theory]
        [InlineData(@"largeLandscape.jpg")]
        public async Task SaveOriginalSavesSuccessfully(string fileName)
        {
            //setup
            SetupTestDirectory();
            string imageLocation = Path.Combine(_testImageFolder, fileName);

            IFormFile formFile = ConvertFileToStream(imageLocation, this.GetImageTypeEnum(fileName), fileName);

            //act
            string originalFileSaveLocation = await _thumbnailer.SaveOriginalAsync(_originalFileSaveFolder, formFile);
            //_thumbnailer.Create(100, _thumbnailFolder, imageLocation);

            string[] images = Directory.GetFiles(_originalFileSaveFolder);

            //assert
            Assert.NotNull(originalFileSaveLocation);
            Assert.Contains(fileName, originalFileSaveLocation);
            Assert.True(images.Length == 1);
            Assert.Single(images);

            //tear down
            TearDownTestDirectory(); //TODO: Make this part of the test fixture so it runs even if assertions fail
        }

        private string GetImageTypeEnum(string fileName)
        {
            string fileExtension = fileName.Split('.').Last();

            return ImageTypeEnum.ImageTypes
                .Where(x => string.Equals(x.Key, fileExtension, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault()
                .Value;
        }

        private IFormFile ConvertFileToStream(string filePath, string fileType, string fileName)
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

        private void SetupTestDirectory()
        {
            try
            {
                if (!Directory.Exists(_thumbnailFolder))
                {
                    Directory.CreateDirectory(_thumbnailFolder);
                }
                if (!Directory.Exists(_originalFileSaveFolder))
                {
                    Directory.CreateDirectory(_originalFileSaveFolder);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void TearDownTestDirectory()
        {
            try
            {
                if (Directory.Exists(_thumbnailFolder))
                {
                    Directory.Delete(_thumbnailFolder);
                }
                if (Directory.Exists(_originalFileSaveFolder))
                {
                    Directory.Delete(_originalFileSaveFolder, true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    //TODO: Figure out why this doesn't work
    public class DirectoryFixture : IDisposable
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages\largeLandscape.jpg");
        private static string _thumbnailFolder = Path.Combine(_testImageFolder, @"TestThumbnails");

        private void SetupTestDirectory()
        {
            try
            {
                if (!Directory.Exists(_thumbnailFolder))
                {
                    Directory.CreateDirectory(_thumbnailFolder);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void TearDownTestDirectory()
        {
            try
            {
                if (Directory.Exists(_thumbnailFolder))
                {
                    Directory.Delete(_thumbnailFolder);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            TearDownTestDirectory();
        }
    }
}