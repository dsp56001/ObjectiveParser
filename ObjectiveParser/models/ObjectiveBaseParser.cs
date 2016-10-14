using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveParser.models
{
    public class ObjectiveBaseParser : IObjectiveBaseParser
    {

        private bool debug = false;

        public List<string> Objectives
        {
            get; set;
        }
        
        public string FileToParsePath { get; protected set; }
    
        protected string fullText;

        public ParseCourse Course { get; set; }

        public ObjectiveBaseParser(string path)
        {
            this.FileToParsePath = path;
        }

        public virtual string Parse()
        {
            try
            {
                if (debug) Console.WriteLine(this.OpenFile());
                else this.OpenFile();
                //Read file to local string
                if (this.fullText == null)
                {
                    if (debug) Console.WriteLine(this.ReadFile());
                    else this.ReadFile();
                }
                //Parse local string
                if (this.fullText != null)
                {
                    if (debug) Console.WriteLine(this.ParseToObject());
                    else this.ParseToObject();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (debug) Console.WriteLine(this.CloseFile());
                else this.CloseFile();
            }

            string objs = "";
            Console.WriteLine(Course.About());

            return objs;
        }

        protected virtual string CloseFile()
        {
            return string.Format("Closing File: {0}", this.FileToParsePath);
        }

        protected virtual string ParseToObject()
        {
            Course = new ParseCourse(this.fullText);
            Course.FileName = FileToParsePath;
            if(Course != null)
            {
                return "\nCourse Parsed.\n" + Course.About() ;
            }
            else
            {
                return "\nCourse failed Parsed.";
            }
        }

        protected virtual string OpenFile()
        {
            return string.Format("Opening File: {0}", this.FileToParsePath);
        }

        protected virtual string ReadFile()
        {
            //base class does't change fullText
            return string.Format("Read File: {0}", this.FileToParsePath);

        }
    }

    public interface IObjectiveBaseParser
    {
        List<string> Objectives { get; set; }

        string FileToParsePath { get; }
        string Parse();
    }


}
