using Microsoft.AspNetCore.Mvc;
using OCDETF.iDAP.Core.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OCDETF.iDAP.Core.API.Controllers
{
    [Route("api/Download")]
    [ApiController]
    public class DataLakeDownloadController : ControllerBase
    {

        string accountName = "kpidapv2";
        string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
        string serviceURI = "https://kpidapv2.blob.core.windows.net/";

        // POST api/<DataLakeDownloadController>
        [HttpPost]
        public void Post(DownloadParameters data)
        {
            string workingFolder = @"C:\Home";
            workingFolder = Path.Combine(workingFolder, "Output");
            if (!Directory.Exists(workingFolder))
                Directory.CreateDirectory(workingFolder);

            new DataLakeUploadService(accountName, accountKey, serviceURI).Download(data.appName, data.category, data.query);


        }

    }
}
