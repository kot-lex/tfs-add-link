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
        static void AddLink(Options options)
        {

            Console.OutputEncoding = System.Text.Encoding.Default;

            Console.WriteLine("");

            NetworkCredential credentials = new NetworkCredential(options.Username, options.Password);
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(options.TFSAddress), credentials);

            tfs.EnsureAuthenticated();
            WorkItemStore workitemstore = tfs.GetService<WorkItemStore>();
            Int32 workitemId = options.WorkItemID;
            WorkItem workItem = workitemstore.GetWorkItem(workitemId);

            Hyperlink link = new Hyperlink(options.Link);

            try
            {
                workItem.Links.Add(link);
                workItem.Save();
                Console.WriteLine("Link saved");
            }
            catch (ValidationException exception)
            {
                Console.WriteLine("The work item threw a validation exception.");
                Console.WriteLine(exception.Message);
            }
        }
    }
}
