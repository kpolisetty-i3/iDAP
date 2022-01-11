using System;
using System.IO;

namespace OCDETF.iDAP.Core.Library
{
    public class CSVService
    {
        public CSVService() { }

        public void WriteHeader(string filePath, string header)
        {
            File.Delete(filePath);
            using (StreamWriter sw = File.AppendText(filePath))
            {                
                sw.WriteLine(header);
            }
        }
    }
}
