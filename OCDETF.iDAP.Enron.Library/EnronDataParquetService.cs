using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Enron.Library
{
    public class EnronDataParquetService
    {
        public EnronDataParquetService() { }

        public void Process(string zipFile)
        {
            var files = ZipFile.OpenRead(zipFile);

            foreach (ZipArchiveEntry file in files.Entries)
            {
                
            }
        }
    }
}
