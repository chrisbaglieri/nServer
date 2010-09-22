using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Controllers
{
    /// <summary>
    /// Base server controller.  Primarily used when running the server as-is
    /// (e.g. as an executable) versus wrapping it to cater to a given client's
    /// needs (e.g. as a DLL).
    /// </summary>
    class ServerController : ApplicationController
    {
    }
}