using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Reflection;
using System.Net.Http.Headers;

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
              //  if (ppvr.RuleId.Contains('.'))
              //  {
              //      ppvr.TrimmedRuleId = ppvr.RuleId.Split('.')[0];
              //  }
              //  else
              //  {
              //      ppvr.TrimmedRuleId = ppvr.RuleId;
              //  }

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
                        else if (child.Name.ToLower() == "legacyid")
                        {
                            ppvr.LegacyId = child.InnerText;
                        }
                        else if (child.Name.ToLower() == "organizationalvaluerequired")
                        {
                            ppvr.OrganizationalValueRequired = bool.Parse(child.InnerText);
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
        static string[] GetPreStigTitleAndDescription(string FilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string[] returnList = new string[2];
            xmlDoc.Load(FilePath);

            XmlNode benchmarkNode = xmlDoc.SelectSingleNode("*");

            foreach (XmlNode node in benchmarkNode.ChildNodes)
            {
                if (node.Name.ToLower() == "title")
                {
                    returnList[0] = node.InnerText;
                }
                else if (node.Name.ToLower() == "description")
                {
                    returnList[1] = node.InnerText;
                }
            }
            return returnList;
        }
        static Stig GetPostProcessedStig(string FilePath)
        {

            List<PostProcessedVRule> newRuleList = PopulatePostProcessedRules(FilePath);

            string[] splits = FilePath.Split('-');
            Stig stig = new Stig();
            stig.IsPostProcessed = true;
            string[] pathSplits = splits[0].Split('\\');
            string prefix = pathSplits[pathSplits.Length - 1];
            if (prefix.ToLower() == "office" || prefix.ToLower() == "office")
            {
                string officeYearPattern = @"\d{4}";
                Regex regex = new Regex(officeYearPattern);

                stig.IsOfficeProduct = true;
                if (regex.IsMatch(splits[1]))
                {
                    stig.OfficeProduct = Regex.Replace(splits[1], @"[^a-zA-Z]", "");
                }
                else
                {
                    stig.OfficeProduct = splits[1];
                }
                regex = new Regex(officeYearPattern, RegexOptions.IgnoreCase);
                Match match = regex.Match(splits[1]);
                stig.OfficeYear = match.Value;
            }
            stig.FilePath = FilePath;
            stig.PostProcessRules = newRuleList;
            stig.Product = GetPostProcessedProduct(FilePath, ref stig);
            stig.Company = GetPostProcessedCompany(FilePath);
            stig.StigVersion = GetStigVersion(FilePath);
            stig.Purpose = GetPostProcessedPurpose(FilePath);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);

            XmlNode disaNode = xmlDoc.SelectSingleNode("*");
            stig.Title = disaNode.Attributes["title"].Value;
            stig.Description = disaNode.Attributes["description"].Value;
            stig.OriginalFile = disaNode.Attributes["filename"].Value;

            return stig;
        }
        static Stig GetPreProcessedStig(string FilePath)
        {
            Stig stig = new Stig();
            stig.FilePath = FilePath;
            stig.OriginalFile = FilePath;
            //stig.Rules = PopulatePreProcessedRules(FilePath);
            stig.Rules = Stig.PopulatePreProcessedRules(FilePath);
            stig.Product = GetPreProcessedProduct(FilePath, ref stig);
            // could move this to the GetPreProcessedProduct function
            if (stig.IsOfficeProduct) 
            { 
                stig.OfficeYear = Stig.GetOfficeYear(FilePath); 
            }
            else
            {
                stig.Company = GetPreProcessedCompany(FilePath);
            }
            stig.StigVersion = GetStigVersion(FilePath);
            stig.Purpose = GetPreProcessedPurpose(FilePath);
            stig.Notice = GetPreProcessNotice(FilePath);
            stig.Source = GetPreProcessedSource(FilePath);
            string[] titleAndDescription = GetPreStigTitleAndDescription(FilePath);
            stig.Title = titleAndDescription[0];
            stig.Description = titleAndDescription[1];

            // process change log
            stig.ProcessChangeLog();
            // process exception list

            return stig;
        }
        static string GetPreProcessedSource(string FilePath)
        {
            string source = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                if (node.Name.ToLower() == "benchmark")
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name.ToLower() == "reference")
                        {
                            foreach(XmlNode refNode in childNode.ChildNodes)
                            {
                                if(refNode.Name.ToLower().Contains("source"))
                                {
                                    source = refNode.ChildNodes[0].InnerText;
                                    break;
                                }
                                //source = refNode.Attributes["id"].InnerText;
                                
                            }
                            
                        }
                        if(source.Length > 0)
                        {
                            break;
                        }
                    }
                }
                if(source.Length > 0)
                {
                    break;
                }
            }
            return source;
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
            string company = "";
            try
            {
                string[] splits = data.Split('_');
                company =  splits[1];
                if (company == "MS") { company = "Microsoft"; }
            }
            catch (Exception ex)
            {
                company = ex.Message;
            }
            return company;
        }
        static string GetPreProcessNotice(string FilePath)
        {
            string notice = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            foreach(XmlNode node in xmlDoc.ChildNodes)
            {
                if(node.Name.ToLower() == "benchmark")
                {
                    foreach(XmlNode childNode in  node.ChildNodes)
                    {
                        if(childNode.Name.ToLower() == "notice")
                        {
                            notice = childNode.Attributes["id"].InnerText;
                        }
                    }
                }
                
            }
            return notice;
        }
        static string GetPostProcessedPurpose(string data)
        {
            string purpose = "";
            try
            {
                string[] splitString = data.Split('\\');
                string fileName = splitString[splitString.Length - 1];
                if (fileName.ToLower().Contains("microsoft") || fileName.ToLower().Contains("windows") || fileName.ToLower().Contains("ms"))
                {
                    string[] fileNameSplit = fileName.Split('-');
                    for (int i = 1; i < fileNameSplit.Length; i++)
                    {
                        if (fileNameSplit[i].ToLower() == "ms")
                        {
                            purpose = "MS";
                            break;
                        }
                        else if (fileNameSplit[i].ToLower() == "dc")
                        {
                            purpose = "DC";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                purpose = ex.Message;
            }
            return purpose;
        }
        static string GetPreProcessedPurpose(string data)
        {
            string purpose = "";
            try
            {
                string[] splitString = data.Split('\\');
                string fileName = splitString[splitString.Length - 1];
                if(fileName.ToLower().Contains("microsoft") || fileName.ToLower().Contains("windows") || fileName.ToLower().Contains("ms"))
                {
                    string[] splits = fileName.Split('_');
                    for (int i = 2; i < splits.Length; i++)
                    {
                        if (splits[i].ToLower() == "ms")
                        {
                            purpose = "MS";
                        }
                        else if(splits[i].ToLower() == "dc")
                        {
                            purpose = "DC";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                purpose = ex.Message;
            }
            return purpose;
        }
        static string GetPostProcessedCompany(string data)
        {
            string company = "";
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(data);
                string stigId = xmlDoc.ChildNodes[0].Attributes["stigid"].Value;
                if(!(stigId == ""))
                {
                    string[] companySplits = stigId.Split('_');
                    company = companySplits[0];
                    if(company == "Windows")
                    {
                        company = "Microsoft";
                    }
                }
                else
                {
                    string[] splits = data.Split('\\');
                    string[] parts = splits[splits.Count() - 1].Split('-');
                    company = parts[0];
                }
            }
            catch(Exception ex)
            {
                company = ex.Message;
            }
            if (company.ToLower() == "ms" || company.ToLower() == "dc" || company.ToLower() == "iis" || company.ToLower() == "ie") { company = "Microsoft"; }
            return company;

        }
        static string GetPreProcessedProduct(string data, ref Stig CurrentStig)
        {
            string product = "";
            try
            {
                string[] splits = data.Split('_');

                // is it an office product
                string officeProduct = Stig.GetPreOfficeProduct(data);
                if (officeProduct != "")
                {
                    // get office year
                    product = officeProduct;
                    CurrentStig.IsOfficeProduct = true;
                    CurrentStig.OfficeYear = Stig.GetOfficeYear(data);
                    CurrentStig.Company = "Microsoft";
                }
                else
                {
                    CurrentStig.IsOfficeProduct = false;
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
                }


                product = product.Replace("_STIG", "");

            }
            catch (Exception ex)
            {
                product = ex.Message;
            }

            return product.TrimEnd('_');
        }
        static string GetPostProcessedProduct(string data, ref Stig CurrentStig)
        {
            // read in xml
            // get value in attribute filename="Adobe_Acrobat_Reader_DC_Continuous_Track_STIG"
            // trim value to match the preProcessed Product Name

            string product = "";
            try
            {
               // string[] splits = 




                //string officeProduct = Stig.GetPostOfficeProduct(data);
                //if (officeProduct != "")
                //{
                 //   product = officeProduct;
                    CurrentStig.IsOfficeProduct = true;
                    CurrentStig.OfficeYear = Stig.GetOfficeYear(data);
                    CurrentStig.Company = "Microsoft";
               // }
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
                    stig.Product = GetPostProcessedProduct(file, ref stig);
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
        static void CompareStigLists(List<Stig> PreProcessedList, List<Stig> PostProcessedList)
        {
            bool ShowOnlyErrors = true;
            Stig.CompareStigLists(PreProcessedList, PostProcessedList, ShowOnlyErrors);

        }
        static Stig GetSingleStig(string PathToXCCDF)
        {
            Stig stig = GetPreProcessedStig(PathToXCCDF);
            stig.IsPostProcessed = false;
            return stig;
        }
        static void Main(string[] args)
        {

            /*
             Command line
            # filePath will look for duplicate rules in a single STIG file
                --filePath "C:\Demo\Duplicates\Oracle-Linux-8-2.3.xml"
            # folderPath will look for duplicate rules in all of the files in a folder
                --folderPath "C:\git\PowerStig\source\StigData\Processed
            # preprecessedFilePath and PostPrcessedFilePath will compare two STIGs, outputing values missing from one or the other file
                --preprocessedFilePath "C:\git\PowerStig\source\StigData\Archive\Windows.Client\U_MS_Windows_10_STIG_V3R3_Manual-xccdf.xml" --postprocessedFilePath "C:\git\PowerStig\source\StigData\Processed\WindowsClient-10-3.3.xml"
            # preprecessedFolderPath and PostPrcessedFolderPath will compare two folders, matching STIGS and outputing duplicates (work in progress)
                --preprocessedFolderPath "C:\git\PowerStig\source\StigData\Archive" --PostProcessedFolderPath "C:\git\PowerStig\source\StigData\Processed"
            # list rule IDs of a single STIG file
                --listRulesFilePath "C:\git\PowerStig\source\StigData\Archive\Linux.RHEL\U_RHEL_9_STIG_V2R3_Manual-xccdf.xml"
            # converts a DISA stig to a DSC compatible XML document (work in progress)
                --ConvertDisaStigFilePath "C:\git\PowerStig\source\StigData\Archive\Adobe\U_Adobe_Acrobat_Pro_DC_Continuous_V2R1_Manual-xccdf.xml" --OutputFilePath "c:\test"
             */
            var argDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string preprocessedFolderPath = String.Empty;
            string postprocessedFolderPath = String.Empty;
            string preprocessedFilePath = String.Empty;
            string postprocessedFilePath = String.Empty;

            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    argDictionary[args[i]] = args[i + 1];
                }
            }
            // look for duplicates in a single file, could be pre or post processed
            if (argDictionary.TryGetValue("--FilePath", out string filePathArg))
            {
                Stig.GetFileInfo(filePathArg);
            }
            else if (argDictionary.TryGetValue("--FolderPath", out string folderPathArg))
            {
                Stig.GetFolderInfo(folderPathArg);
            }
            // list rules of a pre or post processed xml
            else if (argDictionary.TryGetValue("--listRulesFilePath", out string ruleFile))
            {
                Console.WriteLine($"Rules in file: {ruleFile}");
                //string[] rules = ListVRules(ruleFile);
                List<string> ruleList = ListVRules(ruleFile);
                foreach (string rule in ruleList)
                {
                    Console.WriteLine($"{rule}");
                }
            }
            else if(argDictionary.TryGetValue("--ConvertDisaStigFilePath", out string stigToConvertArg))
            {
                Console.WriteLine($"Converting {stigToConvertArg}");
                argDictionary.TryGetValue("--OutputFilePath", out string outputFilePath);
                Stig stig = GetSingleStig(stigToConvertArg);
                Stig.OutputStigToDisk(outputFilePath, ref stig);
                Console.WriteLine($"Conversion completed");
            }
            else
            {
                // adding comparing two files (a pre and post presumably),  Once parsing CheckContent is completed, go back to the 
                // original, commented out section below

                if (argDictionary.TryGetValue("--preprocessedFilePath", out string preFilePathArg))
                {
                    preprocessedFilePath = preFilePathArg;
                }

                if (argDictionary.TryGetValue("--postprocessedFilePath", out string postFilePathArg))
                {
                    postprocessedFilePath = postFilePathArg;
                }

                if (preprocessedFilePath.Length <= 0 && postprocessedFilePath.Length <= 0)
                {
                    Console.WriteLine("Enter a --PreProcessedFilePath and a --PostProcessedFilePath to continue.");
                }
                else
                {
                    List<string> postList = Stig.GetPostProcessedRuleList(postprocessedFilePath);
                    List<string> preList = Stig.GetPreProcessedRuleList(preFilePathArg);
                    Stig.CompareStigs(preList, postList);
                }

                /*
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
                    // list of rules in one file but not in the other
                    List<string> postProcessedRules = Stig.GetPostProcessedRuleList(postprocessedFolderPath);
                    List<string> preProcessedRules = Stig.GetPreProcessedRuleList(preprocessedFolderPath);



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
                */
                // use the above to check ALL files in a folder, populate the STIG object with the original file used to match the pre and post stig
            }
        }
    }
}
