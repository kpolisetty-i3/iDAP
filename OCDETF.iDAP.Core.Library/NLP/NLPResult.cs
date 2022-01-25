using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Core.Library.NLP
{
    public class NLPResult
    {
        public int End { get; set; }
        public int Start { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
    }
}
