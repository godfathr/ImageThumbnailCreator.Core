using FluentAssertions;
using ImageThumbnailCreator.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Drawing.Imaging;
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

        private const string FileName = "ChuckNorrisOrderedAWhopperAtMcDonaldsAndGotOne.jpeg";
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

        [Fact(DisplayName = "SaveOriginalAsync happy path")]
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

        [Fact(DisplayName = "SaveOriginalAsync happy path", Skip = "Revisit this test after this method is refactored into more single responsibility.")]
        public async Task CreateAsync_ValidParameters_ReturnsExpectedString()
        {
            //setup
            var sut = GetThumbnailer();

            //act
            var result = await sut.CreateAsync(200L, DirectoryName, DirectoryName, _formFileMock.Object, 70L);

            //assert
            result.Should().NotBeNull();
            result.Should().Contain(DirectoryName).And.Contain(FileName);
        }

        [Fact]
        public void GetEncoderInfo_ValidMimeType_ReturnsExpectedEncoder()
        {
            //setup
            var sut = GetThumbnailer();

            //act
            var result = sut.GetEncoderInfo("image/jpeg");

            //assert
            result.Should().NotBeNull();
            result.CodecName.Should().BeEquivalentTo("Built-in JPEG Codec");
            result.FilenameExtension.Should().BeEquivalentTo("*.JPG;*.JPEG;*.JPE;*.JFIF");
        }

        private Thumbnailer GetThumbnailer()
        {
            return new Thumbnailer();
        }
    }
}
