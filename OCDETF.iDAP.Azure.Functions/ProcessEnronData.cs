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
using OCDETF.iDAP.Core.Library;
using OCDETF.iDAP.Enron.Library;

namespace OCDETF.iDAP.Azure.Functions
{
    public static class ProcessEnronData
    {
        [FunctionName("ProcessEnronData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Comments
            string accountName = "kpidapv2";
            string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
            string serviceURI = "https://kpidapv2.blob.core.windows.net/";
            string createFileName = string.Empty;
            string result = string.Empty;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            InputParameters data = JsonConvert.DeserializeObject<InputParameters>(requestBody);

            try
            {

                new DataLakeUploadService(accountName, accountKey, serviceURI).Download(data.appName, data.category, data.query);
                ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(), data.query), Path.GetTempPath());

                string inputDirectory = Path.Combine(Path.GetTempPath(), "2018487913", "maildir");
                string outputDirectory = Path.Combine(Path.GetTempPath(), "2018487913", "Output");

                new EnronDataCSVService().Process(inputDirectory, outputDirectory);

                foreach (string file in Directory.GetFiles(outputDirectory))
                    new DataLakeUploadService(accountName, accountKey, serviceURI).Upload(data.appName, data.category, file);
            }
            catch (Exception e)
            {

                result = $" {data.appName} {data.category} {data.query} {e.StackTrace} {Path.GetTempPath()} {e.Message}";
            }

            return new OkObjectResult(result);
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
