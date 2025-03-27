using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

                    break;

                }
                if(!match)
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
