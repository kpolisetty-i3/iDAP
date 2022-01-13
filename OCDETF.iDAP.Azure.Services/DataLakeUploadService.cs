﻿using Azure.Storage;
using Azure.Storage.Files.DataLake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Core.Library
{
    public class DataLakeUploadService
    {
        private readonly string accountName;
        private readonly string accountKey;
        private readonly string serviceURI;
        public DataLakeUploadService(string account, string key, string uri) {
            this.accountKey = key;
            this.accountName = account;
            this.serviceURI = uri;
        }

        public void Upload(string container, string folder, string fileNamePath)
        {
            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);

            var dlContainer = dataLakeServiceClient.GetFileSystemClient(container.ToLower());

            dlContainer.CreateIfNotExists();

            var directory = dlContainer.GetDirectoryClient(folder.ToLower());
            directory.CreateIfNotExists();


            DataLakeFileClient fileClient = directory.GetFileClient(Path.GetFileName(fileNamePath));
            fileClient.CreateIfNotExists();

            FileStream fileStream = File.OpenRead(fileNamePath);
            long fileSize = fileStream.Length;
            fileClient.Append(fileStream, offset: 0);
            fileClient.Flush(position: fileSize);
        }
    }
}
