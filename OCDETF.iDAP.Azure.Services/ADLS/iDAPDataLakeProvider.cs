using Azure;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCDETF.iDAP.Azure.Services.ADLS
{
    public class iDAPDataLakeProvider : IDataLakeProvider
    {
        readonly DataLakeServiceClient dataLakeServiceClient;
        readonly int BUFFER_SIZE = 4096;
        public iDAPDataLakeProvider(string accountName, string accountKey, string serviceURI)
        {
            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);



        }
        public BinaryReader Download(string container, string folder, string fileName)
        {
            DataLakeFileClient fileClient = GetFileClient(container, folder, fileName);

            if (fileClient != null)
            {
                Response<FileDownloadInfo> downloadResponse = fileClient.Read();
                return new BinaryReader(downloadResponse.Value.Content);
            }
            else
                return null;
        }

        public bool Download(string container, string folder, string fileName, string destinationFilePath)
        {
            BinaryReader reader = Download(container, folder, fileName);
            bool result = true;
            if (reader != null)
            {
                FileStream fileStream = File.OpenWrite(destinationFilePath);
                byte[] buffer = new byte[BUFFER_SIZE];

                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    fileStream.Write(buffer, 0, count);
                }

                fileStream.Flush();
                fileStream.Close();
            }
            else
                result = false;

            return result;
        }

        private DataLakeFileClient GetFileClient(string container, string folder, string fileName)
        {            
            DataLakeFileClient fileClient = null;
            DataLakeDirectoryClient directory = GetDirectoryClient(container, folder);

            if (directory != null)
                fileClient = directory.GetFileClient(fileName);

            return fileClient;
        }

        private DataLakeDirectoryClient GetDirectoryClient(string container, string folder)
        {
            var dlContainer = dataLakeServiceClient.GetFileSystemClient(container.ToLower());
            DataLakeDirectoryClient directory = null;

            if (dlContainer.Exists())
            {
                directory = dlContainer.GetDirectoryClient(folder.ToLower());
                if (directory.Exists())
                {
                    return directory;
                }
            }

            return directory;
        }

        public IList<string> GetFiles(string container, string folder)
        {
            IList<string> result = new List<string>();

            var dlContainer = dataLakeServiceClient.GetFileSystemClient(container.ToLower());
            IEnumerator<PathItem> enumerator = dlContainer.GetPaths(folder).GetEnumerator();
            enumerator.MoveNext();
            PathItem item = enumerator.Current;

            while (item != null)
            {
                result.Add(item.Name);

                if (!enumerator.MoveNext())
                {
                    break;
                }

                item = enumerator.Current;
            }

            return result;
        }

        public bool Upload(string container, string folder, string uploadFileName, string uploadFileContents)
        {
            DataLakeFileClient fileClient = GetFileClient(container, folder, uploadFileName);
            if (fileClient != null)
            {
                fileClient.DeleteIfExists();
                fileClient.Upload(new MemoryStream(Encoding.ASCII.GetBytes(uploadFileContents)));
                return true;
            }
            return false;
        }
    }
}
