using Server.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Controllers
{
    public static class ControllerManager
    {
        // internal collection of available controllers <controller name, controller type>
        private static Dictionary<string, Type> controllers = new Dictionary<string, Type>();

        /// <summary>
        /// Registers all controllers assuming the 'Application.Controllers' namespace 
        /// and ApplicationController base class.  All others classes are ignorated.
        /// </summary>
        public static void RegisterAllControllers()
        {
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes()) {
                if (type.IsSubclassOf(typeof(ApplicationController)))
                    RegisterController(type);
            }
        }

        /// <summary>
        /// Registers a particular controller
        /// </summary>
        /// <param name="controllerType">type of controller to register</param>
        public static void RegisterController(Type controllerType)
        {
            controllers.Add(controllerType.Name.ToLower(), controllerType);
        }

        /// <summary>
        /// Gets an instance of a particular controller
        /// </summary>
        /// <param name="controllerName">name of the controller of interest</param>
        /// <returns></returns>
        public static ApplicationController GetControllerInstance(string controllerName)
        {
            ApplicationController controller = null;
            if (controllers.ContainsKey(controllerName.ToLower())) {
                controller = (ApplicationController)Activator.CreateInstance(controllers[controllerName.ToLower()]);
            }
            return controller;
        }

        /// <summary>
        /// Gets the instance of a particular action on a particular controller
        /// </summary>
        /// <param name="controller">controller of interest</param>
        /// <param name="actionName">action of interest</param>
        /// <returns></returns>
        public static MethodInfo GetControllerAction(ApplicationController controller, string actionName)
        {
            string cleansedActionName = actionName.ToLower();
            MethodInfo actionImplementation = null;
            foreach (MethodInfo methodInfo in controller.GetType().GetMethods()) {
                if (methodInfo.Name.ToLower().Equals(cleansedActionName)) {
                    actionImplementation = methodInfo;
                }
            }
            return actionImplementation;
        }
    }
}