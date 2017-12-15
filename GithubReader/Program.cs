using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Octokit;

namespace GithubReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter github username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter github password:");
            string password = ReadPassword();

            var client = new GitHubClient(new ProductHeaderValue("GithubSocialGraph"));
            var basicAuth = new Credentials(username, password);
            client.Credentials = basicAuth;

            Reader githubReader = new Reader(client);

            try
            {
                githubReader.FetchRepoDetails(client).Wait();
            }
            catch (System.AggregateException)
            {
                Console.WriteLine("An error has occurred, Terminating");
            }
            //PrintUser(client).Wait();
            Console.ReadKey();
        }

        /** <summary> Reads a user's password without having it echoed to console </summary> */
        private static string ReadPassword()
        {
            string result = "";
            char currentChar = Console.ReadKey(true).KeyChar;
            while (currentChar != '\r')
            {
                result += currentChar;
                currentChar = Console.ReadKey(true).KeyChar;
            }

            return result;
        }
    }
}
