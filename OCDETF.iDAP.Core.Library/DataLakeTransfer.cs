using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class DataLakeTransfer : IDataTransfer
    {
        private readonly string endpoint;
        private readonly string key;
        private readonly string account;
        private readonly string container;
        private readonly string folder;


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
