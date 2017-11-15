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
            Console.WriteLine("test");
            var github = new GitHubClient(new ProductHeaderValue("GithubReader"));
        }
    }
}
