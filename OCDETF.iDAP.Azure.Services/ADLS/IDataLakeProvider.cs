using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCDETF.iDAP.Azure.Services.ADLS
{
    public interface IDataLakeProvider
    {
        BinaryReader Download(string container, string folder, string fileName);

        bool Download(string container, string folder, string fileName, string destinationFilePath);

        bool Upload(string container, string folder, string uploadFileName, string uploadFileContents);

        IList<string> GetFiles(string container, string folder);
    }
}
