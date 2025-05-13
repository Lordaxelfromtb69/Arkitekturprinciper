using Microsoft.AspNetCore.Mvc;
using SearchAPI;
using Shared.Model;
using System.Threading.Tasks;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly SearchLogic _searchLogic;

    public SearchController()
    {
        _searchLogic = new SearchLogic();
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required.");
        }

        var searchTerms = query.Split(' ');
        var result = _searchLogic.Search(searchTerms);

        return Ok(result);
    }
}
