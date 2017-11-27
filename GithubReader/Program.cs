using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string password = Console.ReadLine();

            var client = new GitHubClient(new ProductHeaderValue("GithubSocialGraph"));
            var basicAuth = new Credentials(username, password);
            client.Credentials = basicAuth;
            
            PrintUser("macdarat", client).Wait();
            Console.ReadKey();
        }

        /** <summary> Reads a user's password without having it echoed to console </summary> */
        private static string ReadPassword()
        {
            string result = "";
            char currentChar = 'a';
            while (currentChar != '\r')
            {
                currentChar = Console.ReadKey(true).KeyChar;
                result += currentChar;
            }
            return result;
        }

        /** <summary> Print details about a given user on Github </summary> */
        async static Task PrintUser(string uname, GitHubClient github)
        {
            Console.WriteLine("Print user");
            var user = await github.User.Get(uname);
            Console.WriteLine(uname + "'s bio:" + user.Bio + uname + "'s total public repos:" + user.PublicRepos);

            var repos = await github.Repository.GetAllForUser(uname);
            foreach (Repository r in repos)
            {
                Console.WriteLine("Repository {0}, {1}, language {2}, last updated {3}, created at {4}", 
                    r.FullName, r.Description, r.Language, r.UpdatedAt, r.CreatedAt);
            }
            ApiInfo apiInfo = github.GetLastApiInfo();

            var rateLimit = apiInfo?.RateLimit;
            var reqPerHour = rateLimit?.Limit;
            var reqRemaining = rateLimit?.Remaining;
            Console.WriteLine(reqPerHour + " reqs per hour, remaining reqs:" + reqRemaining);
        }
    }
}
