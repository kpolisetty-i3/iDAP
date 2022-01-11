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

            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);

            var container = dataLakeServiceClient.GetFileSystemClient(data.appName.ToLower());

            container.CreateIfNotExists();

            var directory = container.GetDirectoryClient(data.category.ToLower());
            directory.CreateIfNotExists();

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static async Task<string> GetEnronData(InputParameters data, DataLakeDirectoryClient directory)
        {
            string createFileName = string.Empty;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(data.apiURL))
                {
                    string stagingDirectory = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString());
                    Directory.CreateDirectory(stagingDirectory);
                    createFileName = string.Format("File-" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss")) + ".json";
                    string fileExtension = Path.GetExtension(data.apiURL);

                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        createFileName = Path.GetFileName(data.apiURL);
                    }


                    string apiResponse = await response.Content.ReadAsStringAsync();
                    File.WriteAllText(Path.Combine(stagingDirectory, createFileName), apiResponse);


                    DataLakeFileClient fileClient = directory.GetFileClient(createFileName);
                    fileClient.CreateIfNotExists();

                    FileStream fileStream = File.OpenRead(Path.Combine(stagingDirectory, createFileName));
                    //long fileSize = fileStream.Length;
                    //fileClient.Append(fileStream, offset: 0);
                    //fileClient.Flush(position: fileSize);

                    ZipFile.ExtractToDirectory(createFileName, Path.GetTempPath());
                    

                    //File.Delete(localFilePathWithName);
                }
            }

            return createFileName;
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
