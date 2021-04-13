using System;
using System.IO;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests
{
    public class ThumbnailerTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages\largeLandscape.jpg");
        private static string _thumbnailFolder = Path.Combine(_testImageFolder, @"TestThumbnails");
        private Thumbnailer _thumbnailer = new Thumbnailer();

        [Fact]
        public void CreateNewThumbnailDirectoryCreatesDirectorySuccessfully()
        {
            _thumbnailer.CheckAndCreateDirectory(_thumbnailFolder);

            Assert.True(Directory.Exists(_thumbnailFolder));

            // Cleanup the unnecessary folder
            Directory.Delete(_thumbnailFolder);
        }

        [Fact]
        public void SaveOriginalSavesSuccessfully()
        {
            //setup
            string imageLocation = Path.Combine(_testImageFolder, @"TestImages\largeLandscape.jpg");

            //act
            _thumbnailer.Create(100, _thumbnailFolder, imageLocation);

            string[] images = Directory.GetFiles(_thumbnailFolder);

            //assert
            Assert.True(images.Length == 1);
            Assert.Single(images);
        }
    }

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