using OCDETF.iDAP.Azure.Services;
using OCDETF.iDAP.Core.Library;
using OCDETF.iDAP.Core.Library.Office;
using OCDETF.iDAP.Core.Library.PDF;
using OCDETF.iDAP.Enron.Library;
using Parquet;
using Parquet.Data;
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
            //new EmailParser().Parse(@"D:\enron\enron_dataset.zip", @"d:\enron\output", 20, new ParquetFileWriter(),
            //    new DataLakeTransfer("kpidapv2",
            //    "L56P4ZOvy5zvYKCI /gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==",
            //    "https://kpidapv2.blob.core.windows.net/",
            //    "idapv2",
            //    "enron"));

            //var result = new SpacyProcessor("http://localhost:8080/ent").GetEntities("Pastafarians are smarter than people with Coca Cola bottles.");

            //IList<StringBuilder> list = new WordDocument().Read(@"D:\enron\output\test.docx");

            new PDFNLPProcessor("kpidapv2",
                "L56P4ZOvy5zvYKCI /gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==",
                "https://kpidapv2.blob.core.windows.net/", new Core.Library.Models.InputParameters() { appName = "idapv2", category = "public", spacyEndpointURL = "http://idapv2spacy.eastus.azurecontainer.io/ent" }).Process();
        }


    }
}
