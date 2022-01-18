using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class EmailFileParser
    {
        static string MSG_ID = "Message-ID:";
        static string DATE_C = "Date:";
        static string FROM = "From:";
        static string TO = "To:";
        static string SUBJECT = "Subject:";
        static string CC = "Cc:";
        static string MIME = "Mime-Version:";
        static string CONTENT_TYPE = "Content-Type:";
        static string CONTENT_ENCODING = "Content-Transfer-Encoding:";
        static string BCC = "Bcc:";
        static string X_FROM = "X-From:";
        static string X_TO = "X-To:";
        static string X_CC = "X-cc:";
        static string X_BCC = "X-bcc:";
        static string X_FOLDER = "X-Folder:";
        static string X_ORIGIN = "X-Origin:";
        static string X_FILE = "X-FileName:";
        static string BODY = "X-FileName:";
        static string PHRASES = "KeyPhrases";
        static string ENTITIES = "KeyEntities";
        static string LINKED_ENTITIES = "LinkedEntities";

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
        private string contents { get; set; }
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
        

        public Dictionary<string,string> ProcessFile(string file, string folder)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();

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

                if (i == messageAttributesOrder.Count-1)
                {
                    rowValues.Add("Body", streamR.ReadToEnd());
                    rowValues.Add("KeyPhrases", string.Empty);
                    rowValues.Add("KeyEntities", string.Empty);
                    rowValues.Add("LinkedEntities", string.Empty);
                }
            }
            //get mail folderpath. eg., allen-p/inbox, allen-p/sent, allen-p/sent-items
            rowValues.Add("Folder", folder);
            
            streamR.Close();
            return rowValues;
        }
    }
}
