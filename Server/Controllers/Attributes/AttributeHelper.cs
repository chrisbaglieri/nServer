using Server.Controllers;
using Server.Controllers.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Controllers.Attributes
{
    /// <summary>
    /// Helps work with custom server attributes.
    /// </summary>
    internal static class AttributeHelper
    {
        /// <summary>
        /// Fetchs a particular type of attribute on a given controller's action.
        /// </summary>
        /// <typeparam name="T">type of attribute</typeparam>
        /// <param name="controller">controller of interest</param>
        /// <param name="action">action of interest</param>
        /// <returns></returns>
        internal static List<T> FetchAttributes<T>(MethodInfo actionImplementation) where T : CustomAttribute
        {
            List<T> attributes = new List<T>();

            // grab the set of custom attributes of a certain type
            T[] customAttributes = (T[])actionImplementation.GetCustomAttributes(typeof(T), true);

            // return the found attributes or the default behavior of the attribute if none
            if (customAttributes.Length > 0) {
                attributes.AddRange(customAttributes);
            }

            return attributes;
        }
    }
}