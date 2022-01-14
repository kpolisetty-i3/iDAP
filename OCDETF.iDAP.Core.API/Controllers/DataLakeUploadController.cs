using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCDETF.iDAP.Core.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Core.API.Controllers
{
    [Route("api/Upload")]
    [ApiController]
    public class DataLakeUploadController : ControllerBase
    {

        [HttpPost]
        public string Upload(DownloadParameters data)
        {
            string accountName = "kpidapv2";
            string accountKey = "L56P4ZOvy5zvYKCI/gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==";
            string serviceURI = "https://kpidapv2.blob.core.windows.net/";
            string createFileName = string.Empty;
            string result = string.Empty;



            try
            {

                if (string.IsNullOrEmpty(data.apiURL) || string.IsNullOrEmpty(data.appName) || string.IsNullOrEmpty(data.category))
                {
                    result = "apiURL, appName and category are required parameters";
                }
                else
                {
                    Uri requestUri = new Uri(data.apiURL);
                    createFileName = Path.GetFileName(requestUri.OriginalString);
                    if(!string.IsNullOrEmpty(requestUri.Query))
                        createFileName = Path.GetFileName(requestUri.OriginalString.Replace(requestUri.Query, string.Empty));
                    new DownloadService().Download(data.apiURL, Path.Combine(Path.GetTempPath(), createFileName));

                    new DataLakeUploadService(accountName, accountKey, serviceURI).Upload(data.appName, data.category, Path.Combine(Path.GetTempPath(), createFileName));
                    result = $"Hello, This HTTP triggered function executed successfully created {createFileName} from {data.apiURL}, {data.appName}, {data.category} {Path.Combine(Path.GetTempPath(), createFileName)}!!";
                }
            }
            catch (Exception e)
            {
                result = e.Message + " Stack Trace:" + e.StackTrace;
            }

            return result;
            
        }
    }
}
