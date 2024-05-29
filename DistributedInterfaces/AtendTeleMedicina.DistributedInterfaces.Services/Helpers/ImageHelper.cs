using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class ImageHelper
    {
        public static string SaveToFile(string imageString, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string image = fileName + ".jpg";
            string imgPath = Path.Combine(path, image);

            string[] str = imageString.Split(',');
            byte[] bytes = Convert.FromBase64String(str[1]);

            File.WriteAllBytes(imgPath, bytes);

            return image;
        }

        public static void SaveFromBytes(byte[] imageString, string imgName, string path)
        {
            var base64 = Convert.ToBase64String(imageString);
            var imgSrc = String.Format("data:image/jpeg;base64,{0}", base64);
            SaveToFile(imgSrc, imgName, path);
        }

    }
}
