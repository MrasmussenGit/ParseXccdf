using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class RegistryData
    {
        private string registryKey;
        private string registryValue;
        private string registryType;
        private string registryName;

        public string RegistryKey
        {
            get { return registryKey; }
            set { registryKey = value; }
        }

        public string RegistryValue
        {
            get { return registryValue; }
            set { registryValue = value; }
        }

        public string RegistryType
        {
            get { return registryType; }
            set { registryType = value; }
        }

        public string RegistryName
        {
            get { return registryName; }
            set { registryName = value; }
        }
    }
}
