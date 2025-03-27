using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class PostProcessedVRule : VRule
    {
        private string duplicateOf;
        private string legacyId;
        private bool organizationalValueRequired;
        private string rawString;
        private string vulnDiscussion;
        private string dscResource;
        private string isNullorEmpty;
        private string filePath;

        public string DuplicateOf
        {
            get { return duplicateOf; }
            set { duplicateOf = value; }
        }

        public string LegacyId
        {
            get { return legacyId; }
            set { legacyId = value; }
        }

        public bool OrganizationalValueRequired
        {
            get { return organizationalValueRequired; }
            set { organizationalValueRequired = value; }
        }

        public string RawString
        {
            get { return rawString; }
            set { rawString = value; }
        }

        public string VulnDiscussion
        {
            get { return vulnDiscussion; }
            set { vulnDiscussion = value; }
        }

        public string DscResource
        {
            get { return dscResource; }
            set { dscResource = value; }
        }

        public string IsNullOrEmpty
        {
            get { return isNullorEmpty; }
            set { isNullorEmpty = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public static string[] CompareRuleToPostRuleLists(List<Rule> PreProcessedRules, List<PostProcessedVRule> PostProcessedRules)
        {
            StringBuilder sb = new StringBuilder();
            bool returnMatch = true;
            string currentRuleId = "";
            bool match = false;
            string filePath = "";
            sb.AppendLine("Searching for rules in the XCCDF not located in the post-processed XML");
            foreach (Rule preRule in PreProcessedRules)
            {
                foreach(PostProcessedVRule postRule in PostProcessedRules)
                {
                    // criteria that makes them equal
                    currentRuleId = postRule.RuleId;
                    filePath = postRule.FilePath;
                    if (preRule.Rules[0].GroupId == postRule.RuleId)
                    {
                        match = true; break;
                    }
                }
                if(!match)
                {
                    sb.AppendLine($"{preRule.Rules[0].GroupId} MISSING in {filePath}");
                    returnMatch = false;
                }
                else
                {
                    sb.AppendLine($"XCCDF rule:{preRule.Rules[0].GroupId} FOUND IN {filePath}");
                }
                match = false;
            }
            sb.AppendLine("Search completed");
            sb.AppendLine("Searching for rules in the post-processed XML that are not in the XCCDF file");
            foreach(PostProcessedVRule postRule in PostProcessedRules)
            {
                foreach(Rule preRule in PreProcessedRules)
                {
                    // criteria that makes them equal
                    currentRuleId = preRule.Rules[0].GroupId;
                    filePath = preRule.FilePath;
                    if (postRule.RuleId == preRule.Rules[0].GroupId)
                    {
                        match = true; break;
                    }
                }
                if (!match)
                {
                    sb.AppendLine($"{postRule.RuleId} not found in {filePath}");
                    returnMatch = false;
                }
                else
                {
                    sb.AppendLine($"Post process XML rule:{postRule.RuleId} FOUND IN {filePath}");
                }
                match = false;
            }
            sb.AppendLine("Search completed");
            string[] returnArray = new string[2];
            returnArray[0] = sb.ToString();
            returnArray[1] = returnMatch.ToString();
            return returnArray;
        }

        public static bool IsEqual(Rule Rule, PostProcessedVRule postProcessedVRule)
        {
            bool match = false;

            if (Rule.Rules[0].GroupId == postProcessedVRule.RuleId)
            {
                match = true;
            }

            return match;
        }
    }
}
