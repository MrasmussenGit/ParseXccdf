using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class nxServiceRule : VRule
    {
        private string enabled;
        private string name;
        private string state;

        public string Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public static string getServiceName(string FixText)
        {
            string pattern = @"(sudo)?\\s*systemctl\\s+\\w*\\s*(?<serviceName>(\\w*(\\.?))+)";
            string serviceName = "";

            return serviceName;
        }

    }
}
