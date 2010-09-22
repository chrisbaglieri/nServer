using System;
using System.Collections.Generic;
using System.Configuration;

namespace Server.Configurations
{
    /// <summary>
    /// Custom server configurations
    /// </summary>
    public class Custom : Configuration
    {
        /// <summary>
        /// Returns the requested custom configuration setting
        /// </summary>
        /// <param name="key">name of the configuration setting</param>
        /// <returns>value of the configuration setting</returns>
        public static string Setting(string key) 
        { 
            return Configuration.Setting("customSettings", key); 
        }
    }
}