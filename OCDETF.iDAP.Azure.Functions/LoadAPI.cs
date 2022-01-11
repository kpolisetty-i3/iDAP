using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadCognitiveData
{
    public static class LoadAPI
    {
        [FunctionName("LoadAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            string accountName = "kpidapv2";
            string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
            string serviceURI = "https://kpidapv2.blob.core.windows.net/";
            string createFileName = string.Empty;
            string errorMessage = string.Empty;


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            InputParameters data = JsonConvert.DeserializeObject<InputParameters>(requestBody);

            try
            {

                if (string.IsNullOrEmpty(data.apiURL) || string.IsNullOrEmpty(data.appName) || string.IsNullOrEmpty(data.category))
                {
                    errorMessage = "apiURL, appName and category are required parameters";
                }
                else
                {

                    var directory = CreateDataLakeDirectory(accountName, accountKey, serviceURI, data);
                    createFileName = await DownloadAPItoDataLake(createFileName, data, directory);
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message + Environment.NewLine + $"aPrameters {createFileName} from {data.apiURL}, {data.appName}, {data.category}!!" + e.StackTrace;
            }


            string responseMessage = string.IsNullOrEmpty(errorMessage)
                ? $"Hello, This HTTP triggered function executed successfully created {createFileName} from {data.apiURL}, {data.appName}, {data.category} {Path.GetFileName(data.apiURL)}!!"
                : errorMessage;

            return new OkObjectResult(responseMessage);
        }

        private static async Task<string> DownloadAPItoDataLake(string createFileName, InputParameters data, DataLakeDirectoryClient directory)
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(data.authorization))
                    httpClient.DefaultRequestHeaders.Add("Authorization", data.authorization);


                using (var response = await httpClient.GetAsync(data.apiURL))
                {
                    string stagingDirectory = Path.GetTempPath();
                    createFileName = string.Format("File-" + DateTime.Now.ToString("yyyyMMdd HHmmss")) + ".json";
                    string fileExtension = Path.GetExtension(data.apiURL);

                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        createFileName = Path.GetFileName(data.apiURL);
                        if (createFileName.IndexOf("?") > 0)
                            createFileName = createFileName.Substring(0, createFileName.IndexOf("?"));
                    }


                    string apiResponse = await response.Content.ReadAsStringAsync();
                    File.WriteAllText(Path.Combine(stagingDirectory, createFileName), apiResponse);

                    

                    DataLakeFileClient fileClient = directory.GetFileClient(createFileName);
                    fileClient.CreateIfNotExists();

                    FileStream fileStream = File.OpenRead(Path.Combine(stagingDirectory, createFileName));
                    long fileSize = fileStream.Length;
                    fileClient.Append(fileStream, offset: 0);
                    fileClient.Flush(position: fileSize);



                    //File.Delete(localFilePathWithName);
                }
            }

            return createFileName;
        }

        private static async Task<string> DownloadAPItoDataLakeNew(string createFileName, InputParameters data, DataLakeDirectoryClient directory)
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(data.authorization))
                    httpClient.DefaultRequestHeaders.Add("Authorization", data.authorization);


                using (var response = await httpClient.GetAsync(data.apiURL))
                {
                    string stagingDirectory = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString());
                    Directory.CreateDirectory(stagingDirectory);
                    createFileName = string.Format("File-" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss")) + ".json";
                    string fileExtension = Path.GetExtension(data.apiURL);

                    if (!string.IsNullOrEmpty(Path.GetExtension(data.apiURL)))
                    {
                        createFileName = Path.GetFileName(data.apiURL);
                    }


                    string apiResponse = await response.Content.ReadAsStringAsync();
                    File.WriteAllText(Path.Combine(stagingDirectory, createFileName), apiResponse);


                    DataLakeFileClient fileClient = directory.GetFileClient(createFileName);
                    fileClient.CreateIfNotExists();

                    FileStream fileStream = File.OpenRead(Path.Combine(stagingDirectory, createFileName));
                    long fileSize = fileStream.Length;
                    fileClient.Append(fileStream, offset: 0);
                    fileClient.Flush(position: fileSize);



                    //File.Delete(localFilePathWithName);
                }
            }

            return createFileName;
        }

        public static DataLakeDirectoryClient CreateDataLakeDirectory(string accountName, string accountKey, string serviceURI, InputParameters data)
        {
            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);

            var container = dataLakeServiceClient.GetFileSystemClient(data.appName.ToLower());

            container.CreateIfNotExists();

            var directory = container.GetDirectoryClient(data.category.ToLower());
            directory.CreateIfNotExists();

            return directory;
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
