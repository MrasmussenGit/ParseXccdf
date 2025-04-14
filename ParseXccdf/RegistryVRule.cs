using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class RegistryVRule : VRule
    {
        private string valueData;
        private string valueName;
        private string valueType;
        private string key;
        private string ensure;
        private bool isNullOrEmpty;

        public string ValueData
        {
            get { return valueData; }
            set { valueData = value; }
        }

        public string ValueName
        {
            get { return valueName; }
            set { valueName = value; }
        }

        public string ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
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
        public static RegistryVRule Clone(VRule Rule)
        {
            RegistryVRule newVRule = new RegistryVRule();
            newVRule.GroupId = Rule.GroupId;
            foreach(string id in Rule.Identifiers)
            {
                newVRule.Identifiers.Add(id);
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
        public static bool IsMultilineRegEntry(string CheckContent)
        {
            bool isMultiline = false;
            string pattern = @"HKEY_(LOCAL_MACHINE|CURRENT_USER|CLASSES_ROOT|USERS|CURRENT_CONFIG)\\[\w\\]+";
            Regex regex = new Regex(pattern);
            MatchCollection mc = regex.Matches(CheckContent);
            
            if (mc.Count > 1)
            {
                isMultiline = true;
                foreach(Match m in mc)
                {
                    string temp = "";
                }
            }
            return isMultiline; 
        }
    }
}
