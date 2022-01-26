using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library.Models
{
    public class InputParameters
    {
        public string apiURL { get; set; }
        public string appName { get; set; }
        public string category { get; set; }
        public string authorization { get; set; }
        public string query { get; set; }

        public string spacyEndpointURL { get; set; }
    }
}
