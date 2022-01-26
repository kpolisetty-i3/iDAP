using OCDETF.iDAP.Azure.Services.ADLS;
using OCDETF.iDAP.Core.Library;
using OCDETF.iDAP.Core.Library.Models;
using OCDETF.iDAP.Core.Library.NLP;
using OCDETF.iDAP.Core.Library.PDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace OCDETF.iDAP.Azure.Services
{
    public class PDFNLPProcessor
    {
        readonly string accountName, accountKey, serviceURI;
        readonly InputParameters data;

        public PDFNLPProcessor(string accountName, string accountKey, string serviceURI, InputParameters data) {
            this.accountKey = accountKey;
            this.accountName = accountName;
            this.serviceURI = serviceURI;
            this.data = data;
        }

        public string Process()
        {
            string result = string.Empty;
            try
            {
                IDataLakeProvider dataLakeProvider = new iDAPDataLakeProvider(accountName, accountKey, serviceURI);
                IList<string> filesToProcess = dataLakeProvider.GetFiles(data.appName, data.category);
                INaturalLangProcessor langProcessor = new SpacyProcessor(data.spacyEndpointURL);

                foreach (string file in filesToProcess)
                {
                    if (Path.GetExtension(file).ToLower() == ".pdf")
                    {
                        BinaryReader reader = dataLakeProvider.Download(data.appName, data.category, Path.GetFileName(file));

                        IPDFProvider pdfProvider = new iTextProvider(reader);
                        int Pages = pdfProvider.GetPageCount();

                        StringBuilder sb_persons = new StringBuilder();
                        StringBuilder sb_orgs = new StringBuilder();
                        HashSet<string> persons = new HashSet<string>();
                        HashSet<string> orgs = new HashSet<string>();
                        for (int i = 1; i <= Pages; i++)
                        {
                            string pageContent = pdfProvider.GetContents(i);
                            HashSet<string> temppeople = langProcessor.GetPersons(pageContent);
                            HashSet<string> temporg = langProcessor.GetOrgs(pageContent);

                            foreach (string person in temppeople)
                                persons.Add(person);

                            foreach (string org in temporg)
                                orgs.Add(org);
                        }
                        var lstPersons = persons.ToList().OrderBy(sel => sel);
                        foreach (string person in lstPersons)
                            sb_persons.AppendLine(person);

                        var lstorgs = orgs.ToList().OrderBy(sel => sel);
                        foreach (string org in lstorgs)
                            sb_orgs.AppendLine(org);

                        dataLakeProvider.Upload(data.appName, data.category, Path.GetFileName(file) + ".persons", sb_persons.ToString());
                        dataLakeProvider.Upload(data.appName, data.category, Path.GetFileName(file) + ".orgs", sb_orgs.ToString());
                    }
                }

            }
            catch (Exception e)
            {

                result = $" {data.appName} {data.category} {data.query} {e.StackTrace} {Path.GetTempPath()} {e.Message}";
            }

            return result;
        }

    }
}
