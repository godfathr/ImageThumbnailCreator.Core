using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageThumbnailCreator.Core.Extensions
{
    public static class IFormFileExtensions
    {
        public static string SaveAs(this IFormFile formFile, string imageFolder, string fileName)
        {
            return "Hello World";
        }
    }
}
