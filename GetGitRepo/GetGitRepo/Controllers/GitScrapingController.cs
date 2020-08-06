using GetGitRepo.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GetGitRepo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GitScrapingController : ControllerBase
    {
        private readonly ILogger<GitScrapingController> _logger;
        
        public GitScrapingController(ILogger<GitScrapingController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDataAsync([FromHeader] string URLRepo) 
        {
            var retorno = await HtmlScraping.GetHtmlAsync(URLRepo);
            return Ok(retorno); 
        }

        
    }
}
