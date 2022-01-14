using System;
using System.IO;

namespace OCDETF.iDAP.Core.Library
{
    public class CSVService
    {
        public CSVService() { }

        public void WriteLine(string filePath, string header)
        {            
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {                
                sw.WriteLine(header);
                sw.Close();
            }
        }      
    }
}
