using ObjectiveParser.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ObjectiveParser
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter a file to parse as an argument.");
                System.Console.WriteLine("Usage: filename.[pfd,doc,docx]");
                System.Console.WriteLine("Usage: directory");
                Console.ReadKey();
                return 1;
            }

           

            if (args.Length == 1)
            {
                //file
                string path = args[0];
                if (System.IO.File.Exists(path))
                {
                    TryParse(path);
                }
                //folder
                if (System.IO.Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach( var fi in di.EnumerateFiles())
                    {
                        TryParse(fi.FullName);
                        
                    }   
                }
            }

            if (args.Length >1)
            {
                //multiple files
                for(int i = 0; i < args.Length; i++)
                {
                    TryParse(args[i]);
                    //var parser = ObjectiveParserFactory.GetObjectiveParser();
                    //Console.WriteLine(parser.Parse());
                }
            }
            Console.WriteLine("Done...");
            //Console.ReadKey();

            return 0;
        }

        private static void TryParse(string path)
        {
            try
            { 
                var parser = ObjectiveParserFactory.GetObjectiveParser(path);
                Console.WriteLine(parser.Parse());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Path: " + path);
            }

        }
    }
}
