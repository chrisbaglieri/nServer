using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Server.Controllers.Attributes
{
    /// <summary>
    /// Confirms the request contains the expected parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ConfirmParametersAttribute : CustomAttribute
    {
        public readonly List<string> ParametersNames = new List<string>();

        /// <summary>
        /// Default attribute constructor.  Assumes no required tokens.
        /// </summary>
        public ConfirmParametersAttribute() { }

        /// <summary>
        /// Constructs a confirm tokens attribute with a seed list of token names
        /// </summary>
        /// <param name="requiredTokenNames">comma delimited list of required token names</param>
        public ConfirmParametersAttribute(string requiredTokenNames)
        {
            this.ParametersNames.AddRange(requiredTokenNames.Split(','));
        }

        /// <summary>
        /// Applies the confirm parameters attributes
        /// </summary>
        /// <param name="controller">controller to apply the attribute against</param>
        /// <param name="actionImplementation">action to apply the attribute against</param>
        /// <param name="message">outbound message</param>
        /// <returns>whether or not the attribute was successfully applied</returns>
        public override bool Apply(ApplicationController controller, MethodInfo actionImplementation, out string message)
        {
            message = null;
            bool parametersAccountedFor = false;
            bool parameterFound = false;
            List<string> missingParameters = new List<string>();

            // iterate over the parameters
            foreach (string requiredParameter in this.ParametersNames) {
                parameterFound = false;
                foreach (string parameter in controller.Parameters) {
                    if (parameter.ToUpper().Equals(requiredParameter.ToUpper())) {
                        parameterFound = true;
                        break;
                    }
                }
                if (!parameterFound) {
                    missingParameters.Add(requiredParameter);
                }
            }

            // determine the outcome based on what was found
            if (missingParameters.Count == 0 ) {
                parametersAccountedFor = true;
            } else {
                message = String.Format("The following required parameters were missing from the request: {0}",
                  String.Join(",", missingParameters.ToArray()));
            }

            return parametersAccountedFor;
        }
    }
}