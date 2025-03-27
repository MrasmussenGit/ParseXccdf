using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            set { ruleId = value; }
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

    }
}
