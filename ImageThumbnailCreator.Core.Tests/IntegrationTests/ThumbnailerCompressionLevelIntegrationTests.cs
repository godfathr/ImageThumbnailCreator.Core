using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests.IntegrationTests
{
    [Collection("Compression")]
    /// <summary>
    /// Integration tests.
    /// </summary>
    public class ThumbnailerCompressionLevelIntegrationTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailAndOriginalSaveFolder = Path.Combine(_testImageFolder, @"ProcessedImages");

        private Thumbnailer _thumbnailer = new Thumbnailer();
        private DirectoryFixture _fixture;

        public ThumbnailerCompressionLevelIntegrationTests(DirectoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [Trait("Category", "Integration")]
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
        public void CreateThumbnailWithSpecifiedCompressionSavesSuccesfully(string fileName, long compressionLevel)
        {
            //setup
            _fixture.TearDownTestDirectory();
            _fixture.SetupTestDirectory();

            string imageLocation = Path.Combine(_testImageFolder, fileName);
            IFormFile formFile = _fixture.ConvertFileToStream(imageLocation, _fixture.GetImageTypeEnum(fileName), fileName);

            //act
            string thumbnailSaveLocation = _thumbnailer.CreateAsync(100, _thumbnailAndOriginalSaveFolder, _thumbnailAndOriginalSaveFolder, formFile, compressionLevel).Result;

            string[] images = Directory.GetFiles(_thumbnailAndOriginalSaveFolder);

            //assert
            Assert.NotNull(thumbnailSaveLocation);
            Assert.Contains(fileName, thumbnailSaveLocation);
            Assert.True(images.Length == 2); // should be the original image and the thumbnail version

            //tear down
            _fixture.TearDownTestDirectory();
        }
    }
}