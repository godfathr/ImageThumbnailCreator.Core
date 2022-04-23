using ImageThumbnailCreator.Core;
using ImageThumbnailCreator.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageThumbnailCreator
{
    /// <summary>
    /// Implementation of directory management, image manipulation, compression and file management.
    /// </summary>
    public class Thumbnailer : IThumbnailer
    {
        /// <inheritdoc/>
        public async Task<string> SaveOriginalAsync(string imageFolder, IFormFile photo, CancellationToken cancellationToken = default)
        {
            try
            {
                string response = "";

                //check the file size is less than 16MB
                if (photo.Length > 16_777_216) //TODO: Make file size configurable
                {
                    response = "File size is too large. Must be less than 16MB.";
                }
                else
                {
                    var imageType = photo.ContentType;
                    if (ImageTypeEnum.ImageTypes.ContainsValue(imageType.ToString()))
                    {
                        string ticks = DateTime.Now.Ticks.ToString();

                        var fileName = Path.GetFileName($"{ticks}_{photo.FileName}");
                        CheckAndCreateDirectory(imageFolder); //make sure the destination folder exists before attempting to save

                        //TODO: Change this to write a stream to a file
                        //await photo.SaveAsAsync(Path.Combine(imageFolder, fileName));

                        if (photo.Length > 0)
                        {
                            string filePath = Path.Combine(imageFolder, fileName);
                            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(fileStream);
                                fileStream.Dispose(); // ensure stream is disposed
                            }

                            //var filePath = Path.GetTempFileName();

                            //using (var stream = System.IO.File.Create(filePath))
                            //{
                            //    await formFile.CopyToAsync(stream);
                            //}
                        }

                        response = Path.Combine(imageFolder, fileName);
                    }
                }

                return response; //TODO: If photo.length is not greater than 0, then we need to throw
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<string> CreateAsync(float width, string thumbnailImageFolder, string originalFileFolder, IFormFile photo, long compressionLevel = 85L)
        {
            //TODO: too many responsibilities in this method. Refactor to make more testable.
            if (width < 1) throw new ArgumentException($"The width parameter must be greater than 0.");
            if (string.IsNullOrEmpty(thumbnailImageFolder)) throw new ArgumentNullException(nameof(thumbnailImageFolder));
            if (string.IsNullOrEmpty(originalFileFolder)) throw new ArgumentNullException(nameof(originalFileFolder));
            if (photo == null) throw new ArgumentNullException(nameof(photo));
            if (!string.IsNullOrEmpty(thumbnailImageFolder))
            {
                CheckAndCreateDirectory(thumbnailImageFolder); //make sure the directory exists
            }

            //First, save the original file then validate that we get a file location
            string originalFileLocation = await SaveOriginalAsync(thumbnailImageFolder, photo);
            if (string.IsNullOrEmpty(originalFileLocation))
            {
                throw new Exception("Original file location is invalid or the original file failed to save.");
            }

            Bitmap thumbnail;

            try
            {
                //read the bytes
                Bitmap srcImage = new Bitmap(originalFileLocation);

                float imageWidth = srcImage.Width;
                float imageHeight = srcImage.Height;

                if (width > imageWidth)
                {
                    throw new Exception("Thumbnail width can not be greater than the original image width.");
                }

                List<int> propertyIdList = new List<int>();
                if (srcImage.PropertyIdList != null)
                {
                    propertyIdList = srcImage.PropertyIdList.ToList();
                }

                RotateFlipType rotationFlipType = OrientUpright(propertyIdList, srcImage);

                Tuple<float, float> dimensions = SetDimensions(width, ref imageWidth, ref imageHeight);

                // TODO: update to use new FileInfo
                string thumbnailFileName = originalFileLocation.Split('\\').Last().ToString();
                var newWidth = (int)dimensions.Item2;
                var newHeight = (int)dimensions.Item1;
                thumbnail = new Bitmap(newWidth, newHeight);
                var graphics = Graphics.FromImage(thumbnail);

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.RotateTransform(0);
                //graphics.CompositingQuality = CompositingQuality.HighSpeed;

                // Now calculate the X,Y position of the upper-left corner
                // (one of these will always be zero)
                int posX = Convert.ToInt32((imageWidth - (newWidth)) / 2);
                int posY = Convert.ToInt32((imageHeight - (newHeight)) / 2);

                graphics.DrawImage(srcImage,
                        new Rectangle(posX, posY, newWidth, newHeight),
                        new Rectangle(0, 0, srcImage.Width, srcImage.Height),
                        GraphicsUnit.Pixel);

                thumbnail.RotateFlip(rotationFlipType);
                string thumbPath;
                using (thumbnail)
                {
                    //Save the thumbnail to the file system.
                    thumbPath = SaveThumbnail(thumbnail, thumbnailImageFolder, thumbnailFileName, compressionLevel);
                }

                //clean up
                srcImage.Dispose();
                thumbnail.Dispose();
                graphics.Dispose();

                return thumbPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <inheritdoc/>
        public Tuple<float, float> SetDimensions(float width, ref float imageWidth, ref float imageHeight)
        {
            if (width < 1) throw new ArgumentException($"The width parameter must be greater than 0.");

            try
            {
                //compute the thumbnail height
                if (imageWidth > imageHeight)
                {
                    //landscape images
                    imageHeight = imageHeight * ((width / imageWidth));
                    imageWidth = width;
                }
                else if (imageWidth < imageHeight)
                {
                    //portrait images
                    imageHeight = imageHeight * ((width / imageWidth));
                    imageWidth = width;
                }
                else
                {
                    imageHeight = width;
                    imageWidth = width;
                }

                return new Tuple<float, float>(imageHeight, imageWidth);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public RotateFlipType OrientUpright(List<int> propertyIdList, Image srcImage)
        {
            if (srcImage == null) throw new ArgumentNullException(nameof(srcImage));

            try
            {
                int exifOrientationID = 0x112;

                PropertyItem prop = null;

                var ids = srcImage.PropertyIdList.ToList();

                if (ids.Contains(exifOrientationID))
                {
                    prop = srcImage.GetPropertyItem(exifOrientationID);
                }

                var rot = RotateFlipType.RotateNoneFlipNone;

                //determine the orientation of the image from the EXIF orientation ID
                if (prop != null)
                {
                    int val = BitConverter.ToUInt16(prop.Value, 0);

                    if (val == 3 || val == 4)
                        rot = RotateFlipType.Rotate180FlipNone;
                    else if (val == 5 || val == 6)
                        rot = RotateFlipType.Rotate90FlipNone;
                    else if (val == 7 || val == 8)
                        rot = RotateFlipType.Rotate270FlipNone;
                }

                return rot;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public string SaveThumbnail(Bitmap thumbnail, string imagePath, string thumbnailFileName, long compressionLevel = 85L)
        {
            if (thumbnail == null) throw new ArgumentNullException(nameof(thumbnail));
            if (string.IsNullOrEmpty(imagePath)) throw new ArgumentNullException(nameof(imagePath));
            if (string.IsNullOrEmpty(thumbnailFileName)) throw new ArgumentNullException(nameof(thumbnailFileName));

            try
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;

                string thumbPath = Path.Combine(imagePath, $"uncompressed_{thumbnailFileName}");

                using (thumbnail)
                {
                    thumbnail.Save($"{thumbPath}"); //we have to save the newly created file to the file system for the next step where we use an encoder to compress the image
                }

                thumbnail.Dispose();

                Bitmap compressedThumbnail = new Bitmap(thumbPath);

                myImageCodecInfo = GetEncoderInfo("image/jpeg");

                myEncoder = System.Drawing.Imaging.Encoder.Quality;

                myEncoderParameters = new EncoderParameters(1);

                myEncoderParameter = new EncoderParameter(myEncoder, compressionLevel); //compress the image to decrease the file size

                myEncoderParameters.Param[0] = myEncoderParameter;

                var originalThumbnail = Path.GetDirectoryName(thumbPath);

                //string newThumbPath = thumbPath.Split('\\').Last().ToString(); //TODO: Left off - need to point the compressed thumbnail at the same location the ucompressed was saved
                //string newThumbPath2 = new FileInfo(thumbPath).Directory.FullName;

                compressedThumbnail.Save($"{imagePath}\\thumb_{thumbnailFileName}", myImageCodecInfo, myEncoderParameters);

                compressedThumbnail.Dispose();

                //Remove the original uncompressed thumbnail since we only needed it on the file system instead of in memory
                DirectoryInfo di = new DirectoryInfo(imagePath);

                FileInfo uncompressedThumbnail = di.GetFiles()
                    .Where(f => f.Name.Equals($"uncompressed_{ thumbnailFileName}"))
                    .FirstOrDefault();
                if (uncompressedThumbnail != null && uncompressedThumbnail.Exists)
                {
                    uncompressedThumbnail.Delete();
                }

                return thumbPath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a <see cref="ImageCodecInfo"/> for a given mimeType.
        /// </summary>
        /// <param name="mimeType">String name for an <see cref="ImageCodecInfo.MimeType"/></param>
        /// <returns><see cref="ImageCodecInfo"/></returns>
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        ///<inheritdoc />
        public void CheckAndCreateDirectory(string imageFolderPath)
        {
            if (string.IsNullOrWhiteSpace(imageFolderPath))
                throw new ArgumentNullException(nameof(imageFolderPath));
            try
            {
                if (!Directory.Exists(imageFolderPath))
                    Directory.CreateDirectory(imageFolderPath);
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }
    }
}