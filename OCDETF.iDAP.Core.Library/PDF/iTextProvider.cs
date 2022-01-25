using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using System;
using System.Collections.Generic;
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

        public string GetContents(int page)
        {
            if (page <= pdf.GetNumberOfPages())
                return PdfTextExtractor.GetTextFromPage(pdf.GetPage(page));
            else
                return string.Empty;
        }
    }
}
