using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace ParseXccdf
{
    internal class Stig
    {
        private List<Rule> rules;
        private List<PostProcessedVRule> postProcessRules;
        private List<string> changeLog;
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


        public Stig() 
        {
            this.rules = new List<Rule>();
            this.postProcessRules = new List<PostProcessedVRule>();
            this.changeLog = new List<string>();
        }

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
        public List<string> ChangeLog
        {
            set { changeLog = value; }
            get { return changeLog; }
        }

        public string OriginalFile
        {
            set { originalFile = value; }
            get { return originalFile; }
        }

        public static List<String> GetChangeLog(string FilePath)
        {
            
            // filepath should end in .log
            if(Regex.IsMatch(FilePath, @".*xml"))
            {
                FilePath = FilePath.Replace(".xml", ".log");
            }

            List<string> changeLog = new List<string>(File.ReadAllLines(FilePath));

            return changeLog;
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
        public void ProcessChangeLog()
        {
            
            List<string> ruleChanges = GetChangeLog(this.FileNameAndPath);
            foreach(string change in ruleChanges)
            {
                // V-213126::*::HardCodedRule(RegistryRule)@{DscResource = 'RegistryPolicyFile'; Ensure = 'Present';
                // Key = 'HKEY_CURRENT_USER\Software\Adobe\Adobe Acrobat\DC\Security\cDigSig\cEUTLDownload'; ValueData = '0'; ValueName = 'bLoadSettingsFromURL'; ValueType = 'Dword'}
                // match change with rule in rules list, change the checkContent to be the content in the changelog
                string[] splits = change.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                this.changeLog.Add(change);
                foreach(Rule rule in this.Rules)
                {
                    if (rule.Rules[0].GroupId == splits[0])
                    {
                        // found a rule to replace check content
                        //Regex.Replace(rule.Rules[0].CheckContent, ".*", splits[2]);
                        rule.Rules[0].ModifiedCheckContent = true;
                        rule.Rules[0].OriginalCheckContent = rule.Rules[0].CheckContent;
                        rule.Rules[0].CheckContent = splits[2];
                        break;
                    }
                }
            }
        }
        public static List<VRule> GetMultiLineRule(XmlNode RuleXml)
        {
            List<VRule> rules = new List<VRule>();



            return rules;
        }
        public static VRule GetSingleLineRule(XmlNode RuleXml)
        {
            VRule vRule = null;



            return vRule;
        }

        public static List<Rule> PopulateRule(XmlNode RuleXml)
        {
            List<Rule> ruleList = new List<Rule>();
            string type = VRule.GetRuleType(RuleXml);
            string checkContent = "";
            if (type == "RegistryRule")
            {
                if(RegistryVRule.IsMultilineRegEntry(checkContent))
                {
                    //ruleList = 
                }
                else
                {

                }
            }
            else if (type == "ManualRule")
            {

            }
            else if (type == "HardCodedRule")
            {

            }




                foreach (XmlNode child in RuleXml.ChildNodes)
                {
                }

                // get rule type
                // call isMultiLine method
                // process rule/s


                return ruleList;
        }

        public static List<Rule> PopulatePreProcessedRules(string FilePath)
        {
            // get log file to process manual changes
            List<Rule> myList = new List<Rule>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Group");

            // get checkContent
            // if multiline
            // get multiple lines of checkContent
            // foreach checkContent, create a new RULE object
            // only diff is the check content, so all other properties are the same

       


            foreach (XmlNode node in groupRules)
            {
                string checkContent = VRule.GetCheckContent(node);
                string ruleType = VRule.GetRuleType(node);
                bool isMultiline = false;
                switch(ruleType)
                {
                    case "RegistryRule":
                        isMultiline = RegistryVRule.IsMultilineRegEntry((checkContent));
                        break;
                    case "ManualRule":
                        isMultiline = ManualVRule.IsMultiline(checkContent);
                        break;
                    case "HardcodedRule":
                        isMultiline = ManualVRule.IsMultiline(checkContent);
                        break;
                }
                
                //if(VRule.)
                Rule rule = new Rule();
                rule.FilePath = FilePath;
                foreach (XmlNode child in node.ChildNodes)
                {
                    VRule vRule = new VRule();
                    if (child.Name.ToLower() == "title")
                    {

                        rule.Title = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "description")
                    {
                        rule.Description = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "rule")
                    {
                        //vRule.RuleId = node.Attributes["id"].Value;
                        //var newRule = new VRule();

                        List<VRule> newRules = new List<VRule>();
                        vRule.Severity = child.Attributes["severity"].Value;
                        vRule.GroupId = node.Attributes["id"].InnerText;
                        foreach (XmlNode ruleChildNode in child.ChildNodes)
                        {

                            if (ruleChildNode.Name.ToLower() == "title")
                            {
                                vRule.RuleTitle = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "description")
                            {
                                vRule.RuleDescription = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "ensure")
                            {
                                vRule.Ensure = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "version")
                            {
                                vRule.Version = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "ident")
                            {
                                vRule.Identifiers.Add(ruleChildNode.InnerText);
                            }
                            else if (ruleChildNode.Name.ToLower() == "fixtext")
                            {
                                vRule.FixText = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "fix")
                            {
                                vRule.FixId = ruleChildNode.Attributes["id"].InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "check")
                            {
                                vRule.CheckSystem = ruleChildNode.Attributes["system"].Value;
                                foreach (XmlNode checkChildNode in ruleChildNode.ChildNodes)
                                {
                                    if (checkChildNode.Name.ToLower() == "check-content")
                                    {
                                        vRule.CheckContent = checkChildNode.InnerText;

                                        // get isAfinding and isNotAFinding
                                        vRule.IsAFinding = VRule.GetIsAFindingString(vRule.CheckContent);
                                        vRule.IsNotAFinding = VRule.GetIsNotAFindingString(vRule.CheckContent);

                                        // determine rule type
                                        // based on type, properties populated will be different
                                        newRules = VRule.GetSpecificRule(vRule);
                                        // maybe VRule static method to populate data based on rule type
                                    }
                                    else if (checkChildNode.Name.ToLower() == "check-content-ref")
                                    {
                                        vRule.CheckContentRefHref = checkChildNode.Attributes["href"].InnerText;
                                    }
                                }

                            }
                            else
                            {
                                string temp = ruleChildNode.InnerText;
                            }
                        }
                        //if(newRule.Ensure == null || newRule.Ensure.Length == 0)
                        // {
                        //     newRule.Ensure = "Present";
                        // }
                        foreach (VRule r in newRules)
                        {
                            rule.Rules.Add(r);
                        }

                    }
                }
                //i++;
                //newRules.Add(rule);
            }



            if (RegistryVRule.IsMultilineRegEntryFileCheck(FilePath))
            {
                List<Rule> rules = new List<Rule>();
                XmlDocument xmlDoc1 = new XmlDocument();
                xmlDoc.Load(FilePath);
                XmlNodeList groupRules1 = xmlDoc1.GetElementsByTagName("Group");
                foreach(XmlNode node in groupRules1)
                {
                    Rule rule = new Rule();
                    List<Rule>multiList = new List<Rule>();
                    rule.FilePath = FilePath;
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        VRule vRule = new VRule();
                        if (child.Name.ToLower() == "title")
                        {
                            rule.Title = child.InnerText;
                        }
                        else if (child.Name.ToLower() == "description")
                        {
                            rule.Description = child.InnerText;
                        }
                        else if (child.Name.ToLower() == "rule")
                        {
                            List<VRule> newRules = new List<VRule>();
                            vRule.Severity = child.Attributes["severity"].Value;
                            vRule.GroupId = node.Attributes["id"].InnerText;
                            foreach (XmlNode ruleChildNode in child.ChildNodes)
                            {
                                if (ruleChildNode.Name.ToLower() == "title")
                                {
                                    vRule.RuleTitle = ruleChildNode.InnerText;
                                }
                                else if (ruleChildNode.Name.ToLower() == "description")
                                {
                                    vRule.RuleDescription = ruleChildNode.InnerText;
                                }
                                else if (ruleChildNode.Name.ToLower() == "ensure")
                                {
                                    vRule.Ensure = ruleChildNode.InnerText;
                                }
                                else if (ruleChildNode.Name.ToLower() == "version")
                                {
                                    vRule.Version = ruleChildNode.InnerText;
                                }
                                else if (ruleChildNode.Name.ToLower() == "ident")
                                {
                                    vRule.Identifiers.Add(ruleChildNode.InnerText);
                                }
                                else if (ruleChildNode.Name.ToLower() == "fixtext")
                                {
                                    vRule.FixText = ruleChildNode.InnerText;
                                }
                                else if (ruleChildNode.Name.ToLower() == "fix")
                                {
                                    vRule.FixId = ruleChildNode.Attributes["id"].InnerText;
                                }
                                else if (ruleChildNode.Name.ToLower() == "check")
                                {
                                    vRule.CheckSystem = ruleChildNode.Attributes["system"].Value;
                                    foreach (XmlNode checkChildNode in ruleChildNode.ChildNodes)
                                    {
                                        if (checkChildNode.Name.ToLower() == "check-content")
                                        {
                                            vRule.CheckContent = checkChildNode.InnerText;

                                            // get isAfinding and isNotAFinding
                                            vRule.IsAFinding = VRule.GetIsAFindingString(vRule.CheckContent);
                                            vRule.IsNotAFinding = VRule.GetIsNotAFindingString(vRule.CheckContent);

                                            // determine rule type
                                            // based on type, properties populated will be different
                                            newRules = VRule.GetSpecificRule(vRule);
                                            // maybe VRule static method to populate data based on rule type
                                        }
                                        else if (checkChildNode.Name.ToLower() == "check-content-ref")
                                        {
                                            vRule.CheckContentRefHref = checkChildNode.Attributes["href"].InnerText;
                                        }
                                    }

                                }
                                else
                                {
                                    string temp = ruleChildNode.InnerText;
                                }
                            }
                            //if(newRule.Ensure == null || newRule.Ensure.Length == 0)
                            // {
                            //     newRule.Ensure = "Present";
                            // }
                            foreach (VRule r in newRules)
                            {
                                rule.Rules.Add(r);
                            }

                        }
                    }
                }

            }




            //XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Group");
            int i = 0;
            /*
            foreach (XmlNode node in groupRules)
            {
                Rule rule = new Rule();
                rule.FilePath = FilePath;
                foreach (XmlNode child in node.ChildNodes)
                {
                    VRule vRule = new VRule();
                    if (child.Name.ToLower() == "title")
                    {

                        rule.Title = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "description")
                    {
                        rule.Description = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "rule")
                    {
                        //vRule.RuleId = node.Attributes["id"].Value;
                        //var newRule = new VRule();

                        List<VRule> newRules = new List<VRule>();
                        vRule.Severity = child.Attributes["severity"].Value;
                        vRule.GroupId = node.Attributes["id"].InnerText;
                        foreach (XmlNode ruleChildNode in child.ChildNodes)
                        {

                            if (ruleChildNode.Name.ToLower() == "title")
                            {
                                vRule.RuleTitle = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "description")
                            {
                                vRule.RuleDescription = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "ensure")
                            {
                                vRule.Ensure = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "version")
                            {
                                vRule.Version = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "ident")
                            {
                                vRule.Identifiers.Add(ruleChildNode.InnerText);
                            }
                            else if (ruleChildNode.Name.ToLower() == "fixtext")
                            {
                                vRule.FixText = ruleChildNode.InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "fix")
                            {
                                vRule.FixId = ruleChildNode.Attributes["id"].InnerText;
                            }
                            else if (ruleChildNode.Name.ToLower() == "check")
                            {
                                vRule.CheckSystem = ruleChildNode.Attributes["system"].Value;
                                foreach (XmlNode checkChildNode in ruleChildNode.ChildNodes)
                                {
                                    if (checkChildNode.Name.ToLower() == "check-content")
                                    {
                                        vRule.CheckContent = checkChildNode.InnerText;

                                        // get isAfinding and isNotAFinding
                                        vRule.IsAFinding = VRule.GetIsAFindingString(vRule.CheckContent);
                                        vRule.IsNotAFinding = VRule.GetIsNotAFindingString(vRule.CheckContent);

                                        // determine rule type
                                        // based on type, properties populated will be different
                                        newRules = VRule.GetSpecificRule(vRule);
                                        // maybe VRule static method to populate data based on rule type
                                    }
                                    else if (checkChildNode.Name.ToLower() == "check-content-ref")
                                    {
                                        vRule.CheckContentRefHref = checkChildNode.Attributes["href"].InnerText;
                                    }
                                }

                            }
                            else
                            {
                                string temp = ruleChildNode.InnerText;
                            }
                        }
                        //if(newRule.Ensure == null || newRule.Ensure.Length == 0)
                        // {
                        //     newRule.Ensure = "Present";
                        // }
                        foreach(VRule r in newRules)
                        {
                            rule.Rules.Add(r);
                        }
                        
                    }
                }
                i++;
                rules.Add(rule);
            }
            */
            return myList;
        }
        public static void OutputStigToDisk(string Path, ref Stig OutputStig)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlElement root = doc.CreateElement("DISASTIG");
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
                    // check if doc contains ManualRule as a child of DISA
                    // if so, just add the rule as a child
                    // if not create the node and add the rule as a child

                    // could be a manual rule or a document rule
                    XmlNode existingNode = doc.SelectSingleNode("/DISASTIG/ManualRule");
                    XmlElement manElement;
                    if(existingNode != null)
                    {
                        manElement = (XmlElement)existingNode;
                        manElement.SetAttribute("dscresourcemodule", "None");
                        root.AppendChild(manElement);
                    }
                    else
                    {
                        manElement = doc.CreateElement("ManualRule");
                        manElement.SetAttribute("dscresourcemodule", "None");
                        root.AppendChild(manElement);
                    }

                    XmlElement ruleElement = doc.CreateElement("Rule");
                    ruleElement.SetAttribute("id", rule.Rules[0].GroupId);
                    ruleElement.SetAttribute("severity", rule.Rules[0].Severity);
                    ruleElement.SetAttribute("conversionstatus", "");
                    ruleElement.SetAttribute("title", rule.Rules[0].RuleTitle);
                    ruleElement.SetAttribute("dscresource", "RegistryPolicyFile");
                    manElement.AppendChild(ruleElement);

                    XmlElement ruleChild = doc.CreateElement("Description");
                    ruleChild.InnerText = rule.Description;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("DuplicateOf");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Ensure");
                    //ruleChild.InnerText = rule.Rules[0].Ensure;
                    ruleChild.InnerText = "Present";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("IsNullOrEmpty");
                    ruleChild.InnerText = "False";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("LegacyId");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueRequired");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueTestString");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("RawString");
                    ruleElement.AppendChild(ruleChild);
                }
                else if (rule.Rules[0].DscResource == "RegistryPolicyFile")
                {
                    XmlElement regElement;
                    RegistryVRule regVRule = new RegistryVRule();
                    XmlNode existingNode = doc.SelectSingleNode("/DISASTIG/RegistryRule");
                    if (existingNode != null)
                    {
                        regElement = (XmlElement)existingNode;
                        //regElement.SetAttribute("dscresourcemodule", "PDDscResources");
                        root.AppendChild(regElement);
                    }
                    else
                    {
                        regElement = doc.CreateElement("RegistryRule");
                        regElement.SetAttribute("dscresourcemodule", "PDDscResources");
                        root.AppendChild(regElement);
                    }

                    XmlElement ruleElement = doc.CreateElement("Rule");
                    ruleElement.SetAttribute("id", rule.Rules[0].GroupId);
                    ruleElement.SetAttribute("severity", rule.Rules[0].Severity);
                    ruleElement.SetAttribute("conversionstatus", "");
                    ruleElement.SetAttribute("title", rule.Title);
                    regElement.AppendChild(ruleElement);

                    XmlElement ruleChild = doc.CreateElement("Description");
                    ruleChild.InnerText = rule.Rules[0].RuleDescription;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("DuplicateOf");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Ensure");
                    //ruleChild.InnerText = rule.Rules[0].Ensure;
                    ruleChild.InnerText = "Present";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("IsNullOrEmpty");
                    ruleChild.InnerText = "False";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Key");
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryKey;
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
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryValueData;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueName");
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryValueName;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueType");
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryType;
                    ruleElement.AppendChild(ruleChild);

                    ruleElement.SetAttribute("dscresource", regVRule.GetDscResource(regVRule.FixText));
                }
                else if (rule.Rules[0].DscResource == "Registry")
                {
                    XmlElement regElement;
                    RegistryVRule regVRule = new RegistryVRule();
                    XmlNode existingNode = doc.SelectSingleNode("/DISASTIG/RegistryRule");
                    if (existingNode != null)
                    {
                        regElement = (XmlElement)existingNode;
                        //regElement.SetAttribute("dscresourcemodule", "PDDscResources");
                        root.AppendChild(regElement);
                    }
                    else
                    {
                        regElement = doc.CreateElement("RegistryRule");
                        regElement.SetAttribute("dscresourcemodule", "PDDscResources");
                        root.AppendChild(regElement);
                    }

                    XmlElement ruleElement = doc.CreateElement("Rule");
                    ruleElement.SetAttribute("id", rule.Rules[0].GroupId);
                    ruleElement.SetAttribute("severity", rule.Rules[0].Severity);
                    ruleElement.SetAttribute("conversionstatus", "");
                    ruleElement.SetAttribute("title", rule.Title);
                    regElement.AppendChild(ruleElement);

                    XmlElement ruleChild = doc.CreateElement("Description");
                    ruleChild.InnerText = rule.Rules[0].RuleDescription;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("DuplicateOf");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Ensure");
                    //ruleChild.InnerText = rule.Rules[0].Ensure;
                    ruleChild.InnerText = "Present";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("IsNullOrEmpty");
                    ruleChild.InnerText = "False";
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Key");
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryKey;
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
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryValueData;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueName");
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryValueName;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueType");
                    regVRule = (RegistryVRule)rule.Rules[0];
                    ruleChild.InnerText = regVRule.Data.RegistryType;
                    ruleElement.AppendChild(ruleChild);

                    ruleElement.SetAttribute("dscresource", regVRule.GetDscResource(regVRule.FixText));
                }

            }

            doc.Save(Path);
            // check for manualRules
            // check for registryRules
            // set attribute for reg rules -> dscresourcemodule="PSDscResources"



        }
    }
}
