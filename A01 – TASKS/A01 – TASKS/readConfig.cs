using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// FILE               : readConfig.cs
// PROJECT            : A01 - TASKS
// PROGRAMMER		  : Josiah Williams, Ricardo Gao, Jeff David Tieng
// FIRST VERSION      : 2025-01-28
// DESCRIPTION        : this just seprate the class for read from the app.config
//
namespace A01___TASKS
{
    internal class readConfig
    {

        public int messageLength;
        public string ipAddress;
        public string port;
        public string sizeDoc;
        public int cilentNumber;
        /// <summary>
        /// this is the method for read argument from the app config and write into the varible 
        /// </summary>
        public void reConfig()
        {
            messageLength = int.Parse(ConfigurationManager.AppSettings["MessageLength"] ?? "12");
            ipAddress = ConfigurationManager.AppSettings["Ipaddress"] ?? "127.0.0.1";
            port = ConfigurationManager.AppSettings["Port"] ?? "14000";
            sizeDoc = ConfigurationManager.AppSettings["size"] ?? "0";
            string cilentNumberString = ConfigurationManager.AppSettings["ClientCount"] ?? "50";
            cilentNumber = int.Parse(cilentNumberString);
        }
    }
}
