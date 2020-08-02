using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GetGitRepo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebScrapingController : ControllerBase
    {
        [HttpGet]
        public string Get() 
        {
            GetHtmlAsync();
            
            return "Teste!!!";
        }

        public async void GetHtmlAsync() 
        {
            //CancellationTokenSource cancellationToken = new CancellationTokenSource();
            //HttpClient httpClient = new HttpClient();
            //HttpResponseMessage request = await httpClient.GetAsync("http://www.morningstar.com/");

            //cancellationToken.Token.ThrowIfCancellationRequested();

            //Stream response = await request.Content.ReadAsStreamAsync();

            //cancellationToken.Token.ThrowIfCancellationRequested();
            string base_url = "https://github.com/aspnet/RoslynCodeDomProvider";
            WebClient browser = new WebClient();
            var html = browser.DownloadString(base_url);
            
            
        }
    }
}
