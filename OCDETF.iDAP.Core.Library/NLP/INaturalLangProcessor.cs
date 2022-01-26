using System;
using System.Collections.Generic;
using System.Text;

namespace OCDETF.iDAP.Core.Library.NLP
{
    public interface INaturalLangProcessor
    {
        string GetPersons(IList<NLPResult> entities);
        string GetOrgs(IList<NLPResult> entities);
        IList<NLPResult> GetAll(string text);

        HashSet<string> GetPersons(string content);

        HashSet<string> GetOrgs(string content);

    }
}
