using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParseXccdf
{
    internal class Stig
    {
        private string[] VRules;
        private string FileNameAndPath;
        private string version;
        private string company;
        private string product;
        public Stig() { }

        public string[] V_Rules
        {
            set { VRules = value; }
            get { return VRules; }
        }

        public string FilePath
        {
            set {  FileNameAndPath = value; }
            get { return FileNameAndPath; }
        }

        public string StigVersion
        {
            set { version = value; }
            get { return version; }
        }

        public string Company
        {
            set { company = value; }
            get { return company; }
        }

        public string Product
        {
            set { product = value; }
            get { return product; }
        }

        public override bool Equals(Object Stig)
        {
            bool overallMatch = false;
            bool match = false;
            Stig RealStig = (Stig)Stig;
            if (this.Company.ToLower().Equals(RealStig.Company) && this.Product.ToLower().Equals(RealStig.Product) && this.StigVersion.ToLower().Equals(RealStig.StigVersion)) 
            { 
                foreach(string stigRule in RealStig.V_Rules)
                {
                    foreach(string currentStigRule in this.V_Rules)
                    {
                        if(currentStigRule.Equals(stigRule))
                        {
                            match = true;
                            break;
                        }
                    }
                    if(!match)
                    {
                        Console.WriteLine($"Rule: {stigRule} not found in {this.FilePath}");
                        overallMatch = false;
                    }
                }
                
            }

            return overallMatch;
        }


    }
}
