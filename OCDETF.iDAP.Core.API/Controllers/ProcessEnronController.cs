using Microsoft.AspNetCore.Mvc;
using OCDETF.iDAP.Core.Library;
using OCDETF.iDAP.Enron.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Core.API.Controllers
{


    [ApiController]
    [Route("api/ProcessEnron")]
    public class ProcessEnronController : ControllerBase
    {

        [HttpPost]
        public string Process(DownloadParameters data)
        {

            //Comments
            string accountName = "kpidapv2";
            string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
            string serviceURI = "https://kpidapv2.blob.core.windows.net/";
            string createFileName = string.Empty;
            string result = string.Empty;

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

                result = $" {data.appName} {data.category} {data.query} {e.StackTrace} {Path.GetTempPath()}";
            }
            return result;

        }


        
    }
}
