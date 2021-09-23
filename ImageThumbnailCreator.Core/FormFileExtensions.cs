using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.Extensions
{
    /// <summary>
    /// DEPRECATED.Extracted extension methods for an IFormFile.
    /// Not valid in after version 
    /// </summary>
    [Obsolete]
    public static class IFormFileExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> SaveAsAsync(this IFormFile formFile, string path)
        {
            try
            {
                FileStream destinationStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await formFile.CopyToAsync(destinationStream);

                // Read the source file into a byte array.
                byte[] bytes = new byte[destinationStream.Length];

                int totalBytes = (int)destinationStream.Length;
                int bytesToRead = 0;

                while (totalBytes > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = destinationStream.Read(bytes, bytesToRead, totalBytes);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    bytesToRead += n;
                    totalBytes -= n;
                }
                totalBytes = bytes.Length;

                // Write the byte array to the other FileStream.
                using (destinationStream)
                {
                    destinationStream.Write(bytes, 0, totalBytes);
                }

                return path;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
