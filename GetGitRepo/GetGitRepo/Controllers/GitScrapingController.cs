using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using GetGitRepo.Models;
using GetGitRepo.Utilits;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GetGitRepo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GitScrapingController : ControllerBase
    {
        private readonly ILogger<GitScrapingController> _logger;
        private readonly string URL_Base = "https://github.com/";

        public GitScrapingController(ILogger<GitScrapingController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDataAsync([FromHeader] string URLRepo) 
        {
            var retorno = await GetHtmlAsync(URLRepo);
            return Ok(retorno); 
        }

        public async Task<IEnumerable<Files>> GetHtmlAsync(string url) 
        {
            using (HttpClient httpClient = new HttpClient()) 
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                HttpContent content = response.Content;
                HtmlDocument document = new HtmlDocument();
                //var htmlContext = await content.ReadAsStringAsync(); 
                document.LoadHtml(await content.ReadAsStringAsync());

                var itensBox = document.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("aria-labelledby","").Equals("files")).ToList();

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

                return fileList;
                                
            }
        }

       

        private async Task<Files> getFileInfoAsync(string fileURL)
        {
            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(fileURL, HttpCompletionOption.ResponseContentRead);
            HttpContent content = response.Content;
            HtmlDocument document = new HtmlDocument();
            //var htmlContext = await content.ReadAsStringAsync(); 
            document.LoadHtml(await content.ReadAsStringAsync());

            var file = new Files();

            _logger.Log(LogLevel.Information, fileURL);

            var nomeArquivo = document.DocumentNode.Descendants("strong").Where(node => node.GetAttributeValue("class", "").Equals("final-path")).FirstOrDefault();
            var fileData = document.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0")).FirstOrDefault();

            //var nodesInfo = document.DocumentNode.SelectNodes("/html/body/div[4]/div/main/div[2]/div/div[3]/div[1]/div[1]/text()");
            var nodesInfo = document.DocumentNode.SelectNodes("//div[@class = 'text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0']");
            var fileLines = 0;
            decimal fileSize = 0;

            foreach (var item in nodesInfo)
            {
                var text = item.InnerText.Replace("\n", "").Replace("  ", "").Replace("(","").Replace(")","").Replace("sloc", "").Split(" ");

                if (item.InnerText.Contains("lines")) 
                {
                    fileLines = Convert.ToInt32(text[0]);
                }

                if (item.InnerText.Contains("Bytes") || item.InnerText.Contains("KB"))
                {
                    fileSize = Convert.ToDecimal(text[3]);
                }
            }

            //var lines = document.DocumentNode.SelectNodes("/html/body/div[4]/div/main/div[2]/div/div[3]/div[1]/div[1]/text()[1]");
            //var size = document.DocumentNode.SelectNodes("/html/body/div[4]/div/main/div[2]/div/div[3]/div[1]/div[1]/text()[2]");

            file.FileName = nomeArquivo.InnerText.Replace("\n", "").Replace("  ", "");
            file.FileExtension = Utils.getFileExtension(file.FileName);
            file.FileLineCode = fileLines;
            file.FileSize = fileSize;

            return file;
        }
    }
}
