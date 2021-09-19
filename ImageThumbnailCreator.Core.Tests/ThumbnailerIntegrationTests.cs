using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests
{
    /// <summary>
    /// Integration tests.
    /// </summary>
    public class ThumbnailerIntegrationTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailFolder = Path.Combine(_testImageFolder, @"TestThumbnails");
        private static string _originalFileSaveFolder = Path.Combine(_testImageFolder, @"OriginalFiles");
        private Thumbnailer _thumbnailer = new Thumbnailer();
        private DirectoryFixture _fixture;

        public ThumbnailerIntegrationTests(DirectoryFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void CreateNewThumbnailDirectoryCreatesDirectorySuccessfully()
        {
            _thumbnailer.CheckAndCreateDirectory(_thumbnailFolder);
            _thumbnailer.CheckAndCreateDirectory(_originalFileSaveFolder);

            Assert.True(Directory.Exists(_thumbnailFolder));
            Assert.True(Directory.Exists(_originalFileSaveFolder));

            // Cleanup the unnecessary folders
            Directory.Delete(_thumbnailFolder);
            Directory.Delete(_originalFileSaveFolder);
        }

        [Theory]
        [InlineData(@"landscapeForewardFacing.jpg")]
        [InlineData(@"landscapeRearFacing.jpg")]
        [InlineData(@"portraitForewardFacing.jpg")]
        [InlineData(@"portraitRearFacing.jpg")]
        [InlineData(@"largeLandscape.jpg")]
        [InlineData(@"largePortrait.jpg")]
        [InlineData(@"largeSquare.jpg")]
        [InlineData(@"largeLandscape.tif")]
        [InlineData(@"largePortrait.tif")]
        [InlineData(@"largeSquare.tif")]
        [InlineData(@"largeLandscape.bmp")]
        [InlineData(@"largePortrait.bmp")]
        [InlineData(@"largeSquare.bmp")]
        [InlineData(@"largeLandscape.gif")]
        [InlineData(@"largePortrait.gif")]
        [InlineData(@"largeSquare.gif")]
        [InlineData(@"largeLandscape.png")]
        [InlineData(@"largePortrait.png")]
        [InlineData(@"largeSquare.png")]
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

        [Theory]
        [InlineData(@"largeLandscape.jpg", 50L)]
        [InlineData(@"largePortrait.jpg", 50L)]
        [InlineData(@"largeSquare.jpg", 50L)]
        [InlineData(@"largeLandscape.tif", 50L)]
        [InlineData(@"largePortrait.tif", 50L)]
        [InlineData(@"largeSquare.tif", 50L)]
        [InlineData(@"largeLandscape.bmp", 50L)]
        [InlineData(@"largePortrait.bmp", 50L)]
        [InlineData(@"largeSquare.bmp", 50L)]
        [InlineData(@"largeLandscape.gif", 50L)]
        [InlineData(@"largePortrait.gif", 50L)]
        [InlineData(@"largeSquare.gif", 50L)]
        [InlineData(@"largeLandscape.png", 50L)]
        [InlineData(@"largePortrait.png", 50L)]
        [InlineData(@"largeSquare.png", 50L)]
        public async Task CreateThumbnailWithSpecifiedCompressionSavesSuccesfully(string fileName, long compressionLevel)
        {
            //setup
            SetupTestDirectory();
            string imageLocation = Path.Combine(_testImageFolder, fileName);

            IFormFile formFile = ConvertFileToStream(imageLocation, this.GetImageTypeEnum(fileName), fileName);

            //act
            string thumbnailSaveLocation = await _thumbnailer.Create(100, _thumbnailFolder, _originalFileSaveFolder, formFile, compressionLevel);

            string[] images = Directory.GetFiles(_thumbnailFolder);

            //assert
            Assert.NotNull(thumbnailSaveLocation);
            Assert.Contains(fileName, thumbnailSaveLocation);
            Assert.True(images.Length == 1);
            Assert.Single(images);

            //tear down
            TearDownTestDirectory(); //TODO: Make this part of the test fixture so it runs even if assertions fail
        }

        [Theory]
        [InlineData(@"largeLandscape.jpg")]
        [InlineData(@"largePortrait.jpg")]
        [InlineData(@"largeSquare.jpg")]
        [InlineData(@"largeLandscape.tif")]
        [InlineData(@"largePortrait.tif")]
        [InlineData(@"largeSquare.tif")]
        [InlineData(@"largeLandscape.bmp")]
        [InlineData(@"largePortrait.bmp")]
        [InlineData(@"largeSquare.bmp")]
        [InlineData(@"largeLandscape.gif")]
        [InlineData(@"largePortrait.gif")]
        [InlineData(@"largeSquare.gif")]
        [InlineData(@"largeLandscape.png")]
        [InlineData(@"largePortrait.png")]
        [InlineData(@"largeSquare.png")]
        public async Task CreateThumbnailWithDefaultCompressionSavesSuccesfully(string fileName)
        {
            //setup
            SetupTestDirectory();
            string imageLocation = Path.Combine(_testImageFolder, fileName);

            IFormFile formFile = ConvertFileToStream(imageLocation, this.GetImageTypeEnum(fileName), fileName);

            //act
            string thumbnailSaveLocation = await _thumbnailer.Create(100, _thumbnailFolder, _originalFileSaveFolder, formFile);

            string[] images = Directory.GetFiles(_thumbnailFolder);

            //assert
            Assert.NotNull(thumbnailSaveLocation);
            Assert.Contains(fileName, thumbnailSaveLocation);
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
                    Directory.Delete(_thumbnailFolder, true);
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