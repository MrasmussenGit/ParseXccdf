using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ParseXccdf
{
    internal class VRule
    {
        private string groupId;
        private List<string> identifiers;
        private string ruleId;
        private string severity;
        private string version;
        private string ruleTitle;
        private string ruleDescription;
        private string fixText;
        private string trimmedFixText;
        private string fixId;
        private string checkSystem;
        private string checkContentRefHref;
        private string checkContent;
        private string originalCheckContent;
        private string trimmedRuleId;
        private string ruleType;
        private string dscResource;
        private string ensure;
        private bool modifiedCheckContent;
        private List<string> isAFinding;
        private List<string> isNotAFinding;

        #region Properties
        public VRule()
        {
            identifiers = new List<string>();
            isAFinding = new List<string>();
            isNotAFinding = new List<string>();
        }
        
        public string GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        public List<string> Identifiers
        {
            get { return identifiers; }
            set { identifiers = value; }
        }

        public string RuleId
        {
            get { return ruleId; }
            set 
            {
                ruleId = value;
                this.trimmedRuleId = TrimPostProcessRuleId(value);
            }
        }

        public string Severity
        {
            get { return severity; }
            set { severity = value; }
        }
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        public string RuleTitle
        {
            get { return ruleTitle; }
            set { ruleTitle = value; }
        }

        public string RuleDescription
        {
            get { return ruleDescription; }
            set { ruleDescription = value; }
        }

        public string FixText
        {
            get { return fixText; }
            set 
            { 
                fixText = value;
                TrimmedFixText = TrimFixText(value);
            }
        }
        public string TrimmedFixText
        {
            get { return trimmedFixText; }
            set { trimmedFixText = value; }
        }
        public string FixId
        {
            get { return fixId; }
            set { fixId = value; }
        }
        public string CheckSystem
        {
            get { return checkSystem; }
            set{ checkSystem = value; }
        }

        public string CheckContentRefHref
        {
            get { return checkContentRefHref; }
            set { checkContentRefHref = value; }
        }
        public string OriginalCheckContent
        {
            get { return originalCheckContent; }
            set { originalCheckContent = value; }
        }
        public string CheckContent
        {
            get { return checkContent; }
            set { checkContent = value; }

        }
        public string TrimmedRuleId
        {
            get { return trimmedRuleId; }
            set { trimmedRuleId = value; }
        }
        public string RuleType
        {
            get { return ruleType; }
            set { ruleType = value; }
        }
        public string DscResource
        {
            get { return dscResource; }
            set { dscResource = value; }
        }
        public List<string>IsAFinding
        {
            get { return isAFinding; }
            set { isAFinding = value; }
        }
        public List<string> IsNotAFinding
        {
            get { return isNotAFinding; }
            set { isNotAFinding = value; }
        }
        public bool ModifiedCheckContent
        {
            get { return modifiedCheckContent; }
            set { modifiedCheckContent = value; }
        }
        public string Ensure
        {
            get { return ensure; }
            set { ensure = value; }
        }
        #endregion 
        public static string TrimFixText(string FixText)
        {
            string trimmedText = "";
            // remove all after value: x


            return trimmedText;
        }
        public static string GetCheckContent(XmlNode Rule)
        {
            string checkContent = "";
            foreach (XmlNode child in Rule.ChildNodes)
            {
                if (child.Name.ToLower() == "rule")
                {
                    foreach (XmlNode ruleChildNode in child.ChildNodes)
                    {
                        if (ruleChildNode.Name.ToLower() == "check")
                        {
                            foreach (XmlNode checkChildNode in ruleChildNode.ChildNodes)
                            {
                                if (checkChildNode.Name.ToLower() == "check-content")
                                {
                                    checkContent = checkChildNode.InnerText;
                                }
                            }
                        }
                    }
                }
            }

            // clean checkContent - remove newlines


            return checkContent;
        }
        public static string GetRuleType(XmlNode RuleXml)
        {
            List<VRule> rules = new List<VRule>();
            string type = "";
            // original code has trim extra lines from content
            string checkContent = GetCheckContent(RuleXml);


            if (IsRegistryRule(checkContent))
            {
                type = "RegistryPolicyFile";
            }
            else if (IsHardCodedRule(checkContent))
            {
                type = "HardCodedRule";
            }
            else if (IsAccountPolicyRule(checkContent))
            {
            }
            else if (IsAuditPolicyRule(checkContent))
            {
            }
            else if (IsDnsServerSettingRule(checkContent))
            {
            }
            else if (IsDnsServerRootHintRule(checkContent))
            {

            }
            else if (IsFileContentRule(checkContent))
            {

            }
            else if (IsGroupRule(checkContent))
            {

            }
            else if (IsIISLoggingRule(checkContent))
            {

            }
            else if (IsGroupRule(checkContent))
            {

            }
            else if (IsMimeTypeRule(checkContent))
            {

            }
            else if (IsPermissionRule(checkContent))
            {

            }
            else if (IsProcessMitigationRule(checkContent))
            {

            }
            else if (IsSecurityOptionsRule(checkContent))
            {

            }
            else
            {
                type = "ManualRule";
            }
            return type;
            //return ruleType;
        }
        public static string GetRuleType(string CheckContent)
        {
            List<VRule> rules = new List<VRule>();
            string type = "";
            // original code has trim extra lines from content

            if (IsRegistryRule(CheckContent))
            {
                type = "RegistryPolicyFile";
            }
            else if (IsHardCodedRule(CheckContent))
            {
                type = "HardCodedRule";
            }
            else if (IsAccountPolicyRule(CheckContent))
            {
            }
            else if (IsAuditPolicyRule(CheckContent))
            {
            }
            else if (IsDnsServerSettingRule(CheckContent))
            {
            }
            else if (IsDnsServerRootHintRule(CheckContent))
            {

            }
            else if (IsFileContentRule(CheckContent))
            {

            }
            else if (IsGroupRule(CheckContent))
            {

            }
            else if (IsIISLoggingRule(CheckContent))
            {

            }
            else if (IsGroupRule(CheckContent))
            {

            }
            else if (IsMimeTypeRule(CheckContent))
            {

            }
            else if (IsPermissionRule(CheckContent))
            {

            }
            else if (IsProcessMitigationRule(CheckContent))
            {

            }
            else if (IsSecurityOptionsRule(CheckContent))
            {

            }
            else
            {
                type = "ManualRule";
            }
            if(type == null || type.Length == 0)
            {
                type = "ManualRule";
            }
            return type;
        }
        public static List<VRule> GetSpecificRule(VRule VRule)
        {
            List<VRule> rules = new List<VRule>();
            VRule returnRule = new VRule();
            // original code has trim extra lines from content



            if (IsRegistryRule(VRule.CheckContent))
            {
                List<RegistryVRule> rulesToAdd = ConvertToRegRule(VRule);
                foreach(RegistryVRule r in rulesToAdd)
                {
                    rules.Add(r);
                }
            }
            else if (IsHardCodedRule(VRule.CheckContent))
            {
            }
            else if (IsAccountPolicyRule(VRule.CheckContent))
            {
            }
            else if (IsAuditPolicyRule(VRule.CheckContent))
            {
            }
            else if (IsDnsServerSettingRule(VRule.CheckContent))
            {
            }
            else if(IsDnsServerRootHintRule(VRule.CheckContent))
            {

            }
            else if (IsFileContentRule(VRule.CheckContent))
            {

            }
            else if (IsGroupRule(VRule.CheckContent))
            {

            }
            else if (IsIISLoggingRule(VRule.CheckContent))
            {

            }
            else if (IsGroupRule(VRule.CheckContent))
            {

            }
            else if(IsMimeTypeRule(VRule.CheckContent))
            {

            }
            else if(IsPermissionRule(VRule.CheckContent))
            {

            }
            else if(IsProcessMitigationRule(VRule.CheckContent))
            {

            }
            else if(IsSecurityOptionsRule(VRule.CheckContent))
            {

            }
            else
            {
                // manual rule
                //rules = ManualVRule.Clone(VRule);
            }
                return rules;
            //return ruleType;
        }
        public static VRule PopulateAdditionalData(VRule Rule)
        {
            // switch on Rule type
            // call specific populate of that type
            switch(Rule.RuleType)
            {
                case "RegistryPolicyFile":
                    RegistryVRule newRegRule = new RegistryVRule();
                    RegistryVRule.CopyProperties(Rule, newRegRule);
                    newRegRule = RegistryVRule.PopulateRegistryVRule(newRegRule);
                    return newRegRule;

                    break;
                case "HardCodedRule":
                    break;
            }


            return Rule;
        }
        public static RegistryVRule PopulateRegistryVRule(RegistryVRule Rule)
        {

            Rule.Data.RegistryKey = RegistryVRule.GetRegKeyFromContent(Rule.CheckContent);
            Rule.Data.RegistryValueName = RegistryVRule.GetRegValueName(Rule.CheckContent);
            Rule.Data.RegistryType = RegistryVRule.GetRegValueDataType(Rule.CheckContent);
            Rule.Data.RegistryValueData = RegistryVRule.GetRegValueData(Rule.CheckContent);

            return Rule;
        }

        #region ConvertToTypes
        private static List<RegistryVRule> ConvertToRegRule(VRule Rule)
        {
            RegistryVRule regVRule = new RegistryVRule(); 
            RegistryVRule.CopyProperties(Rule, regVRule);
            regVRule.IsAFinding = VRule.GetIsAFindingString(Rule.CheckContent);
            regVRule.IsNotAFinding = VRule.GetIsNotAFindingString(Rule.CheckContent);

            // test if value is multiline?
            
        //    if(regVRule.IsMultilineRegEntry(Rule.CheckContent))
        //    {
                string temp = "";
                // check content contains multiple reg keys and values?  populate data list in regVRule
                // create multiple rules, one for each reg entry

       //     }
       //     else
       //     {
                regVRule.Data.RegistryKey = RegistryVRule.GetRegKeyFromContent(regVRule.CheckContent);
                regVRule.Data.RegistryValueName = RegistryVRule.GetRegValueName(regVRule.CheckContent);
                regVRule.Data.RegistryType = RegistryVRule.GetRegValueDataType(regVRule.CheckContent);
                regVRule.Data.RegistryValueData = RegistryVRule.GetRegValueData(regVRule.CheckContent);
      //      }

            // items reg data is checked and adjusted for
            //  isBlank
            //  isEnabledOrDisabled
            //  isHexCode
            //  isInteger
            //  this.ValueType = 'MultiString'
            //
            //  if ($regData match "see below") -> GetMultiValueRegistryStringData($this.RawString)
            //  else -> FormatMultiStringRegistryData($registryValueData)



            // needs works
            //regVRule.FixText = RegistryVRule.TrimRegFixText(Rule.FixText);
            regVRule.FixText = Rule.FixText;
            regVRule.trimmedFixText = RegistryVRule.TrimRegistryFixText(Rule.FixText);


            regVRule.DscResource = GetDscResourceValue(regVRule.FixText, regVRule.Data.RegistryKey, regVRule.Data.RegistryValueName);



            //regVRule.ValueData = GetRegValueData(regVRule.CheckContent);
       // $this.SetValueType($rawString)
       // $this.SetDuplicateRule()
       // $this.SetDscResource($fixText)

            List<RegistryVRule> rules = new List<RegistryVRule>();
            rules.Add(regVRule);

            return rules;
        }
        #endregion

        #region IsRuleTypes
        private static bool IsHardCodedRule(string CheckContent)
        {
            return Regex.IsMatch(CheckContent, "HardCodedRule");
        }
        private static bool IsAccountPolicyRule(string CheckContent)
        {
            return Regex.IsMatch(CheckContent, "Navigate to.+Windows Settings\\s*(-|>)?>\\s*Security Settings\\s*(-|>)?>\\s*Account Policies");
        }
        private static bool IsAuditPolicyRule(string CheckContent)
        {
            bool isMatch = false;
            if(Regex.IsMatch(CheckContent, "\bAuditpol\b") &&
               !Regex.IsMatch(CheckContent, "resourceSACL"))
            {
                isMatch = true;
            }
            return isMatch;
        }
        private static bool IsDnsServerSettingRule(string CheckContent)
        {
            bool isMatch = false;
            if(Regex.IsMatch(CheckContent, "dnsmgmt\\.msc") &&
                !Regex.IsMatch(CheckContent, "Forward Lookup Zones") &&
                !Regex.IsMatch(CheckContent, @"Logs\\Microsoft") &&
                !Regex.IsMatch(CheckContent, "Verify the \"root hints\"")
                )
            {
                isMatch = true;
            }
            return isMatch;
        }
        private static bool IsDnsServerRootHintRule(string CheckContent)
        {
            bool isMatch = false;
            if(Regex.IsMatch(CheckContent, "dnsmgmt\\.msc") &&
                Regex.IsMatch(CheckContent, "Verify the \"root hints\""))
            {
                isMatch = true;
            }
            return isMatch;
        }
        private static bool IsFileContentRule(string CheckContent)
        {
            bool isMatch = false;

            return isMatch;
        }
        private static bool IsGroupRule(string CheckContent)
        {
            bool isMatch = false;

            return isMatch;
        }
        private static bool IsIISLoggingRule(string CheckContent)
        {
            bool isMatch = false;

            return isMatch;
        }
        private static bool IsMimeTypeRule(string CheckContent)
        {
            bool isMatch = false;

            return isMatch;
        }
        private static bool IsPermissionRule(string CheckContent)
        {
            bool isMatch = false;

            return isMatch;
        }
        private static bool IsProcessMitigationRule(string CheckContent)
        {
            bool isMatch = false;

            return isMatch;
        }
        private static bool IsRegistryRule(string CheckContent)
        {
            bool isReg = false;
            string lmkeyPattern = @".*HKEY_LOCAL_MACHINE.*";
            string luKeyPattern = @".*HKEY_CURRENT_USER.*";
            if ((Regex.IsMatch(CheckContent, lmkeyPattern) || Regex.IsMatch(CheckContent, luKeyPattern) &&
                !Regex.IsMatch(CheckContent, "Permission(s|)") &&
                !Regex.IsMatch(CheckContent, "Sql Server") &&
                !Regex.IsMatch(CheckContent, "Sql Server") &&
                !Regex.IsMatch(CheckContent, "Review the Catalog") &&
                !Regex.IsMatch(CheckContent, "For 32.bit (production systems|applications)") &&
                !Regex.IsMatch(CheckContent, "If the \"AllowStrongNameBypass\" registry key") &&
                !Regex.IsMatch(CheckContent, "DSA Database file") &&
                !Regex.IsMatch(CheckContent, "N'HKEY_LOCAL_MACHINE'"))
                ||
                (Regex.IsMatch(CheckContent, "Windows Registry Editor") &&
                 Regex.IsMatch(CheckContent, "HKLM|HKCU"))
                ||
                (Regex.IsMatch(CheckContent, "HKLM|HKCU") &&
                 Regex.IsMatch(CheckContent, "REG_DWORD"))
                ||
                (Regex.IsMatch(CheckContent, "regedit") &&
                 Regex.IsMatch(CheckContent, "omnibox")))
                
                
            {
                isReg = true;
            }
            return isReg;
        }
        private static bool IsSecurityOptionsRule(string CheckContent)
        {
            bool isMatch = false;

            if(Regex.IsMatch(CheckContent, "Logging") &&
                !Regex.IsMatch(CheckContent, "IIS 8\\.5|IIS 10\\.0") &&
                !Regex.IsMatch(CheckContent, "verify only authorized groups") &&
                !Regex.IsMatch(CheckContent, "Confirm|Consult with the System Administrator") &&
                !Regex.IsMatch(CheckContent, "If an account associated with roles other than auditors") &&
                !Regex.IsMatch(CheckContent, "review source IP"))
            {
                isMatch = true;
            }
            return isMatch;
        }
        #endregion

        #region Helper Functions
        private static string GetDscResourceValue(string FixText, string RegistryKey, string RegistryValueName)
        {
            string dscResource = "";
            string pattern1 = @"Administrative Template";
            string pattern2 = @"(^hkcu|^HKEY_CURRENT_USER)";
            string pattern3 = @"RemoteAccessHostFirewallTraversal";

            if(Regex.IsMatch(FixText, pattern1) || Regex.IsMatch(RegistryKey, pattern2) || Regex.IsMatch(RegistryValueName, pattern3))
            {
                dscResource = "RegistryPolicyFile";
            }
            else
            {
                dscResource = "Registry";
            }

            return dscResource;
        }
        private static string GetRegValueDataType(string CheckContent)
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
        private static string GetRegValueData(string CheckContent)
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
        private static string GetRegValueName(string CheckContent)
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
        private static string GetMcAfeeRegistryPath(string CheckContent)
        {
            return "";
        }
        private static string GetRegKeyFromContent(string CheckContent)
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
                return regKey.Trim('\r','\n');
        }
        public static string TrimPostProcessRuleId(string RuleId)
        {
            string trimmedRuleId = "";
            if (RuleId != null)
            {
                trimmedRuleId = RuleId.Split('.')[0];
            }

            return trimmedRuleId.Trim();

        }
        public static List<string> GetIsAFindingString(string CheckContent)
        {
            List<string> result = new List<string>();

            if (CheckContent != null)
            {
                string[] splits = CheckContent.Split('\r', '\n');
                foreach (string line in splits)
                {
                    if (line.ToLower().Contains("is a finding"))
                    {
                        result.Add(line);
                    }
                }
            }

            return result;
        }
        public static List<string> GetIsNotAFindingString(string CheckContent)
        {
            List<string> result = new List<string>();

            if (CheckContent != null)
            {
                string[] splits = CheckContent.Split('\r', '\n');
                foreach (string line in splits)
                {
                    if (line.ToLower().Contains("is not a finding"))
                    {
                        result.Add(line);
                    }
                }
            }

            return result;
        }
        
        #endregion

    }
}
