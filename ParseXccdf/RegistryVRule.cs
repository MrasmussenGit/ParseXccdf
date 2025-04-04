using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
