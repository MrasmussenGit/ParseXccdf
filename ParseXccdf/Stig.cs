using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace ParseXccdf
{
    internal class Stig
    {
        private List<Rule> rules;
        private List<PostProcessedVRule> postProcessRules;
        private string title;
        private string description;
        private string FileNameAndPath;
        private string version;
        private string company;
        private string product;
        private string officeProduct;
        private string purpose;
        private bool isPostProcessed;
        public Stig() { }

        public List<Rule> Rules
        {
            set { rules = value; }
            get { return rules; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string FilePath
        {
            set {  FileNameAndPath = value; }
            get { return FileNameAndPath; }
        }

        public string StigVersion
        {
            set { version = value; }
            get { return version; }
        }

        public string Company
        {
            set { company = value; }
            get { return company; }
        }

        public string Product
        {
            set { product = value; }
            get { return product; }
        }

        public List<PostProcessedVRule> PostProcessRules
        {
            set { postProcessRules = value; }
            get { return postProcessRules; }
        }

        public bool IsPostProcessed
        {
            get { return isPostProcessed; }
            set { isPostProcessed = value; }
        }

        public string Purpose
        {
            set { purpose = value; }
            get { return purpose; }
        }
        public string OfficeProduct
        {
            get { return officeProduct; }
            set { officeProduct = value; }
        }



        public static bool PercentageOfStigRulesMatch(Stig PreStig, Stig PostStig, int Percentage)
        {
            bool match = false;
            string tempId = "";
            string pattern = @"v-\d{3,6}\.\w$";
            float result = 0;
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> matchedList = new List<string>();
            
            // we have to match .a, .b etc
            foreach(Rule preRule in PreStig.Rules)
            {
                foreach(PostProcessedVRule postRule in PostStig.PostProcessRules)
                {
                    Match regMatch = regex.Match(postRule.RuleId);

                    if (regMatch.Success)
                    {
                        postRule.RuleId = regMatch.Value.Split('.')[0];
                    }

                    if(postRule.RuleId == preRule.Rules[0].GroupId)
                    {
                        matchedList.Add(postRule.RuleId);
                        match = true;
                        break;
                    }
                    tempId = preRule.Rules[0].GroupId;
                }

                if(!match)
                {
                    //Console.WriteLine($"{tempId} in {PreStig.FileNameAndPath} Not matched in {PostStig.FileNameAndPath}");
                }
                match = false;
            }

            // check we matched a percentage of rules
            if(null == matchedList || matchedList.Count == 0)
            {
                result = 0;
            }
            else
            {
                result = PreStig.Rules.Count / matchedList.Count;
                result = result * 100;
            }
            if(result >= Percentage)
            {
                match = true;
            }
            else
            {
                match = false;
            }

                return match;
        }

        public static bool CompareStigLists(List<Stig>PreprocessedList,  List<Stig>PostProcessedList)
        {
            bool match = false;
            // determine which is postProcessed
            foreach (Stig preStig in PreprocessedList)
            {
                foreach(Stig postStig in PostProcessedList)
                {
                    // compare Stig properties?
                    // compare rules
                    match = false;
                    if(preStig.product.ToLower()  == postStig.product.ToLower())
                    {
                        if(preStig.version ==  postStig.version)
                        {
                            Console.WriteLine($"{preStig.FilePath} matched with {postStig.FilePath}");
                            match = true;
                            string[] results = PostProcessedVRule.CompareRuleToPostRuleLists(preStig.Rules, postStig.PostProcessRules);
                            bool listsMatched = bool.Parse(results[1]);
                            string messageOutput = results[0];
                            Console.WriteLine(messageOutput);
                        }
                    }

                    if (match) { break; };

                }
                if(!match)
                {
                    Console.WriteLine($"{preStig.Product} version: {preStig.version} not found in Processed data");
                }

            }

            return match;
        }

        public static string[] ParseFileName(string fileName)
        {
            string[] results = fileName.Split('_');


            return results;
        }

        public static string GetMicrosoftOSPurpose(string FileName)
        {
            string[] splits = FileName.Split('_');
            string purpose = "";
            for (int i = 2; i < splits.Length; i++)
            {
                if (splits[i].ToLower() == "ms" || splits[i].ToLower() == "dc")
                {
                    purpose = splits[i].ToUpper();
                }
            }
            return purpose;
        }

        public static string GetOfficeProduct(string FileName)
        {
            string lowerFileName = "";
            // office names to search for
            if (FileName.Contains('\\'))
            {
                string[] splits = FileName.Split('\\');
                lowerFileName = splits[splits.Length - 1].ToLower();
            }
            else
            {
                lowerFileName = FileName.ToLower();
            }
            string productName = "";
            if(lowerFileName.Contains("office") || lowerFileName.Contains("system"))
            {
                productName = "Office System";
            }
            else if(lowerFileName.Contains("access"))
            {
                productName = "Access";
            }
            else if (lowerFileName.Contains("onenote"))
            {
                productName = "OneNote";
            }
            else if (lowerFileName.Contains("powerpoint"))
            {
                productName = "PowerPoint";
            }
            else if (lowerFileName.Contains("skype"))
            {
                productName = "Skype for Business";
            }
            else if (lowerFileName.Contains("word"))
            {
                productName = "Word";
            }
            else if (lowerFileName.Contains("outlook"))
            {
                productName = "Outlook";
            }
            else if (lowerFileName.Contains("system"))
            {
                productName = "Office";
            }
            else if (lowerFileName.Contains("publisher"))
            {
                productName = "Publisher";
            }
            else
            {
                productName = "";
            }

                return productName;
        }

        public static bool CompareStigLists(List<Stig> PreprocessedList, List<Stig> PostProcessedList, bool ShowOnlyErrors)
        {
            bool match = false;

            // determine which is postProcessed
            foreach (Stig preStig in PreprocessedList)
            {
                match = false;
                foreach (Stig postStig in PostProcessedList)
                {
                    if (preStig.version == postStig.version)
                    {
                        // handle office Stigs differently.  They use the same v rule numbers and version
                        // so need to parse the product/filename to pull the exact office product in the Stig
                        // also need to pull MS or DC, not from the prefix, but from the middle of the filename
                        if(preStig.FileNameAndPath.ToLower().Contains("office") || preStig.FileNameAndPath.ToLower().Contains("windows"))
                        {
                            // does it contain an office product name?
                            GetOfficeProduct(preStig.FileNameAndPath);
                            string[] postParseResults = ParseFileName(preStig.FileNameAndPath);
                            string[] preParseResults = ParseFileName(postStig.FileNameAndPath);
                        }





                        if (postStig.FileNameAndPath.Contains("Office-PowerPoint2013-1.6.xml"))
                        {
                            string temp = "";
                        }
                        if (Stig.PercentageOfStigRulesMatch(preStig, postStig, 25))
                        {
                            match = true;
                            if(postStig.FileNameAndPath.Contains("Office-PowerPoint2013-1.6.xml"))
                            {
                                string temp = "";
                            }
                            Console.WriteLine($"{preStig.FilePath} MATCHED {postStig.FilePath}");
                            string[] results = PostProcessedVRule.CompareRuleToPostRuleLists(preStig.Rules, postStig.PostProcessRules, ShowOnlyErrors);
                            bool listsMatched = bool.Parse(results[1]);
                            string messageOutput = results[0];
                            if (messageOutput != "\r\n")
                            {
                                Console.WriteLine(messageOutput.Trim());
                            }
                            break;
                        }
                        else
                        {
                            match = false;
                        }   
                        match = true;
                    }

                }
                if (!match)
                {
                    Console.WriteLine($"{preStig.Product} version: {preStig.version} not found in Processed data");
                }

            }

            return match;
        }

        public override bool Equals(Object Stig)
        {
            bool overallMatch = false;
            bool match = false;
            Stig RealStig = (Stig)Stig;
            if (this.Company.ToLower().Equals(RealStig.Company) && this.Product.ToLower().Equals(RealStig.Product) && this.StigVersion.ToLower().Equals(RealStig.StigVersion)) 
            { 
               // foreach(VRule stigRule in RealStig.V_Rules)
               // {
                   // foreach(string currentStigRule in this.V_Rules)
                   // {
                   //     if(currentStigRule.Equals(stigRule))
                   //     {
                   //         match = true;
                   //         break;
                   //     }
                   // }
              //      if(!match)
              //      {
              //          Console.WriteLine($"Rule: {stigRule} not found in {this.FilePath}");
              //          overallMatch = false;
              //      }
              //  }
                
            }

            return overallMatch;
        }


    }
}
