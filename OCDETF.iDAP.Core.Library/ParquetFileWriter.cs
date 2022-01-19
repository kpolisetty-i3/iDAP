using Parquet;
using Parquet.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class ParquetFileWriter : IEmailOutputWriter
    {
        public ParquetFileWriter() { }


        public void Write(string filePath, IList<Dictionary<string, string>> records)
        {
            IList<DataColumn> columns = new List<DataColumn>();

            Dictionary<string, string> row = records.FirstOrDefault();

            foreach (string key in row.Keys)
            {
                columns.Add(new DataColumn(new DataField<string>(key), records.Select(sel => sel.GetValueOrDefault(key)).ToArray()));
            }

            var schema = new Schema(columns.Select(sel => sel.Field).ToArray());
            using (Stream fileStream = System.IO.File.OpenWrite(filePath))
            {
                using (var parquetWriter = new ParquetWriter(schema, fileStream))
                {
                    // create a new row group in the file
                    using (ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup())
                    {
                        foreach (DataColumn dataColumn in columns)
                        {
                            groupWriter.WriteColumn(dataColumn);
                        }
                        
                    }
                }
                fileStream.Close();
            }
        }

        
    }
}
