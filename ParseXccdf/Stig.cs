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
using System.Collections;

namespace ParseXccdf
{
    internal class Stig
    {
        private List<Rule> rules;
        private List<Rule> fullCheckContentRules;
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
            this.fullCheckContentRules = new List<Rule>();
            this.postProcessRules = new List<PostProcessedVRule>();
            this.changeLog = new List<string>();
        }
        public List<Rule> FullCheckContentRules
        {
            get { return this.fullCheckContentRules; }
            set { this.fullCheckContentRules = value; }
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
        private static List<object> FindDuplicates(ArrayList arrayList)
        {
            // finds duplicates in the arrayList and returns them

            Dictionary<object, int> countDict = new Dictionary<object, int>();
            List<object> duplicates = new List<object>();

            foreach (var item in arrayList)
            {
                if (countDict.ContainsKey(item))
                {
                    countDict[item]++;
                }
                else
                {
                    countDict[item] = 1;
                }
            }

            foreach (var kvp in countDict)
            {
                if (kvp.Value > 1)
                {
                    for (int i = 0; i < kvp.Value - 1; i++)
                    {
                        duplicates.Add(kvp.Key);
                    }

                }
            }

            return duplicates;
        }

        public static void GetFileInfo(string FilePath)
        {
            XDocument xmlDoc = XDocument.Load(FilePath);
            XElement root = xmlDoc.Root;
            ArrayList rules = new ArrayList();

            void TraverseElement(XElement element, int level = 0)
            {

                foreach (XAttribute attribute in element.Attributes())
                {
                    if (attribute.Name.ToString().Trim().Equals("id"))
                    {
                        rules.Add(attribute.Value);
                    }
                }

                foreach (XElement child in element.Elements())
                {
                    TraverseElement(child, level + 1);
                }
            }

            TraverseElement(root);

            List<object> dupList = FindDuplicates(rules);
            List<object> usedList = new List<object>();
            int count = 0;
            Console.WriteLine($"{FilePath}");
            if (dupList.Count <= 0)
            {
                Console.WriteLine("\tNo Duplicates");
            }
            foreach (var dup in dupList)
            {

                count = dupList.Count(item => item == dup);

                if (!usedList.Contains(dup))
                {
                    Console.WriteLine($"\tRule '{dup}' appears {count + 1} total times.");
                    usedList.Add(dup);
                }

            }

        }

        public static void GetFolderInfo(string DirectoryPath)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Console.WriteLine($"Directory: {DirectoryPath} does not exist");
                return;
            }
            else
            {
                string[] files = Directory.GetFiles(DirectoryPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    GetFileInfo(file);
                }
            }
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
        public static List<Rule> PopulatePreProcessedRules(string FilePath)
        {
            // this is the one is use
            List<Rule> myList = new List<Rule>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Group");
            Stig stig = new Stig();
            Rule rule = new Rule();
            foreach (XmlNode node in groupRules)
            {
                rule = new Rule();
                VRule vRule = new VRule();
                rule.FilePath = FilePath;
                foreach (XmlNode child in node.ChildNodes)
                {
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
                                        vRule.OriginalCheckContent = checkChildNode.InnerText;
                                        vRule.IsAFinding = VRule.GetIsAFindingString(vRule.CheckContent);
                                        vRule.IsNotAFinding = VRule.GetIsNotAFindingString(vRule.CheckContent);
                                        string[] ruleTypeData = VRule.GetRuleType(vRule.CheckContent);
                                        vRule.RuleType = ruleTypeData[0];
                                        vRule.DscResource = ruleTypeData[1];
                                        
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
                    }
                }
                rule.Rules.Add(vRule);
                myList.Add(rule);
            }
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
            root.SetAttribute("fullversion", OutputStig.StigVersion);
            root.SetAttribute("created", DateTime.Now.ToShortDateString());
            doc.AppendChild(root);

            // add node here regRule, manualRule then append the below

