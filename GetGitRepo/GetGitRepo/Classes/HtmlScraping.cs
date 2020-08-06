using GetGitRepo.Models;
using GetGitRepo.Utilits;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GetGitRepo.Classes
{
    public static class HtmlScraping
    {
        private static string URL_Base = "https://github.com/";

        public static async Task<IEnumerable<Files>> GetHtmlAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                HttpContent content = response.Content;
                HtmlDocument document = new HtmlDocument();
                //var htmlContext = await content.ReadAsStringAsync(); 
                document.LoadHtml(await content.ReadAsStringAsync());

                var itensBox = document.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("aria-labelledby", "").Equals("files")).ToList();

                var fileList = new List<Files>();

                if (itensBox.Count() > 0)
                {
                    var itensRow = itensBox[0].Descendants("div").Where(node => node.GetAttributeValue("role", "").Equals("row")).ToList();

                    foreach (var item in itensRow)
                    {
                        //Verifying object type (Directory or File)
                        if (item.Descendants("svg").Where(node => node.GetAttributeValue("aria-label", "").Equals("Directory")).Count() > 0)
                        {
                            //Is directory
                            var itensDir = item.Descendants("div").Where(no => no.GetAttributeValue("role", "").Equals("rowheader")).ToList();
                        }

                        if (item.Descendants("svg").Where(node => node.GetAttributeValue("aria-label", "").Equals("File")).Count() > 0)
                        {
                            //Is file
                            var itensFile = item.Descendants("div").Where(no => no.GetAttributeValue("role", "").Equals("rowheader")).FirstOrDefault();
                            var fileURL = itensFile.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("js-navigation-open link-gray-dark")).FirstOrDefault().GetAttributeValue("href", "");
                            var fileInfo = await getFileInfoAsync(URL_Base + fileURL);

                            fileList.Add(fileInfo);

                        }
                    }
                }

                if (fileList.Count > 0)
                {
                    //var fileListGrouped = fileList.GroupBy(x => x.FileExtension)
                    //    .Select(g => new { Key = g.Key, TotalSize = g.Key.S});

                    //var listFiles = new List<Files>(); 

                    //foreach (var group in fileListGrouped)
                    //{
                    //    foreach (var item in group)
                    //    {
                    //        listFiles.Add(new Files { 
                    //            FileExtension = item.FileExtension
                    //        }); 
                    //    }
                    //}

                }

                return fileList;

            }
        }


        private static async Task<Files> getFileInfoAsync(string fileURL)
        {
            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(fileURL, HttpCompletionOption.ResponseContentRead);
            HttpContent content = response.Content;
            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(await content.ReadAsStringAsync());

            var file = new Files();

            var nomeArquivo = document.DocumentNode.Descendants("strong").Where(node => node.GetAttributeValue("class", "").Equals("final-path")).FirstOrDefault();
            var fileData = document.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0")).FirstOrDefault();

            var nodesInfo = document.DocumentNode.SelectNodes("//div[@class = 'text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0']");
            var fileLines = 0;
            decimal fileSize = 0;

            foreach (var item in nodesInfo)
            {
                var text = item.InnerText.Replace("\n", "").Replace("  ", "").Replace("(", "").Replace(")", "").Replace("sloc", "").Split(" ");

                if (item.InnerText.Contains("lines"))
                {
                    fileLines = Convert.ToInt32(text[0]);
                }

                if (item.InnerText.Contains("Bytes") || item.InnerText.Contains("KB"))
                {
                    fileSize = Convert.ToDecimal(text[3]);
                }
            }


            file.FileName = nomeArquivo.InnerText.Replace("\n", "").Replace("  ", "");
            file.FileExtension = Utils.getFileExtension(file.FileName);
            file.FileLineCode = fileLines;
            file.FileSize = fileSize;

            return file;
        }
    }
}
