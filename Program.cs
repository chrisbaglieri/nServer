using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine("Preparing nServer...");

        // prepare primary endpoint URL
        UriBuilder uriBuilder = new UriBuilder("http",
            Server.Configurations.Server.Host,
            Server.Configurations.Server.Port);
        Console.WriteLine(String.Format("Setting up to listen for commands on {0}...", uriBuilder.Uri.ToString()));

        // register controllers
        Console.WriteLine("Registering all controllers...");
        Server.Controllers.ControllerManager.RegisterAllControllers();

        // start it up
        nServer server = new nServer(uriBuilder.Uri);
        nServer.CurrentInstance = server;
        server.IncomingRequest += RequestHandler.HandleIncomingRequest;
        server.Start();

        // ping to test it's alive and ready
        Console.WriteLine("Pinging to confirm success...");
        uriBuilder.Path = "server/ping";
        WebRequest request = WebRequest.Create(uriBuilder.Uri);
        WebResponse response = request.GetResponse();
        response.Close();
        
        Console.WriteLine("nServer ready to accept requests...");
        Console.WriteLine("---------------------------------------------------------");
    }
}