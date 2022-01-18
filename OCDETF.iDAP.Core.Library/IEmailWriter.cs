﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public interface IEmailWriter
    {
        void Write(string filePath, IList<Dictionary<string, string>> records);
    }
}
