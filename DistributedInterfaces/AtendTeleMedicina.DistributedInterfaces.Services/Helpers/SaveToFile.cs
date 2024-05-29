using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class SaveToFile
    {
        public void SaveFile(byte[] file, string url, string formato, IHostingEnvironment _environment)
        {

            if (file == null) return;
            var path = $"{_environment.WebRootPath}";
            var filename = "";
            if (formato == ".pdf")
            {
                filename = path + url + ".pdf";
            }
            else if (formato == ".jpeg")
            {
                filename = path + url + ".jpeg";
            }
            else if (formato == ".jpg")
            {
                filename = path + url + ".jpg";
            }
            else if (formato == ".png")
            {
                filename = path + url + ".png";
            }
            else if (formato == ".mp3")
            {
                filename = path + url + ".mp3";
            }
            else if (formato == ".wav")
            {
                filename = path + url + ".wav";
            }
            else if (formato == ".xlsx")
            {
                filename = path + url + ".xlsx";
            }
            else
            {

            }

            try
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(file);
                    fs.Close();
                }

            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}
