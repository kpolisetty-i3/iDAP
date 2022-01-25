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
                string workingFolder = @"C:\Home";
                workingFolder = Path.Combine(workingFolder, "Output");
                if (!Directory.Exists(workingFolder))
                    Directory.CreateDirectory(workingFolder);

                new DataLakeUploadService(accountName, accountKey, serviceURI).Download(data.appName, data.category, data.query);
                
                string zipFileName = Path.Combine(workingFolder, data.query);

                new EmailParser().Parse(zipFileName, workingFolder, 6, new ParquetFileWriter(),
                    new DataLakeTransfer("kpidapv2",
                    "L56P4ZOvy5zvYKCI /gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==",
                    "https://kpidapv2.blob.core.windows.net/",
                    "idapv2",
                    "enron"));
            }
            catch (Exception e)
            {

                result = $" {data.appName} {data.category} {data.query} {e.StackTrace} {Path.GetTempPath()}";
            }
            return result;

        }



    }
}
