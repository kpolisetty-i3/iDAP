using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class EmailParser
    {
        public EmailParser() { }

        public void Parse(string zipFilePath, string workingFolder, int partitions, IEmailOutputWriter writer, IDataTransfer dataTransfer)
        {
            EmailFileParser emailFileParser = new EmailFileParser();
            IList<Dictionary<string, string>> records = new List<Dictionary<string, string>>();

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
                    var result = int.TryParse(fileName, out int outValue);
                    if (result)
                    {
                        entry.ExtractToFile(Path.Combine(workingFolder, fileName), true);
                        var record = emailFileParser.ProcessFile(Path.Combine(workingFolder, fileName), entry.FullName);
                        records.Add(record);
                        File.Delete(Path.Combine(workingFolder, fileName));
                        counter++;
                    }

                    if (counter > partitionRecordCount)
                    {
                        writer.Write(Path.Combine(workingFolder, $"Partition{partition}.parquet"), records);
                        dataTransfer.Transfer(Path.Combine(workingFolder, $"Partition{partition}.parquet"));
                        File.Delete(Path.Combine(workingFolder, $"Partition{partition}.parquet"));

                        counter = 0; partition++;
                        records = new List<Dictionary<string, string>>();
                    }

                }
            }

        }
    }
}
