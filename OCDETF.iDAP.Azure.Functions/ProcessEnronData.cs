using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Azure.Storage.Files.DataLake;
using Azure.Storage;
using System.IO.Compression;
using Azure.Storage.Files.DataLake.Models;
using Azure;

namespace OCDETF.iDAP.Azure.Functions
{
    public static class ProcessEnronData
    {
        [FunctionName("ProcessEnronData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];

            string accountName = "kpidapv2";
            string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
            string serviceURI = "https://kpidapv2.blob.core.windows.net/";

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            InputParameters data = JsonConvert.DeserializeObject<InputParameters>(requestBody);

            string stagingDirectory = Path.GetTempPath();
            string zipFilePath = Path.Combine(stagingDirectory, "enron-dataset.zip");

            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);

            var container = dataLakeServiceClient.GetFileSystemClient(data.appName.ToLower());
            container.CreateIfNotExists();

            var directory = container.GetDirectoryClient(data.category.ToLower());
            directory.CreateIfNotExists();

            DataLakeFileClient fileClient = directory.GetFileClient("enron-dataset.zip");

            Response<FileDownloadInfo> downloadResponse = fileClient.Read();

            BinaryReader reader = new BinaryReader(downloadResponse.Value.Content);

            FileStream fileStream = File.OpenWrite(zipFilePath);

            int bufferSize = 4096;

            byte[] buffer = new byte[bufferSize];
            int count;

            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
            {
                fileStream.Write(buffer, 0, count);
            }

            await fileStream.FlushAsync();

            fileStream.Close();

            ZipFile.ExtractToDirectory(zipFilePath, Path.GetTempPath());
            

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {Directory.Exists(Path.Combine(Path.GetTempPath(), "2018487913", "maildir"))}. This HTTP triggered function executed successfully and unzipped.";

            return new OkObjectResult(responseMessage);
        }

    }

    public class InputParameters
    {
        public string apiURL { get; set; }
        public string appName { get; set; }
        public string category { get; set; }
        public string authorization { get; set; }
        public string query { get; set; }
    }
}
