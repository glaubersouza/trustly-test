using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GetGitRepo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GetGitRepo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;

        public FilesController(ILogger<FilesController> logger)
        {
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
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();

                if (response.Content is object)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                   
                }
            }
               

           

            return null;
        }
    }
}
