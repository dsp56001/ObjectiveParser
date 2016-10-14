using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ObjectiveParser.models
{

    public enum OutputFormat { text, tab }

    public class ParseCourse
    {
        public string Name { get; set; }
        public string Number { get; set; }

        public string Description { get; set; }

        public string ObjectiveText { get; set; }

        public List<string> Objectives;

        public string Department { get; set; }
        public string Program { get; set; }

        public string Degree { get; set; }

        public string FileName { get; set; }

        public OutputFormat OutputFormat;

        public ParseCourse(string TextToParse)
        {
            this.OutputFormat = OutputFormat.text;
            this.Objectives = new List<string>();
            this.Description = ParseDescription(TextToParse);
            this.Name = ParseName(TextToParse);
            this.Number = ParseNumber(TextToParse);
            this.ObjectiveText = ParseObjectives(TextToParse);

            //Add objectives to Objective List
            using (StringReader reader = new StringReader(this.ObjectiveText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    AddToObjectiveIfContainsSLOVerb(line);
                }
            }
        }

        private string ParseNumber(string textToParse)
        {
            var regex = new Regex(@"\b\d{2}-\d{4}");
            var matches = regex.Matches(textToParse);
            string firstMatch = matches[0].ToString();
            if (string.IsNullOrEmpty(firstMatch)) firstMatch = "00-0000";
            return firstMatch;
        }

        public void AddToObjectiveIfContainsSLOVerb(string line)
        {
            if(this.StringContainsSLOVerb(line))
            {
                this.Objectives.Add(line.Trim());
            }
        }

        public bool StringContainsSLOVerb(string line)
        {
            //Find SLOVerbs
            foreach (string s in SLOVerbs)
            {
                if (line.ToLower().Contains(s.ToLower() + " "))
                {
                    if (!this.Objectives.Contains(line.Trim()))
                    {
                        return true;
                    }
                    
                }
                
            }
            return false;
        }

        private string ParseObjectives(string TextToParse)
        {
            //string objectivePara = this.MatchTillNextLine("At the conclusion of this course students will be able to", TextToParse);
            string objectivePara = this.MatchTillNextLine("students will be able to", TextToParse);
            if(string.IsNullOrEmpty(objectivePara))
            {
                objectivePara = this.MatchTillNextLine("students should be able to", TextToParse);
            }
            if(string.IsNullOrEmpty(objectivePara))
            {
                objectivePara = this.MatchTillNextLine("students will be expected to", TextToParse);
            }
            if (string.IsNullOrEmpty(objectivePara))
            {
                objectivePara = this.MatchTillNextLine("learning outcomes", TextToParse);
            }
            if (string.IsNullOrEmpty(objectivePara))
            {
                objectivePara = this.MatchTillNextLine("outcomes", TextToParse);
            }
            try
            {
                objectivePara = objectivePara.Replace("(the official course learning objectives from Coordinator Only)", "");
                if (String.IsNullOrEmpty(objectivePara))
                {
                    objectivePara = this.MatchTillNextLine("course objectives", TextToParse);
                }
                if (String.IsNullOrEmpty(objectivePara))
                {
                    objectivePara = this.MatchTillNextLine("Course Objectives", TextToParse);
                }
                if (String.IsNullOrEmpty(objectivePara))
                {
                    objectivePara = this.MatchTillNextLine("objectives", TextToParse);
                }
                if (String.IsNullOrEmpty(objectivePara))
                {
                    objectivePara = this.MatchTillNextLine("Objectives", TextToParse);
                }

                if (String.IsNullOrEmpty(objectivePara))
                {
                    objectivePara = this.MatchTillNextLine("students will be", TextToParse);
                }
                if (String.IsNullOrEmpty(objectivePara))
                {
                    objectivePara = this.MatchTillNextLine("students should be", TextToParse);
                }
            }
            catch (Exception exPo)
            {
                Console.WriteLine("ParseObjectives Exception:{0}", exPo);
                //throw;
            }
            
            
            return objectivePara;
        }

        private string ParseName(string TextToParse)
        {
            return this.MatchTillNextLine("Course Name", TextToParse);
        }

        private string ParseDescription(string TextToParse)
        {
            return this.MatchTillNextLine("Course Description:", TextToParse);
        }

        private string MatchTillNextLine(string match, string TextToParse)
        {
            int firstLoc = TextToParse.IndexOf(match);
            int lastLoc = TextToParse.LastIndexOf(match);

            string paragraph = "";

            //reurn if nothing is found
            if (firstLoc <= 0)
                return string.Empty;

            //only one found go with it
            if (firstLoc == lastLoc)
            {
                int nextLineBreak = TextToParse.IndexOf("\n", firstLoc);
                paragraph = TextToParse.Substring(firstLoc, nextLineBreak);
            }
            else //more than 1 
            {
                //take first
                int nextLineBreak = TextToParse.IndexOf("\n", firstLoc);
                paragraph = TextToParse.Substring(firstLoc, nextLineBreak);
            }


            return paragraph.Replace(match, "");
        }

        internal string About()
        {
            string about = "";
            switch (this.OutputFormat)
            {
                case OutputFormat.tab:
                    about = "\"" + this.Name.Trim() + "\"\tFile\t\"" + this.FileName.Trim() + "\"";
                    about += "\t\"" + this.Number.Trim() + "\"";
                    about += "\t\"" + this.Program + "\""; //May be null causes null exception they all may do this if trim is called
                    about += "\t\"" + this.Description.Trim() + "\"";
                    about += "\t\"" + this.Department + "\"";
                    about += "\t\"" + this.Degree + "\"";
                    //about += "\nDecription:" + this.Description;
                    about += "\t\"";
                    this.Objectives.ForEach(o => about += ", " + o.ToString());
                    about += "\"";
                    break;
                default:
                    about = "\nName:" + this.Name + "\tFile:" + this.FileName;
                    about += "\nNumber:" + this.Number;
                    about += "\nProgram:" + this.Program;
                    about += "\nDecription:" + this.Description;
                    about += "\nDepartment:" + this.Department;
                    about += "\nDegree:" + this.Degree;
                    //about += "\nDecription:" + this.Description;
                    //about += "\nObjective Text:" + this.ObjectiveText;
                    this.Objectives.ForEach(o => about += "\nObjective:" + o.ToString());
                break;
            }
            return about;
        }

        

        public string[] SLOVerbs = new string[]
        {
            "Count",
            "Define",
            "Describe",
            "Draw",
            "Identify",
            "Labels",
            "List",
            "Match",
            "Name",
            "Outlines",
            "Point",
            "Quote",
            "Read'",
            "Recall",
            "Recite",
            "Recogniz",
            "Record",
            "Repeat",
            "Reproduces",
            "Selects",
            "State",
            "Write",
            "Associate",
            "Compute",
            "Convert",
            "Defend",
            "Discuss",
            "Distinguish",
            "Estimate",
            "Explain",
            "Extend",
            "Extrapolate",
            "Generalize",
            "Give examples",
            "Infer",
            "Paraphrase",
            "Predict",
            "Rewrite",
            "Summarize",
            "Add",
            "Apply",
            "Calculate",
            "Change",
            "Classify",
            "Complete",
            "Compute",
            "Demonstrate",
            "Discover",
            "Divide",
            "Examine",
            "Graph",
            "Interpolate",
            "Manipulate",
            "Modify",
            "Operate",
            "Prepare",
            "Produce",
            "Show",
            "Solve",
            "Subtract",
            "Translate",
            "Use",
            "Analyze",
            "Arrange",
            "Breakdown",
            "Combine",
            "Design",
            "Detect",
            "Develop",
            "Diagram",
            "Differentiate",
            "Discriminate",
            "Illustrate",
            "Infer",
            "Outline",
            "Point out",
            "Relate",
            "Select",
            "Separate",
            "Subdivide",
            "Utilize",
            "Categorize",
            "Combine",
            "Compile",
            "Compose",
            "Create",
            "Drive",
            "Design",
            "Devise",
            "Explain",
            "Generate",
            "Group",
            "Integrate",
            "Modify",
            "Order",
            "Organize",
            "Plan",
            "Prescribe",
            "Propose",
            "Rearrange",
            "Reconstruct",
            "Related",
            "Reorganize",
            "Revise",
            "Rewrite",
            "Summarize",
            "Transform",
            "Specify",
            "Appraise",
            "Assess",
            "Compare",
            "Conclude",
            "Contrast",
            "Criticize",
            "Critique",
            "Determine",
            "Grade",
            "Interpret",
            "Judge",
            "Justify",
            "Measure",
            "Rank",
            "Rate",
            "Support",
            "Test"

        };
    }
}
