using System;
using System.Collections.Generic;
using System.Configuration;

namespace Server.Configurations
{
    /// <summary>
    /// Server configurations
    /// </summary>
    public class Server : Configuration
    {
        public static string Version { get { return Setting("version"); } }
        public static string Name { get { return Setting("name"); } }
        public static string Host { get { return Setting("host"); } }
        public static int Port { get { return Convert.ToInt32(Setting("port")); } }
        public static string Setting(string key) { return Configuration.Setting("serverSettings", key); }
    }
}