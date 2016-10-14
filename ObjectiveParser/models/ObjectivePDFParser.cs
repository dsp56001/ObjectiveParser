using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfToText;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveParser.models
{
    class ObjectivePDFParser : ObjectiveBaseParser
    {
        public ObjectivePDFParser(string path) : base (path)
        {

        }

        public override string Parse()
        {
            string output = "";
            
            output += base.Parse();
            return output;
        }

        protected override string ReadFile()
        {
            return base.ReadFile() + "\n" + ParsePDF();
        }

        public string ParsePDF()
        {

            string rangeText = ReadPdfFile(this.FileToParsePath);
            this.fullText = rangeText;
            return rangeText;
        }

        protected string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }
    }
}
