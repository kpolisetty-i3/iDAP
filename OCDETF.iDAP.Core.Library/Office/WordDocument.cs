using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace OCDETF.iDAP.Core.Library.Office
{
    public class WordDocument
    {
        public WordDocument() { }

        public IList<StringBuilder> Read(string path)
        {
            IList<StringBuilder> pages = new List<StringBuilder>();

            using (WordprocessingDocument wordDocument =  WordprocessingDocument.Open(path, false))
            {
                
                foreach(var aElement in wordDocument.MainDocumentPart.Document.Body)
                {
                    var test = aElement.InnerText;
                    StringBuilder result = new StringBuilder(test);
                    MatchCollection mc = Regex.Matches(test, "[0-9]{12,14}");
                    foreach (Match aMatch in mc)
                        result.Replace(aMatch.Value, " ");

                    mc = Regex.Matches(test, "[A-Z]00[A-Z]");
                    foreach (Match aMatch in mc)
                        result.Replace(aMatch.Value, aMatch.Value.Replace("00"," "));

                    pages.Add(result);
                }
            }
            return pages;
        }

        public IList<StringBuilder> Read(string path, int pageNumber)
        {
            return null;
        }
    }
}
