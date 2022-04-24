using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests.IntegrationTests
{
    [Collection("Directory")]
    /// <summary>
    /// Integration tests.
    /// </summary>
    public class ThumbnailerDirectoryIntegrationTests : IClassFixture<DirectoryFixture>
    {
        private static string _testImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestImages");
        private static string _thumbnailAndOriginalSaveFolder = Path.Combine(_testImageFolder, @"ProcessedImages");

        private Thumbnailer _thumbnailer = new Thumbnailer();
        private DirectoryFixture _fixture;

        public ThumbnailerDirectoryIntegrationTests(DirectoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void CreateNewThumbnailDirectoryCreatesDirectorySuccessfully()
        {
            // reset and setup test
            _fixture.TearDownTestDirectory();
            _thumbnailer.CheckAndCreateDirectory(_thumbnailAndOriginalSaveFolder);

            // assert
            Assert.True(Directory.Exists(_thumbnailAndOriginalSaveFolder));

            // Cleanup the unnecessary folders
            Directory.Delete(_thumbnailAndOriginalSaveFolder);
        }
    }
}