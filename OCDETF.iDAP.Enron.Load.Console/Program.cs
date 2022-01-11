using OCDETF.iDAP.Azure.Services;
using OCDETF.iDAP.Core.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace OCDETF.iDAP.Enron.Load.Console
{
    class Program
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
        static string ENTITIES = "KeyPhrases";
        static string LINKED_ENTITIES = "KeyPhrases";

        static string cog_endpoint = "https://idapv2-cognitive.cognitiveservices.azure.com/";
        static string cog_key = "7a5f7b23e66748f2a32a1d84348605f8";


        static List<string> messageAttributesOrder = new List<string>()
            {
                "Message-ID:"
                ,"Date:"
                ,"From:"
                ,"To:"
                ,"Subject:"
                ,"Cc:"
                ,"Mime-Version:"
                ,"Content-Type:"
                ,"Content-Transfer-Encoding:"
                ,"Bcc:"
                ,"X-From:"
                ,"X-To:"
                ,"X-cc:"
                ,"X-bcc:"
                ,"X-Folder:"
                ,"X-Origin:"
                ,"X-FileName:"
            };

        static void Main(string[] args)
        {
            List<string> readFolder = new List<string>() { "inbox", "sent" };
            List<string> directories = Directory.GetDirectories(@"D:\Enron\maildir").ToList();

            foreach (string aFolder in readFolder)
                new CSVService().WriteHeader($@"D:\Enron\{aFolder}.csv", $"{"Folder"}|{MSG_ID}|{DATE_C}|{FROM}|{TO}|{SUBJECT}|{CC}|{MIME}|{CONTENT_TYPE}|{CONTENT_ENCODING}|{BCC}|{X_FROM}|{X_TO}|{X_CC}|{X_BCC}|{X_FOLDER}|{X_ORIGIN}|{X_FILE}|{PHRASES}|{ENTITIES}|{LINKED_ENTITIES}");

            foreach (string directory in directories)
            {
                foreach (string aFolder in readFolder)
                {
                    if (Directory.Exists(Path.Combine(directory, aFolder)))
                    {
                        ParseDirectory(directory, aFolder);
                    }
                }
            }
        }

        private static Dictionary<string, string> ParseDirectory(string directory, string folder)
        {



            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            try
            {

                foreach (string file in Directory.GetFiles(Path.Combine(directory, folder)))
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
                    using (StreamWriter sw = File.AppendText($@"D:\Enron\{folder}.csv"))
                    {
                        //sw.WriteLine($" ID {aID} \n  Date: {aDate} \n From: {aFrom} \n To: {aTo} \n Subject: {aSubject}\n CC: {aCC} \n MIME: {aMIME}\n Content-Type: {aContentType}\n Content-Encoding: {aContentEncoding}\n BCC:{aBcc} \n X-From: {aXFrom}\n X-To: {aXTo}\n X-cc: {aXCC}\n X-Bcc: {aXBCC}\n X-Folder: {aXFlder}\n X-Origin:{aXOrigin}\n X-File: {aXFile} \n\n\n");
                        sw.WriteLine($" { string.Concat(Path.GetFileName(directory), "/", folder) }|{rowValues["Message-ID:"]}|{rowValues["Date:"]}|{rowValues["From:"]}|{rowValues["To:"]}|{rowValues["Subject:"]}|{rowValues["Cc:"]}|{rowValues["Mime-Version:"]}|{rowValues["Content-Type:"]}|{rowValues["Content-Transfer-Encoding:"]}|{rowValues["Bcc:"]}|{rowValues["X-From:"]}|{rowValues["X-To:"]}|{rowValues["X-cc:"]}|{rowValues["X-bcc:"]}|{rowValues["X-Folder:"]}|{rowValues["X-Origin:"]}|{rowValues["X-FileName:"]}|{rowValues["KeyPhrases"]}|{rowValues["KeyEntities"]}|{rowValues["LinkedEntities"]}");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }


            return rowValues;
        }

        static string ContainsAttribute(string currentLine, string attr)
        {
            string returnValue = string.Empty;
            for (int i = 0; i < messageAttributesOrder.Count; i++)
            {
                if (currentLine.Contains(messageAttributesOrder[i]))
                    returnValue = messageAttributesOrder[i];
            }

            return returnValue;
        }


    }
}
