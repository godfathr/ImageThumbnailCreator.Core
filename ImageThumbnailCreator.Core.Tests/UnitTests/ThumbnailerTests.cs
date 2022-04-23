using FluentAssertions;
using ImageThumbnailCreator.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ImageThumbnailCreator.Core.Tests.UnitTests
{
    [Trait("Category", "Unit")]
    public class ThumbnailerTests
    {
        private readonly Mock<IImageActionManager> _imageActionManagerMock = new Mock<IImageActionManager>();
        private readonly Mock<IFileManager> _fileManagerMock = new Mock<IFileManager>();
        private readonly Mock<IThumbnailer> _thumbnailerMock = new Mock<IThumbnailer>();
        private readonly Mock<IFormFile> _formFileMock = new Mock<IFormFile>();

        private const string FileName = "ChuckNorrisOrderedAWhopperAtMcDonaldsAndGotOne";
        private const string DirectoryName = "c:\\temp";

        public ThumbnailerTests()
        {
            _fileManagerMock
                .Setup(x => x.CheckAndCreateDirectory(It.IsAny<string>()));

            _fileManagerMock
                .Setup(x => x.SaveOriginalAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<string>());

            _formFileMock
                .Setup(x => x.FileName)
                .Returns(FileName);

            _formFileMock
                .Setup(x => x.ContentType)
                .Returns("image/jpeg");

            _formFileMock
                .Setup(x => x.Length)
                .Returns(1080);
        }

        [Fact(DisplayName = "SaveOriginalAsync Happy Path")]
        public async Task SaveOriginalAsync_ValidParameters_ReturnsExpectedString()
        {
            //setup
            var sut = GetThumbnailer();

            //act
            var result = await sut.SaveOriginalAsync(DirectoryName, _formFileMock.Object, CancellationToken.None);

            //assert
            result.Should().NotBeNull();
            result.Should().Contain(DirectoryName).And.Contain(FileName);
        }

        private Thumbnailer GetThumbnailer()
        {
            return new Thumbnailer();
        }
    }
}
