using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCDETF.iDAP.Core.Library.PDF;
using OCDETF.iDAP.Azure.Services.ADLS;
using System.Collections.Generic;
using OCDETF.iDAP.Core.Library.Models;

namespace OCDETF.iDAP.Azure.Functions
{
    public static class ProcessPDFs
    {
        [FunctionName("ProcessPDFs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            //Comments
            string accountName = "kpidapv2";
            string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
            string serviceURI = "https://kpidapv2.blob.core.windows.net/";
            string result = string.Empty;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            InputParameters data = JsonConvert.DeserializeObject<InputParameters>(requestBody);

            try
            {
                IDataLakeProvider dataLakeProvider = new iDAPDataLakeProvider(accountName, accountKey, serviceURI);
                IList<string> filesToProcess = dataLakeProvider.GetFiles(data.appName, data.category);

                foreach (string file in filesToProcess)
                {
                    if (Path.GetExtension(file).ToLower() == ".pdf")
                    {

                        BinaryReader reader = dataLakeProvider.Download(data.appName, data.category, file);

                        IPDFProvider pdfProvider = new iTextProvider(reader);
                        int Pages = pdfProvider.GetPageCount();

                        for (int i = 1; i <= Pages; i++)
                        {
                            string pageContent = pdfProvider.GetContents(1);
                        }
                    }
                }
                                
            }
            catch (Exception e)
            {

                result = $" {data.appName} {data.category} {data.query} {e.StackTrace} {Path.GetTempPath()} {e.Message}";
            }

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }


}
