using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class RootCertificateVRule : VRule
    {
        private string thumbPrint;

        public string ThumbPrint
        {
            get { return thumbPrint; }
            set { thumbPrint = value; }
        }

        public static bool IsMutliCertContent(string CheckContent)
        {
            bool isMultiline = false;
            Regex regex = new Regex(@"(?<=Thumbprint:\s)[A-F0-9]+", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(CheckContent);

            HashSet<string> uniqueThumbprints = new HashSet<string>();

            foreach (Match match in matches)
            {
                uniqueThumbprints.Add(match.Value);
            }
            if(uniqueThumbprints.Count > 0 )
            {
                isMultiline = true;
            }
            return isMultiline;
        }

        public static List<string> SplitRootCertificateRuleContent(string CheckContent)
        {
            List<string> splits = new List<string>();

            return splits;
        }

        public static RootCertificateVRule PopulateRootCertificateVRule(RootCertificateVRule RootCertificateVRule)
        {
            // arg has the CheckContent that contains the additional data for this rule type
            RootCertificateVRule.ThumbPrint = GetThumbprint(RootCertificateVRule.CheckContent);
            

            return RootCertificateVRule;
        }

        public static string GetThumbprint(string CheckContent)
        {
            string thumbprint = "";

            return thumbprint;
        }
    }
}
