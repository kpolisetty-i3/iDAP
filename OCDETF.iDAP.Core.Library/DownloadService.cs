using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Core.Library
{
    public class DownloadService
    {
        public DownloadService() { }

        public bool Download(string apiURL, string destinationFilePath)
        {
            try
            {
                UriBuilder builder = new UriBuilder(apiURL);
                HttpClient client = new HttpClient();
                var contentBytes = client.GetByteArrayAsync(builder.Uri).Result;
                MemoryStream stream = new MemoryStream(contentBytes);
                FileStream file = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
