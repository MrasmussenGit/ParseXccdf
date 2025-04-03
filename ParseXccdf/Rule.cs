using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class Rule
    {
        private string title;
        private string description;
        private string filePath;
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

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public List<VRule> Rules
        {
            get { return rules; }
            set {  rules = value; }
        }

    }
}
