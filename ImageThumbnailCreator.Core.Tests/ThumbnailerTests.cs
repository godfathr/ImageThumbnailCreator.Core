﻿using Microsoft.AspNetCore.Http;
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

        [Fact]
        public async Task SaveOriginalSavesSuccessfully()
        {
            //setup
            SetupTestDirectory();
            string imageLocation = Path.Combine(_testImageFolder, @"largeLandscape.jpg");

            IFormFile formFile = ConvertFileToStream(imageLocation);

            //act
            string originalFileSaveLocation = await _thumbnailer.SaveOriginalAsync(_originalFileSaveFolder, formFile);
            //_thumbnailer.Create(100, _thumbnailFolder, imageLocation);

            string[] images = Directory.GetFiles(_originalFileSaveFolder);

            //assert
            Assert.NotNull(originalFileSaveLocation);
            Assert.Equal("", originalFileSaveLocation);
            Assert.True(images.Length == 1);
            Assert.Single(images);

            //tear down
            TearDownTestDirectory();
        }

        private IFormFile ConvertFileToStream(string filePath)
        {
            var path = Path.Combine(_testImageFolder, filePath);
            //IFormFile file = new FormFile();
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    byte[] b = new byte[fs.Length];
                    //UTF8Encoding temp = new UTF8Encoding(true);

                    FormFile formFile = new FormFile(fs, 0, b.Length, filePath, filePath);

                    //TODO: Use the file type from the actual file and see if we can pass it forward

                    //string s = filePath.Split('\\').Last().ToString();
                    //s = s.Split('.').Last();
                    //string ss = ImageTypeEnum.ImageTypes.SingleOrDefault(x => x.Key.Contains(s, StringComparison.OrdinalIgnoreCase)).Value;

                    formFile.ContentType = ss;

                    return formFile;
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
                if (!Directory.Exists(_originalFileSaveFolder))
                {
                    Directory.Delete(_originalFileSaveFolder);
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