            foreach(Rule rule in OutputStig.Rules)
            {
                if (rule.Rules[0].DscResource == null || rule.Rules[0].DscResource == "None")
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

                    ruleChild = doc.CreateElement("LegacyId");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueRequired");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueTestString");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("RawString");
                    ruleChild.InnerText = rule.Rules[0].CheckContent;
                    ruleElement.AppendChild(ruleChild);
                }
                else if (rule.Rules[0].DscResource == "RegistryPolicyFile" || rule.Rules[0].DscResource == "RegistryRule")
                {
                    XmlElement regElement;
                    RegistryVRule regVRule = new RegistryVRule();
                    XmlNode existingNode = doc.SelectSingleNode("/DISASTIG/RegistryRule");
                    if (existingNode != null)
                    {
                        regElement = (XmlElement)existingNode;
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
                    ruleElement.SetAttribute("conversionstatus", "Pass");
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

                    ruleChild = doc.CreateElement("LegacyId");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueRequired");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("OrganizationValueTestString");
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("RawString");
                    ruleChild.InnerText = rule.Rules[0].CheckContent;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("Key");
                    regVRule = new RegistryVRule();
                    Utilities.CopyProperties(rule.Rules[0], regVRule);
                    RegistryVRule orignialRuleKey = (RegistryVRule) rule.Rules[0];
                    regVRule = RegistryVRule.PopulateRegistryData(orignialRuleKey.Data, regVRule);
                    ruleChild.InnerText = regVRule.Data.RegistryKey;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueData");
                    regVRule = new RegistryVRule();
                    Utilities.CopyProperties(rule.Rules[0], regVRule);
                    RegistryVRule orignialRuleValue = (RegistryVRule)rule.Rules[0];
                    regVRule = RegistryVRule.PopulateRegistryData(orignialRuleValue.Data, regVRule);
                    ruleChild.InnerText = regVRule.Data.RegistryValueData;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueName");
                    regVRule = new RegistryVRule();
                    Utilities.CopyProperties(rule.Rules[0], regVRule);
                    RegistryVRule orignialRuleValueName = (RegistryVRule)rule.Rules[0];
                    regVRule = RegistryVRule.PopulateRegistryData(orignialRuleValueName.Data, regVRule);
                    ruleChild.InnerText = regVRule.Data.RegistryValueName;
                    ruleElement.AppendChild(ruleChild);

                    ruleChild = doc.CreateElement("ValueType");
                    regVRule = new RegistryVRule();
                    Utilities.CopyProperties(rule.Rules[0], regVRule);
                    
                    RegistryVRule orignialRuleValueType = (RegistryVRule)rule.Rules[0];
                    regVRule = RegistryVRule.PopulateRegistryData(orignialRuleValueType.Data, regVRule);
                    ruleChild.InnerText = regVRule.Data.RegistryType;
                    ruleElement.AppendChild(ruleChild);
                    string dscResource = regVRule.GetDscResource(regVRule.FixText);
                    ruleElement.SetAttribute("dscresource", dscResource);
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

            string outputFilePath = Stig.GetPostProcessedFileName(OutputStig.FileNameAndPath);
            if (outputFilePath != null)
            {
                // save to same directory as the exe if getting the file path fails
                outputFilePath = @"c:\test\adobe-acrobatreader-2.1.xml";
            }
            outputFilePath = @"c:\test\adobe-acrobatreader-2.1.xml";
            doc.Save(outputFilePath);
            // check for manualRules
            // check for registryRules
            // set attribute for reg rules -> dscresourcemodule="PSDscResources"



        }
        public static List<string> GetPreProcessedRuleList(string FilePath)
        {
            List<string> ruleIdList = new List<string>();
            var list = new List<string>();
            File.ReadAllText(FilePath);
            XElement root = XElement.Parse(File.ReadAllText(FilePath));
            string pattern = @"v-\d{3,6}$|v-\d{3,6}\.\w$";
            Regex regex = new Regex(pattern);

            var elements = from el in root.Descendants()
                           where el.Attribute("id") != null &&
                           regex.IsMatch(el.Attribute("id").Value.ToLower())
                           select el.Attribute("id");

            ruleIdList = elements.Select(attr => attr.Value).ToList();

            return ruleIdList;

        }
        public static List<string> GetPostProcessedRuleList(string FilePath)
        {
            List<string> ruleIdList = new List<string>();
            var list = new List<string>();
            File.ReadAllText(FilePath);
            XElement root = XElement.Parse(File.ReadAllText(FilePath));
            string pattern = @"v-\d{3,6}$|v-\d{3,6}\.\w$";
            Regex regex = new Regex(pattern);

            var elements = from el in root.Descendants()
                           where el.Attribute("id") != null &&
                           regex.IsMatch(el.Attribute("id").Value.ToLower())
                           select el.Attribute("id");

            ruleIdList = elements.Select(attr => attr.Value).ToList();

            return ruleIdList;
        }
        public static void CompareStigs(List<string>PreProcessedList, List<string>PostProcessedList)
        {
            List<string> normalizedPreProcessedList = PreProcessedList.Select(s => Regex.Replace(s, @"\.\w+$", "")).ToList();
            List<string> normalizedPostProcessedList = PostProcessedList.Select(s => Regex.Replace(s, @"\.\w+$", "")).ToList();


            var onlyInPreProcessedList = normalizedPreProcessedList.Except(normalizedPostProcessedList);
            var onlyInPostProcessedList = normalizedPostProcessedList.Except(normalizedPreProcessedList);

            if(onlyInPreProcessedList.Count() <= 0)
            {
                Console.WriteLine("Values in PreProcessedList but not in PostProcessedList: \nNone");
            }
            else
            {
                Console.WriteLine("Values in PreProcessedList but not in PostProcessedList: \n" + string.Join("\n", onlyInPreProcessedList));
            }
            if (onlyInPostProcessedList.Count() <= 0)
            {
                Console.WriteLine("Values in PostProcessedList but not in PreProcessedList: \nNone");
            }
            else
            {
                Console.WriteLine("Values in PostProcessedList but not in PreProcessedList: \n" + string.Join("\n", onlyInPostProcessedList));
            }
            
            
        }
        public static string GetPostProcessedFileName(string PreProcessedFilePath)
        {
            // create post processed file name from the file path to the xccdf file
            // format: Company-Product-Version.xml (Adobe-AcrobatPro-2.1.xml)
            // there is also an org file
            // format: Company-Product-Version.org.default.xml (Adobe-AcrobatPro-2.1.org.default.xml)

            string postProcessedFileName = null;






            return postProcessedFileName;
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
                if (RegistryVRule.IsMultilineRegEntry(checkContent))
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
        public static List<Rule> PopulatePreProcessedRulesSingleCheckContent(string FilePath)
        {
            List<Rule> myList = new List<Rule>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Group");
            foreach (XmlNode node in groupRules)
            {
                List<string> splitCheckContent = new List<string>();
                string checkContent = VRule.GetCheckContent(node);
                string ruleType = VRule.GetRuleType(node);
                bool isMultiline = false;
                switch (ruleType)
                {
                    case "RegistryRule":
                        isMultiline = RegistryVRule.IsMultilineRegEntry((checkContent));
                        if (isMultiline) { splitCheckContent = RegistryVRule.RegGetMultilineCheckContent(checkContent); }
                        else { splitCheckContent.Add(checkContent); }
                        break;
                    case "ManualRule":
                        isMultiline = ManualVRule.IsMultiline(checkContent);
                        break;
                    case "HardcodedRule":
                        isMultiline = ManualVRule.IsMultiline(checkContent);
                        break;
                    case "RootCertificateRule":
                        isMultiline = RootCertificateVRule.IsMutliCertContent(checkContent);
                        break;
                }

                foreach (string splitContent in splitCheckContent)
                {

                }
            }

            return myList;
        }
    }
}
