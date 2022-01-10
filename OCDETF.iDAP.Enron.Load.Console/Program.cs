using System;
using System.Collections.Generic;
using System.IO;
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
             List<string> directories = Directory.GetDirectories(@"D:\Enron\maildir").ToList();
            
            using (StreamWriter sw = File.AppendText(@"D:\Enron\inbox.csv"))
            {
                //sw.WriteLine($" ID {aID} \n  Date: {aDate} \n From: {aFrom} \n To: {aTo} \n Subject: {aSubject}\n CC: {aCC} \n MIME: {aMIME}\n Content-Type: {aContentType}\n Content-Encoding: {aContentEncoding}\n BCC:{aBcc} \n X-From: {aXFrom}\n X-To: {aXTo}\n X-cc: {aXCC}\n X-Bcc: {aXBCC}\n X-Folder: {aXFlder}\n X-Origin:{aXOrigin}\n X-File: {aXFile} \n\n\n");
                sw.WriteLine($"{"Folder"}|{MSG_ID}|{DATE_C}|{FROM}|{TO}|{SUBJECT}|{CC}|{MIME}|{CONTENT_TYPE}|{CONTENT_ENCODING}|{BCC}|{X_FROM}|{X_TO}|{X_CC}|{X_BCC}|{X_FOLDER}|{X_ORIGIN}|{X_FILE}");
            }

            foreach (string directory in directories)
            {
                
                if (Directory.Exists(Path.Combine(directory, "inbox")))
                {
                    ParseDirectory(directory);
                }
                break;

            }
        }

        private static Dictionary<string, string> ParseDirectory(string directory)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            foreach (string file in Directory.GetFiles(Path.Combine(directory, "inbox")))
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
                }
                using (StreamWriter sw = File.AppendText(@"D:\Enron\inbox.csv"))
                {
                    //sw.WriteLine($" ID {aID} \n  Date: {aDate} \n From: {aFrom} \n To: {aTo} \n Subject: {aSubject}\n CC: {aCC} \n MIME: {aMIME}\n Content-Type: {aContentType}\n Content-Encoding: {aContentEncoding}\n BCC:{aBcc} \n X-From: {aXFrom}\n X-To: {aXTo}\n X-cc: {aXCC}\n X-Bcc: {aXBCC}\n X-Folder: {aXFlder}\n X-Origin:{aXOrigin}\n X-File: {aXFile} \n\n\n");
                    sw.WriteLine($" {Path.GetFileName(directory)}|{rowValues["Message-ID:"]}|{rowValues["Date:"]}|{rowValues["From:"]}|{rowValues["To:"]}|{rowValues["Subject:"]}|{rowValues["Cc:"]}|{rowValues["Mime-Version:"]}|{rowValues["Content-Type:"]}|{rowValues["Content-Transfer-Encoding:"]}|{rowValues["Bcc:"]}|{rowValues["X-From:"]}|{rowValues["X-To:"]}|{rowValues["X-cc:"]}|{rowValues["X-bcc:"]}|{rowValues["X-Folder:"]}|{rowValues["X-Origin:"]}|{rowValues["X-FileName:"]}");
                }               
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
