using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Data;

namespace ParseXccdf
{
    internal class Program
    {
        static List<PostProcessedVRule> PopulatePostProcessedRules(string FilePath)
        {
            // check if this is the xccdf file and not the post processed version.
            List<PostProcessedVRule> rules = new List<PostProcessedVRule>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);

            XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Rule");

            foreach (XmlNode node in groupRules)
            {
                PostProcessedVRule ppvr = new PostProcessedVRule();   
                ppvr.RuleId = node.Attributes["id"].InnerText;
                ppvr.Severity = node.Attributes["severity"].InnerText;
                ppvr.RuleTitle = node.Attributes["title"].InnerText;
                ppvr.DscResource = node.Attributes["dscresource"].InnerText;
                ppvr.FilePath = FilePath;

                foreach (XmlNode child in node.ChildNodes)
                {
                    
                    if (child.Name.ToLower() == "duplicateof")
                    {
                        ppvr.DuplicateOf = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "description")
                    {
                        ppvr.RuleDescription = child.InnerText;
                    }
                    else if(child.Name.ToLower() == "legacyid")
                    {
                        ppvr.LegacyId = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "organizationalvaluerequired")
                    {
                        ppvr.OrganizationalValueRequired =  bool.Parse(child.InnerText);
                    }
                    else if (child.Name.ToLower() == "rawstring")
                    {
                        ppvr.RawString = child.InnerText;
                    }
                    else if (child.Name.ToLower() == "isnullorempty")
                    {
                        ppvr.IsNullOrEmpty = child.InnerText;
                    }
                }
                // before adding to list, populat dscResource specific stuff, so else if on DscResource
                rules.Add(ppvr);
            }



            return rules;
        }
        static List<Rule> PopulatePreProcessedRules(string FilePath)
        {
            // check if this is the xccdf file and not the post processed version.

            List<Rule> rules = new List<Rule>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);

            XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Group");

