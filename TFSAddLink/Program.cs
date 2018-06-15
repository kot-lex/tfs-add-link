using CommandLine;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Net;

namespace TFSAddLink
{
    class Program
    {
        class Options
        {
            [Option('t', "tfs", Required = true, HelpText = "TFS Server address")]
            public string TFSAddress { get; set; }

            [Option('u', "username", Required = true, HelpText = "Username")]
            public string Username { get; set; }

            [Option('p', "password", Required = true, HelpText = "Password")]
            public string Password { get; set; }

            [Option('i', "item", Required = true, HelpText = "Work item id")]
            public Int32 WorkItemID { get; set; }

            [Option('l', "link", Required = true, HelpText = "Link to add")]
            public string Link { get; set; }
        }

        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => AddLink(options));
        }
        static TfsTeamProjectCollection ConnectToTFS(string address, string username, string password)
        {
            NetworkCredential credentials = new NetworkCredential(username, password);
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(address), credentials);
            tfs.EnsureAuthenticated();
            return tfs;
        }
        static void AddLink(Options options)
        {

            try
            {
                var tfs = ConnectToTFS(options.TFSAddress, options.Username, options.Password);
                WorkItemStore workitemstore = tfs.GetService<WorkItemStore>();

                WorkItem workItem = workitemstore.GetWorkItem(options.WorkItemID);

                Hyperlink link = new Hyperlink(options.Link);

                workItem.Links.Add(link);
                workItem.Save();
                Console.WriteLine("Link saved");
            }
            catch (Exception exception)
            {
                Console.WriteLine("The work item threw a validation exception.");
                Console.WriteLine(exception.Message);
            }
        }
    }
}
