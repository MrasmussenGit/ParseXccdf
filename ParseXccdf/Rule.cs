using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class Rule
    {
        private string title;
        private string description;
        List<VRule> rules = new List<VRule>();

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public List<VRule> Rules
        {
            get { return rules; }
            set {  rules = value; }
        }

    }
}
