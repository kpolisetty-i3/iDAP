using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library.PDF
{
    interface IPDFProvider
    {
        string GetContents(int pageNumber);
    }
}
