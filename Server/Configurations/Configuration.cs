using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Server.Configurations
{
    /// <summary>
    /// Base configuration class
    /// </summary>
    public abstract class Configuration
    {
        /// <summary>
        /// Helper method for configuration classes to navigate their settings.
        /// </summary>
        /// <param name="section">name of configuration section</param>
        /// <param name="key">name of configuration item</param>
        /// <returns>value of configuration item</returns>
        protected static string Setting(string section, string key)
        {
            string settingValue = null;
            System.Collections.IDictionary settings = 
                (System.Collections.IDictionary)ConfigurationManager.GetSection(section);
            if (settings.Contains(key)) { settingValue = settings[key].ToString(); }
            return settingValue;
        }
    }
}