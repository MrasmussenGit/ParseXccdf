using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
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
        private string originalFile;
        private string version;
        private string company;
        private string product;
        private string officeYear;
        private string officeProduct;
        private string purpose;
        private bool isPostProcessed;
        private bool isOfficeProduct;
        private string classification;
        private string releaseInfo;
        private string notice;
        private string source;
        private string fullVersion;


        public Stig() { }

        public string Classification
        {
            get { return classification; }
            set { classification = value; }
        }
        public string ReleaseInfo
        {
            get { return releaseInfo; }
            set { releaseInfo = value; }
        }
        public string Notice
        {
            get { return notice; }
            set { notice = value; }
        }
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        public string FullVersion
        {
            get { return fullVersion; }
            set { fullVersion = value; }
        }
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
        public string OfficeYear
        {
            set { officeYear = value; }
            get { return officeYear; }
        }
        public bool IsOfficeProduct
        {
            get { return isOfficeProduct; }
            set { isOfficeProduct = value; }
        }

        public string OriginalFile
        {
            set { originalFile = value; }
            get { return originalFile; }
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

        public static string GetOfficeYear(string FileName)
        {
            string pattern = @"\d{4}";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(FileName);

     
            return match.Value;
        }

        public static string GetPreOfficeProduct(string FileName)
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
                        if(preStig.OriginalFile.Contains('\\'))
                        {
                            string[] splits = preStig.OriginalFile.Split('\\');
                            preStig.OriginalFile = splits[splits.Length - 1];
                        }
                        if(postStig.OriginalFile.Contains('\\'))
                        {
                            string[] splits = postStig.OriginalFile.Split('\\');
                            postStig.OriginalFile = splits[splits.Length - 1];
                        }
                        if(postStig.OriginalFile == preStig.originalFile)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{preStig.OriginalFile} MATCHED {postStig.FileNameAndPath}");
                            Console.ForegroundColor = ConsoleColor.White;
                            match = true;
                            string[] results = PostProcessedVRule.CompareRuleToPostRuleLists(preStig.Rules, postStig.PostProcessRules, ShowOnlyErrors);
                            bool listsMatched = bool.Parse(results[1]);
                            string messageOutput = results[0];
                            if (messageOutput != "\r\n")
                            {
                                Console.WriteLine(messageOutput.Trim());
                            }
                            break;
                        }

                    }
                }
                if (!match)
                {
                    Console.WriteLine($"{preStig.originalFile} not found converted in Processed data");
                }

            }

            return match;
        }
        public static void OutputStigToDisk(string Path, Stig OutputStig)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlElement root = doc.CreateElement("DISA");
            root.SetAttribute("version", OutputStig.StigVersion);
            root.SetAttribute("classification", OutputStig.Classification);
            root.SetAttribute("customname", "");
            root.SetAttribute("stigid", "");
            root.SetAttribute("description", OutputStig.Description);
            root.SetAttribute("filename", OutputStig.FileNameAndPath);
            root.SetAttribute("releaseinfo", OutputStig.ReleaseInfo);
            root.SetAttribute("title", OutputStig.Title);
            root.SetAttribute("notice", OutputStig.Notice);
            root.SetAttribute("source", OutputStig.Source);
            root.SetAttribute("fullversion", OutputStig.FullVersion);
            root.SetAttribute("created", DateTime.Now.ToShortDateString());
            doc.AppendChild(root);

            // add node here regRule, manualRule then append the below

            foreach(Rule rule in OutputStig.Rules)
            {
                if (rule.Rules[0].DscResource == "None")
                {
                    XmlElement manRuleElement = doc.CreateElement("ManualRule");
                    manRuleElement.SetAttribute("dscresourcemodule", "None");
                    root.AppendChild(manRuleElement);
                }
                else if (rule.Rules[0].DscResource == "RegistryPolicyFile")
                {
                    XmlElement regRuleElement = doc.CreateElement("RegistryRule");
                    regRuleElement.SetAttribute("dscresourcemodule", "PSDscResources");
                    root.AppendChild(regRuleElement);

                    XmlElement ruleElement = doc.CreateElement("Rule");
                    ruleElement.SetAttribute("id", rule.Rules[0].GroupId);
                    ruleElement.SetAttribute("severity", rule.Rules[0].Severity);
                    ruleElement.SetAttribute("conversionstatus", "");
                    ruleElement.SetAttribute("title", rule.Rules[0].RuleTitle);
                    ruleElement.SetAttribute("dscresource", "RegistryPolicyFile");
                    regRuleElement.AppendChild(ruleElement);

                    XmlElement ruleChild = doc.CreateElement("Description");
                    ruleChild.InnerText = rule.Description;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("DuplicateOf");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Ensure");
                    ruleChild.InnerText = "Present";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("IsNullOrEmpty");
                    ruleChild.InnerText = "False";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Key");
                    RegistryVRule registryVRule = RegistryVRule.Clone(rule.Rules[0]);
                    ruleChild.InnerText = registryVRule.Key;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("LegacyId");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueRequired");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueTestString");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("RawString");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueData");
                    RegistryVRule registryVRule1 = RegistryVRule.Clone(rule.Rules[0]);
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueName");
                    RegistryVRule registryVRule2 = RegistryVRule.Clone(rule.Rules[0]);
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueType");
                    RegistryVRule registryVRule3 = RegistryVRule.Clone(rule.Rules[0]);
                    ruleElement.AppendChild(ruleChild);

                }

            }

            doc.Save(Path);
            // check for manualRules
            // check for registryRules
            // set attribute for reg rules -> dscresourcemodule="PSDscResources"



        }
    }
}
