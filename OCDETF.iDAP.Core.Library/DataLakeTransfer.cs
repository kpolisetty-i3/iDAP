using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class DataLakeTransfer : IDataTransfer
    {
        private string endpoint;
        private string key;
        private string account;
        private string container;
        private string folder;


        public DataLakeTransfer(string acct, string accesskey, string endpointuri, string container, string folder)
        {
            this.endpoint = endpointuri;
            this.account = acct;
            this.key = accesskey;
            this.container = container;
            this.folder = folder;
        }

        public bool Transfer(string localFilePath)
        {
            new DataLakeUploadService(this.account, this.key, this.endpoint).Upload(this.container, this.folder, localFilePath);
            return true;
        }
    }
}
