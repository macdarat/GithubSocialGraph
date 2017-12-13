using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.IO;

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

            FetchRepoDetails(client).Wait();
            //PrintUser(client).Wait();
            Console.ReadKey();
        }

        /** <summary> Fetches details about top repositories for a number of languages    </summary>         */
        async static Task FetchRepoDetails(GitHubClient github)
        {
            Console.WriteLine("Getting repo details\n");
            var searchReq = new SearchRepositoriesRequest
            {
                Stars = Range.GreaterThan(250),
                SortField = RepoSearchSort.Stars,
                Order = SortDirection.Descending,
                PerPage = 50
            };

            string filePath = "..\\..\\..\\repositories.csv"; //places file 3 directories up, in proper place
            File.Create(filePath);      //create file to store results in
            string averageFP = "..\\..\\..\\repoAverages.csv";
            File.Create(averageFP);    //file for the mean values

            searchReq.Language = Language.CSharp;
            var cSharpRepos = await github.Search.SearchRepo(searchReq);
            string line = "language,OpenIssuesCount,ForksCount,Size\n";     //add first row of csv
            File.AppendAllText(filePath, line);
            line = "language,OpenIssuesAverage,ForksAverage,SizeAverage\n";     //add first row of csv
            File.AppendAllText(averageFP, line);

            OutputRepoDetails(cSharpRepos, "C#", filePath, averageFP);
            
            searchReq.Language = Language.C;
            var cRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(cRepos, "C", filePath, averageFP);

            searchReq.Language = Language.Java;
            var javaRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(javaRepos, "Java", filePath, averageFP);

            searchReq.Language = Language.Ruby;
            var rubyRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(rubyRepos, "Ruby", filePath, averageFP);

            searchReq.Language = Language.JavaScript;
            var javascriptRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(javascriptRepos, "JavaScript", filePath, averageFP);

            searchReq.Language = Language.Haskell;
            var haskellRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(haskellRepos, "Haskell", filePath, averageFP);

            searchReq.Language = Language.Python;
            var pythonRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(javaRepos, "Python", filePath, averageFP);

            searchReq.Language = Language.Assembly;
            var asmRepos = await github.Search.SearchRepo(searchReq);
            OutputRepoDetails(asmRepos, "Assembly", filePath, averageFP);

            PrintReqsRemaining(github);
        }

        /** <summary> Outputs repos details to console, then appends relevant info for each repo into a csv file </summary>*/
        public static void OutputRepoDetails(SearchRepositoryResult sResult, string language, string filePath, string averageFP)
        {
            if (sResult.IncompleteResults)
            {
                Console.WriteLine("INCOMPLETE RESULTS FOR {0}", language);
            }
            var languageResults = sResult.Items;

            int openIssuesAv = 0;
            int ForksCountAv = 0;
            long sizeAv = 0;
            int count = 1;
            string line;

            Console.WriteLine("\n{0} items for {1}\n", languageResults.Count, language);
            foreach (Repository r in languageResults)
            {
                //build csv file of repo values while outputting names to console
                Console.WriteLine(r.FullName);
                line = language + "," + r.OpenIssuesCount + "," + r.ForksCount + "," + r.Size + "\n";
                File.AppendAllText(filePath, line);

                //calculate averages as we go along
                openIssuesAv = (r.OpenIssuesCount + (openIssuesAv * count-1)) / count;
                ForksCountAv = (r.ForksCount + (ForksCountAv * count - 1)) / count;
                sizeAv = (r.Size + (sizeAv * count - 1)) / count;

                count++;
            }

            line = language + "," + openIssuesAv + "," + ForksCountAv + "," + sizeAv + "\n";
            File.AppendAllText(averageFP, line);

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

        /** <summary> Print details about a given user on Github </summary> */
        async static Task PrintUser(GitHubClient github)
        {
            Console.WriteLine("Enter username to view");
            string uname = Console.ReadLine();

            Console.WriteLine("Print user");
            var user = await github.User.Get(uname);
            Console.WriteLine(uname + "'s bio:" + user.Bio + uname + "'s total public repos:" + user.PublicRepos);

            var repos = await github.Repository.GetAllForUser(uname);
            foreach (Octokit.Repository r in repos)
            {
                Console.WriteLine("Repository {0}, {1}, language {2}, last updated {3}, created at {4}", 
                    r.FullName, r.Description, r.Language, r.UpdatedAt, r.CreatedAt);
            }
            PrintReqsRemaining(github);          
        }

        public static void PrintReqsRemaining(GitHubClient github)
        {
            ApiInfo apiInfo = github.GetLastApiInfo();
            var rateLimit = apiInfo?.RateLimit;
            var reqPerHour = rateLimit?.Limit;
            var reqRemaining = rateLimit?.Remaining;
            Console.WriteLine("-------\n" + reqPerHour + " reqs per hour, remaining reqs:" + reqRemaining);
        }
    }
}
