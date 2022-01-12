using OCDETF.iDAP.Core.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OCDETF.iDAP.Enron.Library
{
    public class EnronDataCSVService
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

        public EnronDataCSVService() { }

        public void Process(string enronDataFolderPath, IList<string> folders, string outputDirectory)
        {
            List<string> lookupFolders = new List<string>() { "inbox", "sent" };

            List<string> allDirectories = Directory.GetDirectories(enronDataFolderPath).ToList();

            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFolder = string.Empty;
            string previousOutputFolder = string.Empty;

            IList<Dictionary<string, string>> records;
            foreach (string directory in allDirectories)
            {

                records = new List<Dictionary<string, string>>();
                outputFolder = Path.GetFileName(directory).Substring(0, 1);

                if (!File.Exists(Path.Combine(outputDirectory, outputFolder + ".csv")))
                    new CSVService().WriteLine(Path.Combine(outputDirectory, outputFolder + ".csv"), $"{"Folder"}|{MSG_ID}|{DATE_C}|{FROM}|{TO}|{SUBJECT}|{CC}|{MIME}|{CONTENT_TYPE}|{CONTENT_ENCODING}|{BCC}|{X_FROM}|{X_TO}|{X_CC}|{X_BCC}|{X_FOLDER}|{X_ORIGIN}|{X_FILE}|{PHRASES}|{ENTITIES}|{LINKED_ENTITIES}");


                records = ParseFile(directory, outputFolder);
                WriteRecords(records, outputDirectory, outputFolder);


            }
        }


        private IList<Dictionary<string, string>> ParseFile(string directory, string folder)
        {
            IList<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            try
            {

                foreach (string file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    rowValues = new Dictionary<string, string>();
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


                        if (i == 16)
                        {
                            StringBuilder sb = new StringBuilder(streamR.ReadToEnd());

                            rowValues.Add("KeyPhrases", string.Empty);
                            rowValues.Add("KeyEntities", string.Empty);
                            rowValues.Add("LinkedEntities", string.Empty);
                            //rowValues.Add("KeyPhrases", new CognitiveLanguage(cog_endpoint, cog_key).GetKeyPhrases(sb).ToString());
                            //rowValues.Add("KeyEntities", new CognitiveLanguage(cog_endpoint, cog_key).GetKeyEntitites(sb).ToString());
                            //rowValues.Add("LinkedEntities", new CognitiveLanguage(cog_endpoint, cog_key).GetLinkedEntities(sb).ToString());
                        }
                    }
                    //get mail folderpath. eg., allen-p/inbox, allen-p/sent, allen-p/sent-items
                    rowValues.Add("Folder", Path.GetDirectoryName(file.Substring(file.LastIndexOf(Path.GetFileName(directory)))));

                    records.Add(rowValues);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return records;
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

        public void WriteRecords(IList<Dictionary<string, string>> records, string directory, string folder)
        {
            foreach (Dictionary<string, string> aRecord in records)
            {
                new CSVService().WriteLine(Path.Combine(directory, folder + ".csv"), $" { aRecord["Folder"] }|{aRecord["Message-ID:"]}|{aRecord["Date:"]}|{aRecord["From:"]}|{aRecord["To:"]}|{aRecord["Subject:"]}|{aRecord["Cc:"]}|{aRecord["Mime-Version:"]}|{aRecord["Content-Type:"]}|{aRecord["Content-Transfer-Encoding:"]}|{aRecord["Bcc:"]}|{aRecord["X-From:"]}|{aRecord["X-To:"]}|{aRecord["X-cc:"]}|{aRecord["X-bcc:"]}|{aRecord["X-Folder:"]}|{aRecord["X-Origin:"]}|{aRecord["X-FileName:"]}|{aRecord["KeyPhrases"]}|{aRecord["KeyEntities"]}|{aRecord["LinkedEntities"]}");
            }

        }
    }
}