            foreach(XmlNode node in groupRules)
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
                        rule.Rules.Add(vRule);
                    }
                }
                rules.Add(rule);
            }

            return rules;
        }
        static List<string> ListVRules(string FilePath)
        {
            var list = new List<string>();
            File.ReadAllText(FilePath);
            XElement root = XElement.Parse(File.ReadAllText(FilePath));
            string pattern = @"v-\d{3,6}$|v-\d{3,6}\.\w$";
            Regex regex = new Regex(pattern);

            var elements = from el in root.Descendants()
                           where el.Attribute("id") != null &&
                           regex.IsMatch(el.Attribute("id").Value.ToLower())
                           select el.Attribute("id");

            List<string> attributeValues = elements.Select(attr => attr.Value).ToList();

            return attributeValues;
        }

        static Stig GetPostProcessedStig(string FilePath)
        {

            List<PostProcessedVRule> newRuleList = PopulatePostProcessedRules(FilePath);


            Stig stig = new Stig();
            stig.FilePath = FilePath;
            stig.PostProcessRules = newRuleList;
            stig.Product = GetPostProcessedProduct(FilePath);
            stig.Company = GetPostProcessedCompany(FilePath);
            stig.StigVersion = GetStigVersion(FilePath);


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);

            XmlNode benchmarkNode = xmlDoc.SelectSingleNode("*");

            foreach (XmlNode node in benchmarkNode.ChildNodes)
            {
                if (node.Name.ToLower() == "title")
                {
                    stig.Title = node.InnerText;
                }
                else if (node.Name.ToLower() == "description")
                {
                    stig.Description = node.InnerText;
                }
            }
            // Get the attribute value
            //if (specificNode != null && specificNode.Attributes["attributeName"] != null)
            //{
            //    string attributeValue = specificNode.Attributes["attributeName"].Value;
            //    Console.WriteLine($"Attribute Value: {attributeValue}");
            //}

            return stig;
        }
        static Stig GetPreProcessedStig(string FilePath)
        {
            List<Rule> newRuleList = PopulatePreProcessedRules(FilePath);

            Stig stig = new Stig();
            stig.FilePath = FilePath;
            stig.Rules = newRuleList;
            stig.Product = GetPreProcessedProduct(FilePath);
            stig.Company = GetPreProcessedCompany(FilePath);
            stig.StigVersion = GetStigVersion(FilePath);


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);

            XmlNode benchmarkNode = xmlDoc.SelectSingleNode("*");

            foreach(XmlNode node in benchmarkNode.ChildNodes)
            {
                if (node.Name.ToLower() == "title")
                {
                    stig.Title = node.InnerText;
                }
                else if(node.Name.ToLower() == "description")
                {
                    stig.Description = node.InnerText;
                }
            }
            // Get the attribute value
            //if (specificNode != null && specificNode.Attributes["attributeName"] != null)
            //{
            //    string attributeValue = specificNode.Attributes["attributeName"].Value;
            //    Console.WriteLine($"Attribute Value: {attributeValue}");
           //}

            return stig;
        }
        static ArrayList GetStigs(string FolderPath)
        {


            return new ArrayList();
        }
        static string GetStigVersion(string StigFilePath)
        {
            string returnVersion = "";
            if(StigFilePath.ToLower().Contains("xccdf"))
            {
                try
                {
                    Regex regex = new Regex(@"V\dR\d", RegexOptions.IgnoreCase);
                    string[] preSplit = StigFilePath.Split('_');
                    int i = 0;
                    foreach (string str in preSplit)
                    {
                        if (regex.IsMatch(str)) { break; }
                        i++;
                    }
                    returnVersion = preSplit[i];
                    returnVersion = returnVersion.Trim('V');
                    returnVersion = returnVersion.Replace('R', '.');
                }
                catch (Exception ex)
                {
                    returnVersion = ex.Message;
                }
            }
            else
            {
                try
                {
                    Regex regex = new Regex(@"\d.\d+.xml", RegexOptions.IgnoreCase);
                    Match match = regex.Match(StigFilePath);
                    string value = match.Value;
                    returnVersion = value.Replace(".xml", "");
                }
                catch (Exception ex)
                {
                    returnVersion = ex.Message;
                }
            }



            return returnVersion;
        }
        static string GetPreProcessedCompany(string data)
        {
            try
            {
                string[] splits = data.Split('_');
                return splits[1];
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        static string GetPreProcessedProduct(string data)
        {
            string product = "";
            try
            {
                string[] splits = data.Split('_');

                string pattern = @"V\dR\d";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                int start = 1;
                int end = 0;
                
                foreach (string str in splits)
                {
                    if (regex.IsMatch(str))
                    {
                        break;
                    }
                    end++;
                }

                for (int i = start; i < end; i++)
                {
                    product += splits[i];
                    product += "_";
                }


                product = product.Replace("_STIG", "");

            }
            catch (Exception ex)
            {
                product = ex.Message;
            }

            return product.TrimEnd('_');
        }
        static string GetPostProcessedCompany(string data)
        {
            string returnParts = "";
            try
            {
                string[] splits = data.Split('\\');
                string[] parts = splits[splits.Count() - 1].Split('-');
                returnParts =  parts[0];
            }
            catch (Exception ex)
            {
                returnParts = ex.Message;
            }
            return returnParts;

        }
        static string GetPostProcessedProduct(string data)
        {
            // read in xml
            // get value in attribute filename="Adobe_Acrobat_Reader_DC_Continuous_Track_STIG"
            // trim value to match the preProcessed Product Name

            string product = "";
            try
            {
                string xmlContent = File.ReadAllText(data);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);
                XmlElement root = xmlDoc.DocumentElement;
                if (root != null && root.HasAttribute("filename"))
                {
                    product = root.GetAttribute("filename");

                }
                else
                {
                    Console.WriteLine("Attribute not found.");
                }

                string[] splits = product.Split('_');
                string pattern = @"V\dR\d";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                int start = 1;
                int end = 0;
                foreach (string str in splits)
                {
                    if (regex.IsMatch(str))
                    {
                        break;
                    }
                    end++;
                }

                product = "";
                for (int i = start; i < end; i++)
                {
                    product += splits[i];
                    product += "_";
                }

                product = product.Replace("_STIG", "");
            }
            catch (Exception ex)
            {
                product = ex.Message;
            }
            return product.TrimEnd('_');

        }
        static ArrayList GetPreProcessedStigs(string FolderPath)
        {
            // get all .xml files excluding org files
            ArrayList fullRules = new ArrayList();

            try
            {
                string[] files = Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories);
                var filteredFiles = files.Where(file => Path.GetFileName(file).Contains("-xccdf.xml"));
                foreach (string file in filteredFiles)
                {
                    Stig stig = GetPreProcessedStig(file);
                    stig.IsPostProcessed = false;
                    fullRules.Add(stig);
                }
            }
            catch (Exception ex)
            {
                fullRules.Add(ex.Message);
            }

            return fullRules;
        }
        static ArrayList GetPostProcessedStigs(string XMLFolderPath)
        {
            // get all .xml files excluding org files
            ArrayList fullRules = new ArrayList();

            try
            {
                string[] processessedFiles = Directory.GetFiles(XMLFolderPath, "*", SearchOption.AllDirectories);
                var filteredFiles = processessedFiles.Where(file => !Path.GetFileName(file).Contains("org"));

                foreach (string file in filteredFiles)
                {
                    Stig stig = GetPostProcessedStig(file);
                    stig.Product = GetPostProcessedProduct(file);
                    stig.Company = GetPostProcessedCompany(file);
                    stig.IsPostProcessed = true;
                    fullRules.Add(stig);
                }
            }
            catch (Exception ex)
            {
                fullRules.Add (ex.Message);
            }

            return fullRules;
        }
        /*
        static bool CompareRules(Stig Rule1, Stig Rule2)
        {
            bool match = false;
            string pattern = @"V-\d{3,6}";
            
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
         
            string newRule1 = "";
            string newRule2 = "";
            if(Rule1.FilePath.ToLower().Contains("rhel"))
            {
                string temp = "";
            }
            foreach (Rule rule1 in Rule1.V_Rules)
            {
                match = false;
                regex.Match(rule1.);
                newRule1 = regex.Match(rule1).Value;

                
                foreach (string rule2 in Rule2.V_Rules)
                {
                    regex.Match(rule2);
                    newRule2 = regex.Match(rule2).Value;

                  
                    if (newRule1.Equals(newRule2)) 
                    { 
                        match = true;
                        break;
                    }
                }
                if(!match)
                {
                    Console.WriteLine($"{rule1} did not have a rule that matched in {Rule2.FilePath}");
                }
            }
            foreach (string rule2 in Rule2.V_Rules)
            {
                match = false;
                regex.Match(rule2);
                newRule2 = regex.Match(rule2).Value;


                foreach (string rule1 in Rule1.V_Rules)
                {
                    regex.Match(rule1);
                    newRule1 = regex.Match(rule1).Value;


                    if (newRule2.Equals(newRule1))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    Console.WriteLine($"{rule2} did not have a rule that matched in {Rule1.FilePath}");
                }
            }
            return match;

        }
        */
        static void CompareStigLists(List<Stig> PreProcessedList, List<Stig> PostProcessedList)
        {

            Stig.CompareStigLists(PreProcessedList, PostProcessedList);

        }
        static void Main(string[] args)
        {

            /*
             Command line 
            --preprocessedFolderPath "C:\git\PowerStig\source\StigData\Archive" --PostProcessedFolderPath "C:\git\PowerStig\source\StigData\Processed"
            --listRulesFilePath "C:\git\PowerStig\source\StigData\Archive\Linux.RHEL\U_RHEL_9_STIG_V2R3_Manual-xccdf.xml"
             */
            var argDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string preprocessedFolderPath = String.Empty;
            string postprocessedFolderPath = String.Empty;

            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    argDictionary[args[i]] = args[i + 1];
                }
            }
            // list rules of a pre or post processed xml
            if (argDictionary.TryGetValue("--listRulesFilePath", out string ruleFile))
            {
                Console.WriteLine($"Rules in file: {ruleFile}");
                //string[] rules = ListVRules(ruleFile);
                List<string> ruleList = ListVRules(ruleFile);
                foreach (string rule in ruleList)
                {
                    Console.WriteLine($"{rule}");
                }
            }
            else
            {
                if (argDictionary.TryGetValue("--preprocessedFolderPath", out string preFolderPathArg))
                {
                    preprocessedFolderPath = preFolderPathArg;
                }

                if (argDictionary.TryGetValue("--postprocessedFolderPath", out string postFolderPathArg))
                {
                    postprocessedFolderPath = postFolderPathArg;
                }

                if (preprocessedFolderPath.Length <= 0 && postprocessedFolderPath.Length <= 0)
                {
                    Console.WriteLine("Enter a --PreProcessedFolderPath and a --PostProcessedFolderPath to continue.");
                }
                else
                {
                    ArrayList xccdList = GetPreProcessedStigs(preprocessedFolderPath);
                    ArrayList xmlList = GetPostProcessedStigs(postprocessedFolderPath);

                    try
                    {
                        CompareStigLists(xccdList.Cast<Stig>().ToList(), xmlList.Cast<Stig>().ToList());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
