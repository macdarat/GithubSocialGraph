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
            Console.WriteLine("Print user");
            PrintUser("macdarat");
            Console.ReadLine();
        }

        /**
         * Print details about a given user on Github
         */
        async static void PrintUser(string uname)
        {
            var github = new GitHubClient(new ProductHeaderValue("GithubReader"));
            var user = await github.User.Get(uname);
            Console.WriteLine(uname + "'s bio:" + user.Bio + uname + "'s total public repos:" + user.PublicRepos);
            ApiInfo apiInfo = github.GetLastApiInfo();

            var repos = await github.Repository.GetAllForUser(uname);
            foreach (Repository r in repos)
            {
                Console.WriteLine("Repository {0}, {1}, language {2}, last updated {3}, created at {4}", 
                    r.FullName, r.Description, r.Language, r.UpdatedAt, r.CreatedAt);
            }

            var rateLimit = apiInfo?.RateLimit;
            var reqPerHour = rateLimit?.Limit;
            var reqRemaining = rateLimit?.Remaining;
            Console.WriteLine(reqPerHour + " reqs per hour, remaining reqs:" + reqRemaining);
        }
    }
}
