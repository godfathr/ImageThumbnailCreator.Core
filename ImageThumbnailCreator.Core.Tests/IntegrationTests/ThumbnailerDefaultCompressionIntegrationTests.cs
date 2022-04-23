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
    public class ThumbnailerDefaultCompressionIntegrationTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailAndOriginalSaveFolder = Path.Combine(_testImageFolder, @"ProcessedImages");

        private Thumbnailer _thumbnailer = new Thumbnailer();
        private DirectoryFixture _fixture;

        public ThumbnailerDefaultCompressionIntegrationTests(DirectoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [Trait("Category", "Integration")]
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
        public void CreateThumbnailWithDefaultCompressionSavesSuccesfully(string fileName)
        {
            //setup
            _fixture.TearDownTestDirectory();
            _fixture.SetupTestDirectory();

            string imageLocation = Path.Combine(_testImageFolder, fileName);
            IFormFile formFile = _fixture.ConvertFileToStream(imageLocation, _fixture.GetImageTypeEnum(fileName), fileName);

            //act
            string thumbnailSaveLocation = _thumbnailer.Create(100, _thumbnailAndOriginalSaveFolder, _thumbnailAndOriginalSaveFolder, formFile).Result;
            string[] images = Directory.GetFiles(_thumbnailAndOriginalSaveFolder);

            //assert
            Assert.NotNull(thumbnailSaveLocation);
            Assert.Contains(fileName, thumbnailSaveLocation);
            Assert.True(images.Length == 2); // should be the original image and the compressed thumbnail version

            //tear down
            _fixture.TearDownTestDirectory();
        }
    }
}