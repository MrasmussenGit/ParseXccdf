using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace ParseXccdf
{
    internal class RegistryVRule : VRule
    {
        private RegistryData data;
        private List<RegistryData> dataList;
        private string ensure;
        private bool isNullOrEmpty;
        private bool isMultilineRegRule;

        public RegistryVRule()
        {
            data = new RegistryData();
        }
        public string Ensure
        {
            get { return ensure; }
            set { ensure = value; }
        }

        public bool IsNullOrEmpty
        {
            get { return isNullOrEmpty; }
            set { isNullOrEmpty = value; }
        }
        public RegistryData Data
        {
            get { return data; }
            set { data = value; }
        }
        public List<RegistryData> DataList
        {
            get { return dataList; }
            set { dataList = value; }
        }
        public bool IsMultilineRegRule
        {
            get { return isMultilineRegRule; }
            set { isMultilineRegRule = value; }
        }
        public string GetDscResource(string FixText)
        {
            string dscResource = "";
            string adminMatch = @"^.*Administrative Template.*$";
            string keyMatch = @"^(hkcu|^HKEY_CURRENT_USER)";
            string valueMatch = @".*RemoteAccessHostFirewallTraversal.*";

            if(Regex.IsMatch(FixText, adminMatch) || Regex.IsMatch(this.Data.RegistryKey, keyMatch) || Regex.IsMatch(this.Data.RegistryValueData, valueMatch))
            {
                dscResource = "RegistryPolicyFile";
            }
            else
            {
                dscResource = "RegistryPolicyFile";
            }

            return dscResource;
        }
        public static string TrimRegFixText(string FixText)
        {
            string pattern = "\r\nValue:";
            int index = FixText.IndexOf(pattern);
            string[] result = FixText.Split(new string[] { pattern }, StringSplitOptions.None);
            return string.Join("", result);
            
        }
        public static string TrimRegistryFixText(string FixText)
        {
            List<string> newFixText = new List<string>();
            string[] splits = FixText.Split('\n');
            for (int i =0; i < splits.Length - 1; i++)
            {
                if (splits[i] != "" && splits[i] != "\r")
                {
                    if (splits[i].Contains(':'))
                    {
                        if (splits[i].StartsWith("Value:"))
                        {
                            int colonIndex = splits[i].IndexOf(':');
                            if (colonIndex > 0 && colonIndex + 1 < splits[i].Length)
                            {
                                string newLine = splits[i].Trim('\r');
                                newFixText.Add(newLine);
                            }
                        }
                        else
                        {
                            newFixText.Add(splits[i]);
                        }

                    }
                    else
                    {
                        newFixText.Add(splits[i]);
                    }
                    if (splits[i].StartsWith("Value:")){ break; }
                }
                
            }
            return string.Join("",newFixText);

        }
        public static RegistryVRule PopulateRegistryData(RegistryData Data, RegistryVRule RegVRule)
        {
            RegVRule.data.RegistryKey = Data.RegistryKey;
            RegVRule.Data.RegistryType = Data.RegistryType;
            RegVRule.Data.RegistryValueData = Data.RegistryValueData;
            RegVRule.Data.RegistryValueName = Data.RegistryValueName;

            return RegVRule;
        }
        public static void CopyProperties<T>(T source, T target)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(target, prop.GetValue(source));
                }
            }
        }
        public static RegistryVRule Clone(VRule Rule)
        {
            RegistryVRule newVRule = new RegistryVRule();
            newVRule.GroupId = Rule.GroupId;
            foreach(string id in Rule.Identifiers)
            {
                newVRule.Identifiers.Add(id);
            }
            foreach(string str in Rule.IsAFinding)
            {
                newVRule.IsAFinding.Add(str);
            }
            foreach(string str in Rule.IsNotAFinding)
            {
                newVRule.IsNotAFinding.Add(str);
            }
            newVRule.RuleId = Rule.RuleId;
            newVRule.Severity = Rule.Severity;
            newVRule.Version = Rule.Version;
            newVRule.RuleTitle = Rule.RuleTitle;
            newVRule.RuleDescription = Rule.RuleDescription;
            newVRule.FixText = Rule.FixText;
            newVRule.FixId = Rule.FixId;
            newVRule.CheckSystem = Rule.CheckSystem;
            newVRule.CheckContentRefHref = Rule.CheckContentRefHref;
            newVRule.CheckContent = Rule.CheckContent;
            newVRule.TrimmedRuleId = Rule.TrimmedRuleId;
            newVRule.RuleType = Rule.RuleType;
            return newVRule;
        }
        public bool PopulateMultilineRegistryData(string CheckContent)
        {
            bool successful = false;
            // would contain multiline 


            this.Data.RegistryKey = "";
            this.Data.RegistryKey = "";
            this.Data.RegistryKey = "";
            this.Data.RegistryKey = "";
            this.Data.RegistryKey = "";

            return successful;
        }
        public RegistryData PopulateSingleLineRegistryData(string CheckContent)
        {
            bool successful = false;
            // would contain multiline 
            RegistryData data = new RegistryData();
            data.RegistryKey = "";
            data.RegistryValueData = "";
            data.RegistryValueName = "";
            data.RegistryType = "";
            data.RegistryName = "";

            return data;
        }
        public static bool IsMultilineRegEntry(string CheckContent)
        {

            // stub out as false until mutliline is worked better
            return false;
            /*
            bool isMultiline = false;
            string pattern = @"HKEY_(LOCAL_MACHINE|CURRENT_USER|CLASSES_ROOT|USERS|CURRENT_CONFIG)\\[\w\\]+";
            Regex regex = new Regex(pattern);
            MatchCollection mc = regex.Matches(CheckContent);
            
            if (mc.Count > 1)
            {
                isMultiline = true;
                foreach(Match m in mc)
                {
                    // each m is the key to a new location, grab all of the data here and store it in a collection
                    
                    string temp = "";
                }
            }
            return isMultiline;
            */
        }
        public static bool IsMultilineRegEntryFileCheck(string FilePath)
        {
            string checkContent = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList groupRules = xmlDoc.GetElementsByTagName("Group");
            foreach(XmlNode r in groupRules)
            {
                foreach(XmlNode cgr in r.ChildNodes)
                {
                    if (cgr.Name.ToLower() == "check")
                    {
                        foreach (XmlNode checkChildNode in cgr.ChildNodes)
                        {
                            if (checkChildNode.Name.ToLower() == "check-content")
                            {
                                checkContent = checkChildNode.InnerText;
                            }
                        }
                    }
                }
            }
            bool isMultiline = IsMultilineRegEntry(checkContent);
            return isMultiline;
        }
        public static string GetSingleRegValueDataType(string CheckContent)
        {
            string valueDataType = "";
            string pattern = @"Type.*:\s.*REG_(SZ|BINARY|DWORD|QWORD|MULTI_SZ|EXPAND_SZ)";

            Regex.IsMatch(CheckContent, @"Type.*:\s.*REG_(SZ|BINARY|DWORD|QWORD|MULTI_SZ|EXPAND_SZ)");
            Regex regex = new Regex(pattern);
            Match match = regex.Match(CheckContent);
            if (match.Success)
            {
                valueDataType = match.Value;
            }

            return valueDataType;
        }
        public static string GetSingleRegValueData(string CheckContent)
        {
            string valueData = "";
            string pattern = @"Value:\s.*";
            Match match = Regex.Match(CheckContent, pattern);
            if (match.Success)
            {
                valueData = match.Value.Split(':')[1];
                valueData = valueData.Trim('\r', '\n', ' ');
            }

            /* - original code has these checks after getting the reg value data ***************************************************************
             * 
                 # If a range is found on the value line, it needs further processing.
        if (($this.TestValueDataStringForRange($registryValueData)) -or ($this.RawString -match "LegalNoticeText"))
        {
            # Set the OrganizationValueRequired flag to true so that a org level setting will be required.
            $this.SetOrganizationValueRequired()

            # Try to extract a test string from the range text.
            $OrganizationValueTestString = $this.GetOrganizationValueTestString($registryValueData)

            if ($this.RawString -match "LegalNoticeText")
            {
                $LegalNoticeTextOrganizationValueTestString = '{0} is set to the required legal notice before logon'
                $this.set_OrganizationValueTestString($LegalNoticeTextOrganizationValueTestString)
            }

            # If a test string was returned, add it.
            if ($null -ne $OrganizationValueTestString)
            {
                $this.set_OrganizationValueTestString($OrganizationValueTestString)
            }
        }
        else
        {
            if ($this.IsDataBlank($registryValueData))
            {
                $this.SetIsNullOrEmpty()
                $registryValueData = ''
            }
            elseif ($this.IsDataEnabledOrDisabled($registryValueData))
            {
                $registryValueData = $this.GetValidEnabledOrDisabled(
                    $this.ValueType, $registryValueData
                )
            }
            elseif ($this.IsDataHexCode($registryValueData))
            {
                $registryValueData = $this.GetIntegerFromHex($registryValueData)
            }
            elseif ($this.IsDataInteger($registryValueData))
            {
                $registryValueData = $this.GetNumberFromString($registryValueData)
            }
            elseif ($this.ValueType -eq 'MultiString')
            {
                if ($registryValueData -match "see below")
                {
                    $registryValueData = $this.GetMultiValueRegistryStringData($this.RawString)
                }
                else
                {
                    $registryValueData = $this.FormatMultiStringRegistryData($registryValueData)
                }
            }
            $this.Set_ValueData($registryValueData)
             * */

            return valueData;
        }
        public static string GetSingleRegValueName(string CheckContent)
        {
            string valueName = "";
            string trimmedValueName = "";
            string pattern = @"Value\sName:.*";
            Match match = Regex.Match(CheckContent, pattern);
            if (match.Success)
            {
                valueName = match.Value.Split(':')[1];
                valueName = valueName.Trim('\r', '\n', ' ');
            }

            return valueName;
        }
        public static string GetMcAfeeRegistryPath(string CheckContent)
        {
            return "";
        }
        public static string GetSingleRegKeyFromContent(string CheckContent)
        {
            string regKey = "";
            if (Regex.IsMatch(CheckContent, @"HKEY_LOCAL_MACHINE\\Software\\McAfee\\\s\(32-bit\)|HKLM\\Software\\Wow6432Node\\McAfee\\\s\(64-bit\)"))
            {
                regKey = GetMcAfeeRegistryPath(CheckContent);
            }
            else
            {
                string pattern = @"(HKEY_LOCAL_MACHINE|HKEY_CURRENT_USER|HKEY_CLASSES_ROOT|HKEY_USERS|HKEY_PERFORMANCE_DATA|HKEY_CURRENT_CONFIG).*";
                Match match = Regex.Match(CheckContent, pattern);
                regKey = match.Value;
            }
            return regKey.Trim('\r', '\n');
        }
        public static string[] SplitMultilineContent(string CheckContent)
        {
            string delimiter = "<splitRule>";
            string[] rules = CheckContent.Split(new string[] { delimiter }, StringSplitOptions.None);
            foreach (string rule in rules)
            {
                // create an .a and .b etc for each of the rules
                
            }




            return rules;
        }
        public static List<string> RegGetMultilineCheckContent(string CheckContent)
        {





            return new List<string> { CheckContent };
        }
    }
}
