using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        private string fixId;
        private string checkSystem;
        private string checkContentRefHref;
        private string checkContent;
        private string trimmedRuleId;
        private string ruleType;

        #region Properties
        public VRule()
        {
            identifiers = new List<string>();
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
            set { fixText = value; }
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
        #endregion 

        public static string TrimPostProcessRuleId(string RuleId)
        {
            return RuleId.Split('.')[0];

        }

        public static string DetermineRuleType(string CheckContent)
        {
            string ruleType = "";
            // original code has trim extra lines from content



            if (IsRegistryRule(CheckContent))
            {
                ruleType = "SomeSpecificRegTerm";
            }
            else if (IsHardCodedRule(CheckContent))
            {
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
            else if(IsDnsServerRootHintRule(CheckContent))
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
            else if(IsMimeTypeRule(CheckContent))
            {

            }
            else if(IsPermissionRule(CheckContent))
            {

            }
            else if(IsProcessMitigationRule(CheckContent))
            {

            }



                return ruleType;
        }

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
        

    }
}
