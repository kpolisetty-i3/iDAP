using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public interface IDataTransfer
    {
        bool Transfer(string localFilePath);
    }
}
