using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests.IntegrationTests
{
    [Collection("OriginalFile")]
    [Trait("Category", "Integration")]
    /// <summary>
    /// Integration tests.
    /// </summary>
    public class ThumbnailerOriginalFileIntegrationTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailAndOriginalSaveFolder = Path.Combine(_testImageFolder, @"ProcessedImages");

        private Thumbnailer _thumbnailer = new Thumbnailer();
        private DirectoryFixture _fixture;

        public ThumbnailerOriginalFileIntegrationTests(DirectoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [Trait("Category", "Integration")]
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
        public void SaveOriginalSavesSuccessfully(string fileName)
        {
            //setup
            _fixture.TearDownTestDirectory();
            _fixture.SetupTestDirectory();
            
            string imageLocation = Path.Combine(_testImageFolder, fileName);
            IFormFile formFile = _fixture.ConvertFileToStream(imageLocation, _fixture.GetImageTypeEnum(fileName), fileName);

            //act
            string originalFileSaveLocation =  _thumbnailer.SaveOriginalAsync(_thumbnailAndOriginalSaveFolder, formFile).Result;
            string[] images = Directory.GetFiles(_thumbnailAndOriginalSaveFolder);

            //assert
            Assert.NotNull(originalFileSaveLocation);
            Assert.Contains(fileName, originalFileSaveLocation);
            Assert.True(images.Length == 1);
            Assert.Single(images);

            //tear down
            _fixture.TearDownTestDirectory();
        }
    }
}