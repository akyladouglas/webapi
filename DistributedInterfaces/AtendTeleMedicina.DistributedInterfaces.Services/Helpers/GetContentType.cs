using System.IO;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class GetContentType
    {

        public string ReturnContentType(string path)
        {
            GetMimeTypes getMimeTypes = new GetMimeTypes();
            var types = getMimeTypes.ReturnMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
    }
}
