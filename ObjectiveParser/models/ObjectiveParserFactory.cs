using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveParser.models
{
    public static class ObjectiveParserFactory
    {

        static IObjectiveBaseParser parser;

        public static IObjectiveBaseParser GetObjectiveParser(string path)
        {
            string filePath = path;
            string[] filePathAry = filePath.Split('.');
            string extension = filePathAry[filePathAry.Length - 1];
            switch (extension)
            {
                case "doc":
                case "docx":
                    if(filePath.Contains("Strategic Plan"))
                    {
                        parser = new SPProgramWordParser(filePath);
                    }
                    else
                    {
                        parser = new ObjectiveWordParser(filePath);
                    }
                    
                    break;
                case "pdf":
                    parser = new ObjectivePDFParser(filePath);
                    break;
                default:
                    //nothing
                    //will throw exception later since parser is null
                    break;
            }
            if(parser == null) throw new Exception("No parser for " + extension);
            return parser;
        }
    }
}
