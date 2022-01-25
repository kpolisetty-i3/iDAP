using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using OCDETF.iDAP.Core.Library.NLP;

namespace OCDETF.iDAP.Core.Library
{
    public class EmailFileParser
    {
        static readonly string MSG_ID = "Message-ID:";
        static readonly string DATE_C = "Date:";
        static readonly string FROM = "From:";
        static readonly string TO = "To:";
        static readonly string SUBJECT = "Subject:";
        static readonly string CC = "Cc:";
        static readonly string MIME = "Mime-Version:";
        static readonly string CONTENT_TYPE = "Content-Type:";
        static readonly string CONTENT_ENCODING = "Content-Transfer-Encoding:";
        static readonly string BCC = "Bcc:";
        static readonly string X_FROM = "X-From:";
        static readonly string X_TO = "X-To:";
        static readonly string X_CC = "X-cc:";
        static readonly string X_BCC = "X-bcc:";
        static readonly string X_FOLDER = "X-Folder:";
        static readonly string X_ORIGIN = "X-Origin:";
        static readonly string X_FILE = "X-FileName:";


        static List<string> messageAttributesOrder = new List<string>()
            {
                MSG_ID
                ,DATE_C
                ,FROM
                ,TO
                ,SUBJECT
                ,CC
                ,MIME
                ,CONTENT_TYPE
                ,CONTENT_ENCODING
                ,BCC
                ,X_FROM
                ,X_TO
                ,X_CC
                ,X_BCC
                ,X_FOLDER
                ,X_ORIGIN
                ,X_FILE
            };

        public EmailFileParser()
        {
        }

        public string ContainsAttribute(string currentLine, string attr)
        {
            string returnValue = string.Empty;
            for (int i = 0; i < messageAttributesOrder.Count; i++)
            {
                if (currentLine.Contains(messageAttributesOrder[i]))
                    returnValue = messageAttributesOrder[i];
            }

            return returnValue;
        }


        public Dictionary<string, string> ProcessFile(string file, string folder)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            SpacyProcessor spacyService = new SpacyProcessor("http://localhost:8080/ent");
            StreamReader streamR = new StreamReader(file);
            string currentLine = streamR.ReadLine();
            for (int i = 0; i < messageAttributesOrder.Count; i++)
            {
                string currentValue = string.Empty;
                if (currentLine.Contains(messageAttributesOrder[i]))
                {
                    currentValue = currentLine.Substring(messageAttributesOrder[i].Length);
                }

                if (!string.IsNullOrEmpty(currentValue) && i < 16)
                {
                    currentLine = streamR.ReadLine();
                    while (string.IsNullOrEmpty(ContainsAttribute(currentLine, messageAttributesOrder[i])))
                    {
                        currentValue = currentValue + currentLine;
                        currentLine = streamR.ReadLine();
                    }
                }
                rowValues.Add(messageAttributesOrder[i], currentValue);

                if (i == messageAttributesOrder.Count - 1)
                {

                    rowValues.Add("Body", streamR.ReadToEnd());
                    var entities = spacyService.GetEntities(rowValues["Body"]);
                    rowValues.Add("KeyPersons", GetPersons(entities));
                    rowValues.Add("KeyOrgs", GetOrgs(entities));
                    rowValues.Add("LinkedEntities", string.Empty);
                }
            }
            //get mail folderpath. eg., allen-p/inbox, allen-p/sent, allen-p/sent-items
            rowValues.Add("Folder", folder);

            streamR.Close();
            return rowValues;
        }

        private string GetOrgs(IList<NLPResult> list)
        {
            StringBuilder result = new StringBuilder();
            if (list.Count > 0)
            {
                var filter = list.Where(sel => sel.Type == "ORG" ).ToList();
                foreach (NLPResult avalue in filter)
                {
                    result.Append($"{avalue.Text}, ");
                }
                return result.ToString().Trim();
            }
            else
                return string.Empty;

        }

        private string GetPersons(IList<NLPResult> list)
        {
            StringBuilder result = new StringBuilder();
            if (list.Count > 0)
            {
                var filter = list.Where(sel => sel.Type == "PERSON" || sel.Type == "NORP").ToList();
                foreach (NLPResult avalue in filter)
                {
                    result.Append($"{avalue.Text} - ");
                }
                return result.ToString().Trim();
            }
            else
                return string.Empty;



        }
    }
}
