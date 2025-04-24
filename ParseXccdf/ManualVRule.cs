using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class ManualVRule : VRule
    {
        public static ManualVRule Clone(VRule Rule)
        {
            ManualVRule newVRule = new ManualVRule();
            newVRule.GroupId = Rule.GroupId;
            foreach (string id in Rule.Identifiers)
            {
                newVRule.Identifiers.Add(id);
            }
            foreach (string str in Rule.IsAFinding)
            {
                newVRule.IsAFinding.Add(str);
            }
            foreach (string str in Rule.IsNotAFinding)
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
            newVRule.RuleType = "ManualRule";
            newVRule.DscResource = "None";
            newVRule.Ensure = Rule.Ensure;
            return newVRule;
        }

        public static bool IsMultiline(string CheckContent)
        {
            return true;
        }
    }
}
