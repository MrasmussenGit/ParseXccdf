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
        private string registryValueName;
        private string registryType;
        private string registryName;
        private string registryValueData;


        public RegistryData()
        {
            this.registryKey = "";
            this.registryValueName = "";
            this.registryType = "";
            this.registryName = "";
            this.registryValueData = "";
        }

        public string RegistryKey
        {
            get { return registryKey; }
            set { registryKey = value; }
        }

        public string RegistryValueName
        {
            get { return registryValueName; }
            set { registryValueName = value; }
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
        public string RegistryValueData
        {
            get { return registryValueData; }
            set { registryValueData = value; }
        }
    }
}
