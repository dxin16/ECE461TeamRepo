﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECE461ProjectPart1
{
    /// <summary>
    /// Handles loading & converting github urls from a file containing github urls and npmjs urls
    /// </summary>
    public static class GithubURLRetriever
    {
        /// <summary>
        /// Returns a list of Github URLs when given a text file containing github and npmjs urls
        /// </summary>
        /// <param name="filePath">path to text file</param>
        /// <returns>A list of Github URLs</returns>
        public static List<string> GetURLList(string filePath)
        {
            //Load File
            string[] urlArray = File.ReadAllLines(@filePath);
            Task<List<string>> asyncCall = GetGithubURLListAsync(urlArray);
            asyncCall.Wait();

            return asyncCall.Result;
        }

        //Returns a list of Github URLs
        static async Task<List<string>> GetGithubURLListAsync(string[] urlArray)
        {
            List<string> githubURLs = new List<string>();
            List<string> npmjsURLs = new List<string>();

            foreach (string url in urlArray)
            {
                if (url.Contains("github.com"))
                {
                    githubURLs.Add(url);
                }
                else if (url.Contains("npmjs.com"))
                {
                    npmjsURLs.Add(url);
                }
                else Console.WriteLine("Error, invalid URL");
            }

            githubURLs.AddRange(await ConvertNpmjsToGithubUrlAsync(npmjsURLs));

            return githubURLs;
        }

        //Returns a List of Github URLs
        static async Task<List<string>> ConvertNpmjsToGithubUrlAsync(List<string> npmjsURLs)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            List<string> githubUrlList = new List<string>();
            foreach (string url in npmjsURLs)
            {
                string webPage = await client.GetStringAsync(url);

                //remove all text before the github url
                string[] webPageParsed = webPage.Split("\"repository-link\">", StringSplitOptions.RemoveEmptyEntries);

                //remove all text after the github url
                webPageParsed = webPageParsed[1].Split('<', StringSplitOptions.RemoveEmptyEntries);

                if (url.Contains("github.com"))
                {
                    githubUrlList.Add("https://" + webPageParsed[0]);
                }
                else Console.WriteLine("Error, github url not found on: " + url);
            }

            return githubUrlList;
        }
    }
}
