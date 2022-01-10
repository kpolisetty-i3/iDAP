using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using System.Text;
using Azure.AI.TextAnalytics;
using Azure;

namespace LoadCognitiveData
{
    public static class ProcessCognitiveData
    {
        [FunctionName("ProcessCognitiveData")]
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
            CognitiveParameters data = JsonConvert.DeserializeObject<CognitiveParameters>(requestBody);
            StringBuilder result = new StringBuilder();
            
            try
            {

                if (string.IsNullOrEmpty(data.appName) || string.IsNullOrEmpty(data.category))
                {
                    errorMessage = "apiURL, appName and category are required parameters";
                }
                else
                {
                    var directory = CreateDataLakeDirectory(accountName, accountKey, serviceURI, data);
                   
                    foreach (PathItem pathItem in directory.GetPaths())
                    {
                        if ((pathItem.Name.ToLower().Contains(".txt") || pathItem.Name.ToLower().Contains(".html")) && !pathItem.Name.Contains("_keyphrases.") &&  !pathItem.Name.Contains("_linkedEntities.") && !pathItem.Name.Contains("_entities."))
                        {
                            string fileName = Path.GetFileName(pathItem.Name);
                            var fileHandle = directory.GetFileClient(fileName);
                            Stream fileStream = fileHandle.OpenRead();
                            byte[] readBytes = new byte[fileStream.Length];
                            int numberBytesToRead = (int)fileStream.Length;
                            int output = fileStream.Read(readBytes, 0, numberBytesToRead);
                            result.AppendLine(Encoding.ASCII.GetString(readBytes));

                            StringBuilder result2 = CognitiveServiceKeyPhrases(result);
                            SaveFileIntoDataLake(accountName, accountKey, serviceURI, data, fileName.Replace(".", "_keyphrases."), result2);

                            StringBuilder result3 = CognitiveServiceLinkedEntities(result);
                            SaveFileIntoDataLake(accountName, accountKey, serviceURI, data, fileName.Replace(".", "_linkedEntities."), result3);

                            StringBuilder result4 = CognitiveServiceKeyEntities(result);
                            SaveFileIntoDataLake(accountName, accountKey, serviceURI, data, fileName.Replace(".", "_entities."), result4);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message + Environment.NewLine + $"aParameters {data.appName}, {data.category}!!" + e.StackTrace;
            }


            string responseMessage = string.IsNullOrEmpty(errorMessage)
                ? $"Result: {result.ToString()}"
                : errorMessage;

            return new OkObjectResult(responseMessage);
        }

        public static DataLakeDirectoryClient CreateDataLakeDirectory(string accountName, string accountKey, string serviceURI, CognitiveParameters data)
        {
            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);

            var container = dataLakeServiceClient.GetFileSystemClient(data.appName.ToLower());

            container.CreateIfNotExists();

            var directory = container.GetDirectoryClient(data.category.ToLower());
            directory.CreateIfNotExists();

            return directory;
        }

        public static StringBuilder CognitiveServiceKeyPhrases(StringBuilder text)
        {
            string endpoint = "https://idapv2-cognitive.cognitiveservices.azure.com/";
            AzureKeyCredential credentials = new AzureKeyCredential("7a5f7b23e66748f2a32a1d84348605f8");
            var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
            StringBuilder keyPhrases = new StringBuilder();

            var response = client.ExtractKeyPhrases(text.ToString());
            foreach (string keyphrase in response.Value)
            {
                keyPhrases.AppendLine(keyphrase);
            }
            return keyPhrases;
        }

        public static StringBuilder CognitiveServiceKeyEntities(StringBuilder text)
        {
            string endpoint = "https://idapv2-cognitive.cognitiveservices.azure.com/";
            AzureKeyCredential credentials = new AzureKeyCredential("7a5f7b23e66748f2a32a1d84348605f8");
            var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
            StringBuilder keyPhrases = new StringBuilder();

            var response = client.RecognizeEntities(text.ToString());
            foreach (CategorizedEntity keyphrase in response.Value)
            {
                keyPhrases.AppendLine($" {keyphrase.Text} \t\t {keyphrase.ConfidenceScore} \t\t {keyphrase.Category} \t\t {keyphrase.SubCategory} ");
            }
            return keyPhrases;
        }

        public static StringBuilder CognitiveServiceLinkedEntities(StringBuilder text)
        {
            string endpoint = "https://idapv2-cognitive.cognitiveservices.azure.com/";
            AzureKeyCredential credentials = new AzureKeyCredential("7a5f7b23e66748f2a32a1d84348605f8");
            var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
            StringBuilder linkedEntities = new StringBuilder();

            var response = client.RecognizeLinkedEntities(text.ToString());
            foreach (var entity in response.Value)
            {
                linkedEntities.AppendLine($"\tName: {entity.Name},\tID: {entity.DataSourceEntityId},\tURL: {entity.Url}\tData Source: {entity.DataSource}");
                linkedEntities.AppendLine("\tMatches:");
                foreach (var match in entity.Matches)
                {
                    linkedEntities.AppendLine($"\t\tText: {match.Text}");
                    linkedEntities.AppendLine($"\t\tScore: {match.ConfidenceScore:F2}\n");
                }
            }
            return linkedEntities;
        }

        public static void SaveFileIntoDataLake(string accountName, string accountKey, string serviceURI, CognitiveParameters data, string fileName, StringBuilder text)
        {
            StorageSharedKeyCredential keyCredentials = new StorageSharedKeyCredential(accountName, accountKey);
            DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(new Uri(serviceURI), keyCredentials);

            var container = dataLakeServiceClient.GetFileSystemClient(data.appName.ToLower());

            //container.CreateIfNotExists();

            var directory = container.GetDirectoryClient(data.category.ToLower());
            //directory.CreateIfNotExists();

            DataLakeFileClient fileClient = directory.GetFileClient(fileName);
            fileClient.DeleteIfExists();
            fileClient.CreateIfNotExists();

            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(text.ToString()));

            fileClient.Append(memStream, offset: 0);
            fileClient.Flush(position: memStream.Length);
            
        }
    }
    public class CognitiveParameters
    {        
        public string appName { get; set; }
        public string category { get; set; }
        public string authorization { get; set; }
        public string query { get; set; }
    }
}
