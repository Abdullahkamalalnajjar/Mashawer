using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mashawer.Data.Helpers
{
    public static class FileHelper
    {
        public static string SaveFile(IFormFile file, string folderName, IHttpContextAccessor httpContextAccessor)
        {
            if (file == null || file.Length == 0)
                return null;

            string uploadsFolder = Path.Combine("wwwroot", "Photos", folderName);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            Directory.CreateDirectory(uploadsFolder);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            // هنا بناخد الـ domain والـ scheme ديناميكي
            var request = httpContextAccessor.HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            string relativePath = Path.Combine("Photos", folderName, uniqueFileName);
            return $"{baseUrl}/{relativePath.Replace("\\", "/")}";
        }
    }
}
