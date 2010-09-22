using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Server.Controllers.Attributes
{
    /// <summary>
    /// Base custom attribute.
    /// </summary>
    public abstract class CustomAttribute : System.Attribute 
    {
        public virtual bool Apply(ApplicationController controller, MethodInfo actionImplementation, out string message)
        {
            message = null;
            return true;
        }
    }
}