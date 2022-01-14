using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Core.API
{
    public class DownloadParameters
    {
        public string apiURL { get; set; }
        public string appName { get; set; }
        public string category { get; set; }
        public string authorization { get; set; }
        public string query { get; set; }
    }
}
