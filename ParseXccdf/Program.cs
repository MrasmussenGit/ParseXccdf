using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Data;
using System.Web;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;

namespace ParseXccdf
{
    internal class Program
    {

        static Stig GetVRules(string XMLContent, string FilePath, string Version)
        {
            XElement root = XElement.Parse(XMLContent);
            string pattern = @"v-\d{3,6}$|v-\d{3,6}\.\w$";
            Regex regex = new Regex(pattern);

            var elements = from el in root.Descendants()
                           where el.Attribute("id") != null &&
                           regex.IsMatch(el.Attribute("id").Value.ToLower())
                           select el.Attribute("id");

            List<string> attributeValues = elements.Select(attr => attr.Value).ToList();

            Stig stig = new Stig();
            stig.FilePath = FilePath;
            stig.V_Rules = attributeValues.ToArray();
            stig.StigVersion = Version;
            return stig;
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
        static string GetPreProcessedVersion(string fileName)
        {
            string preSplitVersion = "";
            try
            {
                Regex regex = new Regex(@"V\dR\d", RegexOptions.IgnoreCase);
                string[] preSplit = fileName.Split('_');
                int i = 0;
                foreach (string str in preSplit)
                {
                    if (regex.IsMatch(str)) { break; }
                    i++;
                }
                preSplitVersion = preSplit[i];
                preSplitVersion = preSplitVersion.Trim('V');
                preSplitVersion = preSplitVersion.Replace('R', '.');
            }
            catch (Exception ex)
            {
                preSplitVersion = ex.Message;
            }

            return preSplitVersion;
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
        static string GetPostProcessedVersion(string data)
        {
            string newVersion = "";
            try
            {
                Regex regex = new Regex(@"\d.\d+.xml", RegexOptions.IgnoreCase);
                Match match = regex.Match(data);
                string value = match.Value;
                newVersion = value.Replace(".xml", "");
            }
            catch (Exception ex)
            {
                newVersion = ex.Message;
            }
            return newVersion;
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
                    if (file.ToLower().Contains("rhel"))
                    {
                        string temp = "";
                    }
                    string content = File.ReadAllText(file);

                    Stig stig = GetVRules(content, file, GetPreProcessedVersion(file));
                    stig.Product = GetPreProcessedProduct(file);
                    stig.Company = GetPreProcessedCompany(file);
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
                    Stig stig = GetVRules(File.ReadAllText(file), file, GetPostProcessedVersion(file));
                    stig.Product = GetPostProcessedProduct(file);
                    stig.Company = GetPostProcessedCompany(file);

                    fullRules.Add(stig);
                }
            }
            catch (Exception ex)
            {
                fullRules.Add (ex.Message);
            }

            return fullRules;
        }
        static bool FileNameMatch(string preProcessedFileName, string postProcessedFileName)
        {
            bool match = false;
            Regex regex = new Regex(@"V\dR\d", RegexOptions.IgnoreCase);
            string[] preSplit = preProcessedFileName.Split('_');
            string[] postSplit = postProcessedFileName.Split('-');
            int indexPost = postSplit[0].IndexOf(preSplit[1]);
            string company = postSplit[0].Substring(indexPost, postSplit[0].Length - indexPost);

            if (company.ToLower().Equals(preSplit[1].ToLower())) 
            {
                // match
                // check version match

                // get pre split version, location unknown
                int i = 0;
                foreach (string str in preSplit) 
                {
                    if(regex.IsMatch(str)) { match = true; break; }
                    i++;
                }
                string preSplitVersion = preSplit[i];
                preSplitVersion =  preSplitVersion.Trim('V');
                preSplitVersion = preSplitVersion.Replace('R', '.');

                string postSplitVersion = postSplit[2].Replace(".xml", "");

                if(preSplitVersion.Equals(postSplitVersion))
                {
                    match = true;
                }
            }

            return match;
        }
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
            foreach (string rule1 in Rule1.V_Rules)
            {
                match = false;
                regex.Match(rule1);
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
        static void CompareStigLists(ArrayList PreProcessedList, ArrayList PostProcessedList)
        {
            // for each preprocessed
            // find file name match in postProcessedList
            // compare rule list
            bool match = false;
            string ruleName = "";
            Console.WriteLine("Comparing lists");

            foreach(Stig preRule in PreProcessedList)
            {
                foreach(Stig postRule in PostProcessedList)
                {
                    // if product and version match, we found the objects to compare
                    if(preRule.Product.ToLower().Equals(postRule.Product.ToLower()))
                    {
                        if(preRule.StigVersion.Equals(postRule.StigVersion))
                        {
                            Console.WriteLine($"Comparing {preRule.FilePath} to file {postRule.FilePath} ");
                            // compare rules list, report on issues
                            if (CompareRules(preRule, postRule))
                            {
                                match = true;
                                ruleName = postRule.FilePath;
                            }
                        }
                    }
                    if (match) { break; }
                    

                    // isolate the file names of each
                    // look for manufacture/product/version?
                }
                if (match) 
                {
                    string[] postSplits = ruleName.Split('\\');
                    string[] preSplits = preRule.FilePath.Split('\\');
                    //Console.WriteLine($"XCCDF {preSplits[preSplits.Length - 1]} matched to XML {postSplits[postSplits.Length - 1]}");
                    match = false;
                }

            }

            Console.WriteLine("Compare completed");

        }
        static void Main(string[] args)
        {
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
                    CompareStigLists(xccdList, xmlList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            //string xmlFolderPath = "C:\\git\\PowerStig\\source\\StigData\\Processed";
            //string xccdFolderPath = "C:\\git\\PowerStig\\source\\StigData\\Archive";


            


            // CompareStigLists(xccdList, xmlList);



            //string convertedXMLPath = "C:\\git\\PowerStig\\source\\StigData\\Processed\\Adobe-AcrobatPro-2.1.xml";
            //string convertedXMLContent = File.ReadAllText(convertedXMLPath);

            //string XccdfFilePath = @"C:\PowerStigRHEL\U_Oracle_Linux_8_STIG_V2R3_Manual-xccdf.xml";
            //string XccdfFilePath = @"C:\PowerStigRHEL\test\U_Adobe_Acrobat_Pro_DC_Continuous_V2R1_Manual-xccdf.xml";
            //string XccdXmlContent = File.ReadAllText(XccdfFilePath);

            // compare the rules in coverted to archived.  All rules should be accounted for
            //string[] convertedRules = GetConvertedRules(xmlFolderPath);
            //string[] xccdfRules = GetXCCDFRules(xccdfFolderPath);





            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xmlContent);
            //List<string> xccdfRules = GetVRules(XccdXmlContent);
            //List<string> convertedRules = GetVRules(convertedXMLContent);

            //List<string> uniqueToXccdf = xccdfRules.Except(convertedRules).ToList();
            //List<string> uniqueToConvertedXML = convertedRules.Except(xccdfRules).ToList();
            /*
            Console.WriteLine("Only in XCCDF:");
            if(uniqueToXccdf.Count > 0)
            {
                foreach (var rule in uniqueToXccdf)
                {
                    Console.WriteLine("\t" + rule);
                }
            }
            else
            {
                Console.WriteLine("\tNone");
            }

            Console.WriteLine("Only in XML");
            if (uniqueToConvertedXML.Count > 0)
            {
                foreach (var rule in uniqueToConvertedXML)
                {
                    Console.WriteLine("\t" + rule);
                }
            }
            else
            {
                Console.WriteLine("\tNone");
            }
            */

        }
    }
}
