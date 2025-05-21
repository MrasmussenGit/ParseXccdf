using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private bool isCheckContentMultiline;
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
        public bool IsCheckContentMultiline
        {
            get { return isCheckContentMultiline; }
            set { isCheckContentMultiline = value; }
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
        public static string[] GetRuleType(string CheckContent)
        {
            // return element 0 = RuleType
            // return element 1 = DscResource
            List<VRule> rules = new List<VRule>();
            string type = "";
            string DscResource = "";
            // original code has trim extra lines from content

            if (IsRegistryRule(CheckContent))
            {
                type = "RegistryPolicyFile";
                DscResource = "RegistryPolicyFile";
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
            else if(IsRootCertificateRule(CheckContent))
            {
                type = "RootCertificateRule";
                DscResource = "CertificateDSC";
            }
            else if(IsNxServiceRule(CheckContent))
            {
                type = "nxServiceRule";
                DscResource = "nxService";
            }
            else if(IsNxFileLineRule(CheckContent))
            {
                type = "nxFileLineRule";
                DscResource = "nx";
            }
            else if (IsNxFileRule(CheckContent))
            {
                type = "nxFilRule";
                DscResource = "nx";
            }
            else if (IsNxPackageRule(CheckContent))
            {
                type = "nxPackageRule";
                DscResource = "nx";
            }
            else
            {
                type = "ManualRule";
                DscResource = "None";
            }
            if(type == null || type.Length == 0)
            {
                type = "ManualRule";
                DscResource = "None";
            }
            string[] returnArray = { type, DscResource };
            return returnArray;
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
            else if(IsRootCertificateRule(VRule.CheckContent))
            {
                List<RootCertificateVRule> rulesToAdd = ConvertToRootCertRule(VRule);
                foreach (RootCertificateVRule r in rulesToAdd)
                {
                    rules.Add(r);
                }
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
                    string groupIdPattern = "V-253426*";
                    if(Utilities.WildcardMatch(Rule.GroupId, groupIdPattern) || Rule.GroupId.ToLower() == "v-253426")
                    {
                        string temp = "";
                    }
                    RegistryVRule newRegRule = new RegistryVRule();
                    Utilities.CopyProperties(Rule, newRegRule);
                    newRegRule = RegistryVRule.PopulateRegistryVRule(newRegRule);
                    return newRegRule;
                case "RootCertificateRule":
                    RootCertificateVRule rootCertRule = new RootCertificateVRule();
                    Utilities.CopyProperties(Rule, rootCertRule);
                    rootCertRule = RootCertificateVRule.PopulateRootCertificateVRule(rootCertRule);
                    break;
                case "HardCodedRule":
                    break;
                case "ManualRule":
                    break;
            }
            return Rule;
        }

        #region ConvertToTypes
        private static List<RegistryVRule> ConvertToRegRule(VRule Rule)
        {
            RegistryVRule regVRule = new RegistryVRule(); 
            Utilities.CopyProperties(Rule, regVRule);
            regVRule.IsAFinding = VRule.GetIsAFindingString(Rule.CheckContent);
            regVRule.IsNotAFinding = VRule.GetIsNotAFindingString(Rule.CheckContent);


            regVRule.Data.RegistryKey = RegistryVRule.GetRegKeyFromContent(regVRule.CheckContent);
            regVRule.Data.RegistryValueName = RegistryVRule.GetRegValueName(regVRule.CheckContent);
            regVRule.Data.RegistryType = RegistryVRule.GetRegValueDataType(regVRule.CheckContent);
            regVRule.Data.RegistryValueData = RegistryVRule.GetRegValueData(regVRule.CheckContent);
            regVRule.FixText = Rule.FixText;
            regVRule.trimmedFixText = RegistryVRule.TrimRegistryFixText(Rule.FixText);
            regVRule.DscResource = GetDscResourceValue(regVRule.FixText, regVRule.Data.RegistryKey, regVRule.Data.RegistryValueName);

            List<RegistryVRule> rules = new List<RegistryVRule>();
            rules.Add(regVRule);

            return rules;
        }
        private static List<RootCertificateVRule> ConvertToRootCertRule(VRule Rule)
        {
            RootCertificateVRule rootCertRule = new RootCertificateVRule();
            Utilities.CopyProperties(Rule, rootCertRule);
            rootCertRule.ThumbPrint = "";
            List<RootCertificateVRule> rules = new List<RootCertificateVRule> { rootCertRule };

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
        private static bool IsRootCertificateRule(string CheckContent)
        {
            string pattern = "CN=DoD";
            bool isMatch = Regex.IsMatch(CheckContent, pattern);
            if(isMatch)
            {
                string temp = "";
            }

            return (Regex.IsMatch(CheckContent, pattern));
        }
        private static bool IsNxPackageRule(string CheckContent)
        {
            bool isMatch = false;

            string patternYum = @"dpkg -l \w*|dpkg -l \|#\syum\s+list\s+installed\s+";
            string patterNoMatchNegIntegrity = @"^(?!.*(?:Verify the|A) file integrity tool).*";
            string patternNoMatchNegNotInstalled = @"^(?!not installed, this is Not Applicable$).*";
            string patternNoMatchNegInstalledService = @"^(?!If\s+""\w*""\s+is\s+installed,\s+check\s+to\s+see\s+if\s+the\s+""\w*""\s+service\s+is\s+active\s+with\s+the\s+following\s+command).*";

            //$CheckContent - Match 'dpkg -l \w*|dpkg -l \||#\s*yum\s+list\s+installed\s+' - and
            //$CheckContent - NotMatch '(?:Verify the|A) file integrity tool' - and
            //$CheckContent - NotMatch 'not installed, this is Not Applicable' - and
            //$CheckContent - NotMatch 'If "\w*" is installed, check to see if the "\w*" service is active with the following command'

            if (Regex.IsMatch(CheckContent, patternYum) &&
                Regex.IsMatch(CheckContent, patterNoMatchNegIntegrity) &&
                Regex.IsMatch(CheckContent, patternNoMatchNegNotInstalled) &&
                Regex.IsMatch(CheckContent, patternNoMatchNegInstalledService))
                {
                    isMatch = true;
                }
            return isMatch;
        }
        private static bool IsNxFileRule(string CheckContent)
        {
            bool isMatch = false;
            string patternSudo = @"(?:#|\$\s+sudo|#\s+sudo)\s+(?:cat|grep|more).*/.*/.*(?:grep|).*";
            string patternOs = @"Verify\s+the\s+operating\s+system\s+displays\s+the\s+Standard\s+Mandatory\s+DoD\s+Notice\s+and\s+Consent\s+Banner";
            string patternNoMatch = @"^(?!ESXi$).*";

            if (Regex.IsMatch(CheckContent, patternSudo) &&
                Regex.IsMatch(CheckContent, patternOs) &&
                Regex.IsMatch(CheckContent, patternNoMatch))
            {
                isMatch = true;
            }
            //$CheckContent - Match '(?:#|\$\s+sudo||#\s+sudo)\s+(?:cat|grep|more).*/.*/.*(?:grep|).*' - and
            //$CheckContent - Match 'Verify\s+the\s+operating\s+system\s+displays\s+the\s+Standard\s+Mandatory\s+DoD\s+Notice\s+and\s+Consent\s+Banner' - and
            //$CheckContent - NotMatch 'ESXi'
           
            return isMatch;
        }
        private static bool IsNxFileLineRule(string CheckContent)
        {
            bool isMatch = false;

            //string pattern = @"If\s+.*"".*".* commented out.*this is a finding | If\s +.* ""\w * ".*is missing from.*file.*this is a finding";
            string patternUbuntuFinding = "If\\s+.*\".*\".*commented out.*this is a finding|If\\s+.*\"\\w*\".*is missing from.*file.*this is a finding";
            string patternUbuntuAuditCtrl = "\\s*sudo\\s*aud(i)*tctl\\s*-l\\s*";
            string AIImrpovedUbuntuAuditCtrlPattern = @"(?:#|\$\s*)?sudo\s+(?:/usr/bin/)?auditctl\s+-l(?:\s*\|)?";

            if(Regex.IsMatch(CheckContent,patternUbuntuFinding) || 
                Regex.IsMatch(CheckContent,AIImrpovedUbuntuAuditCtrlPattern))
            {
                isMatch = true;
            }
            return isMatch;

            // For Ubuntu
            //# CheckContent match for Ubuntu STIG
            //(
            //    $CheckContent - Match 'If\s+.*".*".*commented out.*this is a finding|If\s+.*"\w*".*is missing from.*file.*this is a finding' - or
            //    $CheckContent - Match '\s*sudo\s*aud(i)*tctl\s*-l\s*\|'
            //) -or
            //# CheckContent match for RHEL STIG
            // (
            //    $CheckContent - Match '(?:#|\$\s+sudo|#\s+sudo)\s+(?:cat|grep|more).*/.*/.*(?:grep|).*' - and
            //   (
            //       $CheckContent - Match 'If\s+.*(?:"\w*"|"\w*\s*\w"|the\s+line\s+is\s+commented\s+out).*,\s+this\s+is\s+a\s+finding' - or
            //       $CheckContent - Match 'If\s+.*required\s+value\s+is\s+not\s+set.*,\s+this\s+is\s+a\s+finding' - or
            //       $CheckContent - Match 'If\s+.*configuration\s+file\s+does\s+not\s+exist\s+or\s+allows\s+for.*,\s+this\s+is\s+a\s+finding' - or
            //       $CheckContent - Match 'If\s+.*command(?:s|)\s+(?:does|do)\s+not\s+return\s+(?:any\s+|a\s+line\s+|)output.*,\s+this\s+is\s+a\s+finding' - or
            //       $CheckContent - Match 'If\s+.*there\s+is\s+no\s+process\s+to\s+validate.*,\s+this\s+is\s+a\s+finding' - or
            //       $CheckContent - Match 'If\s+there\s+is\s+no\s+evidence\s+(?:that\s+|)the\s+.*,\s+this\s+is\s+a\s+finding'
            //   )
            // ) - and
            // $CheckContent - NotMatch 'ESXi' - and
            // $CheckContent - NotMatch '#\s*(?:cat|more)\s+\/etc\/fstab.*'
            // # for Oracle


        }
        private static bool IsNxServiceRule(string CheckContent)
        {

            bool isMatch = false;
            string patternSysCtrl = @"systemctl\s*(is-enabled|is-active|status)";
            string patternStatus = @"If\s+(?:the\s+)?""\w*"".*status.*,\s*this\s*is\s*a\s*finding";
            string patternReturns = @"If\s*the.*command.*returns.*,\s*this\s*is\s*a\s*finding\.";
            string patternActive = @"If\s*"".*""\s*is\s*not\s*active\s*or\s*loaded,\s*this\s*is\s*a\s*finding\.";
            string patternOtherThan = @"If\s*something\s*other\s*than\s*"".*""\s*is\s*returned,\s*this\s*is\s*a\s*finding\.";
            string patternActiveNotDocumented = @"If\s*the\s*service\s*is\s*active\s*and\s*is\s*not\s*documented,\s*this\s*is\s*a\s*finding\.";

            if( Regex.IsMatch(CheckContent,patternSysCtrl) && 
                (Regex.IsMatch(CheckContent, patternStatus) ||
                Regex.IsMatch(CheckContent, patternReturns) ||
                Regex.IsMatch(CheckContent, patternActive) ||
                Regex.IsMatch(CheckContent, patternOtherThan) ||
                Regex.IsMatch(CheckContent, patternActiveNotDocumented))
              )
            {
                isMatch = true;
            }
            /*
           
            $CheckContent - Match 'systemctl\s*(is-enabled|is-active|status)' - and
            (
                $CheckContent - Match 'If\s+(?:|the\s+)"\w*".*status.*,\s*this\s*is\s*a\s*finding' - or
                $CheckContent - Match 'If\s*the.*command.*returns.*,\s*this\s*is\s*a\s*finding.' - or
                $CheckContent - Match 'If\s*".*"\s*is\s*not\s*active\s*or\s*loaded,\s*this\s*is\s*a\s*finding.' - or
                $CheckContent - Match 'If\s*something\s*other\s*than\s*".*"\s*is\s*returned,\s*this\s*is\s*a\s*finding.' - or
                $CheckContent - Match 'If\s*the\s*service\s*is\s*active\s*and\s*is\s*not\s*documented,\s*this\s*is\s*a\s*finding.'
            )
            */


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
