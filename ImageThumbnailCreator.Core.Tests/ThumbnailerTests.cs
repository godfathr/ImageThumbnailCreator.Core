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
        DirectoryFixture _fixture;

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

        [Fact]
        public void SaveOriginalSavesSuccessfully()
        {
            //setup
            SetupTestDirectory();
            string imageLocation = Path.Combine(_testImageFolder, @"TestImages\largeLandscape.jpg");

            //act
            _thumbnailer.Create(100, _thumbnailFolder, imageLocation);

            string[] images = Directory.GetFiles(_thumbnailFolder);

            //assert
            Assert.True(images.Length == 1);
            Assert.Single(images);

            //tear down
            TearDownTestDirectory();
        }

        private void ConvertFileToStream(string fileName)
        {
            var path = Path.Combine(_testImageFolder, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    while (fs.Read(b, 0, b.Length) > 0)
                    {
                        Console.WriteLine(temp.GetString(b));
                    }
                }
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
    }

    //TODO: Figure out why this doesn't work
    //public class DirectoryFixture : IDisposable
    //{
    //    private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages\largeLandscape.jpg");
    //    private static string _thumbnailFolder = Path.Combine(_testImageFolder, @"TestThumbnails");

    //    private void SetupTestDirectory()
    //    {
    //        try
    //        {
    //            if (!Directory.Exists(_thumbnailFolder))
    //            {
    //                Directory.CreateDirectory(_thumbnailFolder);
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }

    //    }

    //    private void TearDownTestDirectory()
    //    {
    //        try
    //        {
    //            if (Directory.Exists(_thumbnailFolder))
    //            {
    //                Directory.Delete(_thumbnailFolder);
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }

    //    }

    //    public void Dispose()
    //    {
    //        TearDownTestDirectory();
    //    }
    //}
}