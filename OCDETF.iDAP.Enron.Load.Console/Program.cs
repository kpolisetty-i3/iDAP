using OCDETF.iDAP.Azure.Services;
using OCDETF.iDAP.Core.Library;
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
            //var idColumn = new DataColumn(new DataField<int>("id"), new int[] { 1, 2 });

            //var cityColumn = new DataColumn(new DataField<string>("city"), new string[] { "London", "Derby" });


            //// create file schema
            //var schema = new Schema(idColumn.Field, cityColumn.Field);

            //using (Stream fileStream = System.IO.File.OpenWrite(@"D:\enron\test.parquet"))
            //{
            //    using (var parquetWriter = new ParquetWriter(schema, fileStream))
            //    {
            //        // create a new row group in the file
            //        using (ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup())
            //        {
            //            groupWriter.WriteColumn(idColumn);
            //            groupWriter.WriteColumn(cityColumn);
            //        }
            //    }
            //}

            string createFileName = string.Format("FILE-" + DateTime.Now.ToString("yyyyMMdd HHmmss").ToUpper()) + ".download";
            //new DownloadService().Download(data.apiURL, Path.Combine(Path.GetTempPath(), createFileName));

            //new DataLakeUploadService(accountName, accountKey, serviceURI).Upload(data.appName, data.category, Path.Combine(Path.GetTempPath(), createFileName));
            var result = new DownloadService().Download(@"https://tile.loc.gov/storage-services/master/gdc/gdcdatasets/2018487913/2018487913.zip", Path.Combine(Path.GetTempPath(), createFileName));
            if (result)
                new DataLakeUploadService("kpidapv2", "L56P4ZOvy5zvYKCI /gv4iHHNrr3ggiy1EQgop2oijh3T9lU7nHK2MqMBvE9TIH0N2vG8S6mtYkl79EtL2QaiPA==", "https://kpidapv2.blob.core.windows.net/").Upload("idapv2", "enron", Path.Combine(Path.GetTempPath(), createFileName));
            //new EnronDataCSVService().Process(@"D:\Enron\2018487913\maildir", new List<string>() { "inbox", "sent" }, @"D:\Enron\Output");

                //List<string> readFolder = new List<string>() { "inbox", "sent" };
                //List<string> directories = Directory.GetDirectories(@"D:\Enron\maildir").ToList();

                //foreach (string aFolder in readFolder)
                //    new CSVService().WriteHeader($@"D:\Enron\{aFolder}.csv", $"{"Folder"}|{MSG_ID}|{DATE_C}|{FROM}|{TO}|{SUBJECT}|{CC}|{MIME}|{CONTENT_TYPE}|{CONTENT_ENCODING}|{BCC}|{X_FROM}|{X_TO}|{X_CC}|{X_BCC}|{X_FOLDER}|{X_ORIGIN}|{X_FILE}|{PHRASES}|{ENTITIES}|{LINKED_ENTITIES}");

                //foreach (string directory in directories)
                //{
                //    foreach (string aFolder in readFolder)
                //    {
                //        if (Directory.Exists(Path.Combine(directory, aFolder)))
                //        {
                //            ParseDirectory(directory, aFolder);
                //        }
                //    }
                //}
        }


    }
}
