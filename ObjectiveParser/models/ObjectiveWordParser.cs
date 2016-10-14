using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;

namespace ObjectiveParser.models
{
    public class ObjectiveWordParser : ObjectiveBaseParser
    {

        Application app = new Application();
        protected Document doc;

        protected List<string> bullets;
        protected Dictionary<string, string> fields;

        public ObjectiveWordParser(string path) : base (path)
        {
            bullets = new List<string>();
            fields = new Dictionary<string, string>();
        }

        public override string Parse()
        {
            return base.Parse();
        }

        protected override string OpenFile()
        {
            doc = app.Documents.Open(this.FileToParsePath);
            return base.OpenFile();
        }

        protected override string ReadFile()
        {
            string output = base.ReadFile(); ;
            string totaltext = "";

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < doc.Paragraphs.Count; i++)
            {
                //totaltext += " \r\n " + doc.Paragraphs[i + 1].Range.Text.ToString();
                sb.Append(" \r\n " + doc.Paragraphs[i + 1].Range.Text.ToString());
            }

            //doc.Save();
            try
            {
                //extract bullets
                foreach (Paragraph para in doc.Paragraphs)
                {
                    string paraNumber = para.Range.ListFormat.ListLevelNumber.ToString();
                    string bulletStr = para.Range.ListFormat.ListString;
                    if (!string.IsNullOrEmpty(bulletStr))
                    {
                        bullets.Add(para.Range.Text);
                    }
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error exreacting bullets.");
                //throw;
            }
           

            //later find fields????
            //foreach (Field wdField in doc.Fields)
            //{
            //    WdFieldType type = wdField.Type;
            //    if (wdField.Type == WdFieldType.wdFieldMergeField)
            //    {
            //        wdField.Select();
            //        string fieldText = wdField.Result.Text;
            //        foreach(FormField ff in wdField.Result.FormFields)
            //        {
            //            string ffName = ff.Name;
            //        }
            //    }
            //}

            //foreach (FormField wdField in doc.FormFields)
            //{
            //    //if (wdField.Type == WdFieldType.wdFieldMergeField)
            //    //{
            //        wdField.Select();
            //        string fieldText = wdField.Range.Text;
            //        fields.Add(wdField.Name, fieldText);
            //    //}
            //}

            this.fullText = totaltext = sb.ToString();

            return totaltext;
        }

        protected override string ParseToObject()
        {
            string bto = base.ParseToObject();

            //check bullets
            foreach (var line in bullets)
            {
                this.Course.AddToObjectiveIfContainsSLOVerb(line);
            }

            return bto;
        }

        protected override string CloseFile()
        {
            doc.Close();
            app.Quit();
            Marshal.ReleaseComObject(doc);
            Marshal.ReleaseComObject(app);
            return base.CloseFile() + "\nQuit Word!" ;
        }

    }
}
