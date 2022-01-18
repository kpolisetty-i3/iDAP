using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class EmailParser
    {
        public EmailParser() { }

        public void Parse(string zipFilePath, string workingFolder, int partitions, IEmailOutputWriter writer)
        {
            EmailFileParser emailFileParser = new EmailFileParser();
            IList<Dictionary<string,string>> records = new List<Dictionary<string, string>>();

            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                int totalCount = archive.Entries.Count;
                int counter = 0;
                int partition = 1;

                double partitionRecordCount = totalCount / partitions;
                partitionRecordCount = Math.Ceiling(partitionRecordCount);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {

                    var fileName = Path.GetFileName(entry.FullName);
                    int outValue = 0;
                    var result = int.TryParse(fileName, out outValue);
                    if (result)
                    {
                        entry.ExtractToFile(Path.Combine(workingFolder, fileName),true);
                        var record = emailFileParser.ProcessFile(Path.Combine(workingFolder, fileName), entry.FullName);                        
                        records.Add(record);
                        File.Delete(Path.Combine(workingFolder, fileName));
                        counter++;
                    }
                    
                    if (counter > partitionRecordCount)
                    {
                        writer.Write(Path.Combine(workingFolder, $"Partition{partition}.parquet"), records);
                        counter = 0;partition++;
                        records = new List<Dictionary<string, string>>();
                    }

                }
            }

        }

        private IList<ZipArchiveEntry> ProcessFiles(string zipFilePath, string workingFolder)
        {
            IList<ZipArchiveEntry> processFiles = new List<ZipArchiveEntry>();


            EmailFileParser emailFileParser = new EmailFileParser();
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var fileName = Path.GetFileName(entry.FullName);
                    int outValue = 0;
                    var result = int.TryParse(fileName, out outValue);
                    if (result)
                    {
                        processFiles.Add(entry);

                        entry.ExtractToFile(Path.Combine(workingFolder, fileName));
                        var result2 = emailFileParser.ProcessFile(Path.Combine(workingFolder, fileName), entry.FullName);
                        File.Delete(Path.Combine(workingFolder, fileName));
                    }

                }
            }
            return processFiles;
        }
    }
}
