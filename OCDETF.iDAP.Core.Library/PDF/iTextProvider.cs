using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCDETF.iDAP.Core.Library.PDF
{
    public class iTextProvider : IPDFProvider
    {
        readonly PdfDocument pdf;
        public iTextProvider(string strPath)
        {
            pdf = new PdfDocument(new PdfReader(strPath)); 
        }

        public iTextProvider(BinaryReader reader)
        {         
            pdf = new PdfDocument(new PdfReader(reader.BaseStream));
        }

        public string GetContents(int page)
        {
            if (page <= pdf.GetNumberOfPages())
            {
                string pdfText = PdfTextExtractor.GetTextFromPage(pdf.GetPage(page), new SimpleTextExtractionStrategy());
                pdfText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(pdfText)));
                return pdfText;
            }                
            else
                return string.Empty;
        }

        public int GetPageCount()
        {
            return pdf.GetNumberOfPages();
        }
    }
}
