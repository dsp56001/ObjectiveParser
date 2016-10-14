using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveParser.models
{
    /// <summary>
    /// Strategic Plan document specific parser
    /// </summary>
    class SPProgramWordParser : ObjectiveWordParser
    {
        List<ContentControl> contentControls;

        public SPProgramWordParser(string path) : base (path)
        {
            bullets = new List<string>();
            fields = new Dictionary<string, string>();
            contentControls = new List<ContentControl>();
        }

        protected override string ReadFile()
        {
            contentControls = GetAllContentControls(this.doc);
            return base.ReadFile();
        }

        protected override string ParseToObject()
        {
            string bto = base.ParseToObject();
            this.Course.Objectives.Clear();
            bool Q2Found = false;
            //check for controls if name matches set property
            foreach (var control in contentControls)
            {
                switch(control.Title)
                {
                    case "Department":
                        try
                        {
                            this.Course.Department = control.Range.Text;
                        }
                        catch (Exception)
                        {

                            //throw;
                        }
                        
                        break;
                    case "Program":
                        try
                        {
                            this.Course.Program = control.Range.Text;
                        }
                        catch (Exception)
                        {

                            //throw;
                        }
                       
                        break;
                    case "Degree":
                        try
                        {
                            this.Course.Degree = control.Range.Text;
                        }
                        catch (Exception)
                        {

                            //throw;
                        }
                        
                        break;
                    case "Q1":
                        break;
                    case "Q2":
                        Q2Found = true;
                        string bulletStr = control.Range.ListFormat.ListString;
                        string text = control.Range.Text;
                        string[] lines = text.Split(Environment.NewLine.ToCharArray());
                        foreach (var line in lines)
                        {
                            this.Course.Objectives.Add(line);
                        }
                        break;
                    case "Q3":
                        break;
                    case "Q4":
                        break;

                }
                //this.Course.AddToObjectiveIfContainsSLOVerb(line);
            }

            if((Q2Found==false) ) //Always try harder? what is this q2 buisness?
            {
                //try harder look for students should be able to
                int start = this.fullText.IndexOf("students should be able to");
                int stop = this.fullText.IndexOf("List all required and elective courses for the degree including");
                string section = this.fullText.Substring(start, stop-start);
                string[] lines = section.Split(Environment.NewLine.ToCharArray());
                foreach (var line in lines)
                {
                    if(this.Course.StringContainsSLOVerb(line))
                    { 
                        this.Course.Objectives.Add(line);
                    }
                    else
                    {
                        if(line.Trim() != string.Empty 
                            && line != " "
                            && line != "\a"
                            && (!(line.Contains("should be able to")))
                            )
                        {
                            this.Course.Objectives.Add(line);
                        }
                    }
                }
            }

            return bto;
        }

        /// <summary>
        /// Get all content controls contained in the document.
        /// </summary>
        /// <param name="wordDocument"></param>
        /// <returns></returns>
        public static List<ContentControl> GetAllContentControls(Document wordDocument)
        {
            if (null == wordDocument)
                throw new ArgumentNullException("wordDocument");

            List<ContentControl> ccList = new List<ContentControl>();

            // The code below search content controls in all
            // word document stories see http://word.mvps.org/faqs/customization/ReplaceAnywhere.htm
            Range rangeStory;
            foreach (Range range in wordDocument.StoryRanges)
            {
                rangeStory = range;
                do
                {
                    try
                    {
                        foreach (ContentControl cc in range.ContentControls)
                        {
                            ccList.Add(cc);
                        }

                        // Get the content controls in the shapes ranges
                        foreach (Shape shape in range.ShapeRange)
                        {
                            foreach (ContentControl cc in shape.TextFrame.TextRange.ContentControls)
                            {
                                ccList.Add(cc);
                            }

                        }
                    }
                    catch (COMException) { }
                    rangeStory = rangeStory.NextStoryRange;

                }
                while (rangeStory != null);
            }
            return ccList;
        }
    }

    
}
