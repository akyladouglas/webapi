using System.Collections.Generic;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class GetMimeTypes
    {
        public Dictionary<string, string> ReturnMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "application/zip"},
                {".7z", "application/x-7z-compressed"},
                {".gz", "application/x-gzip"},
            };
        }
    }
}
