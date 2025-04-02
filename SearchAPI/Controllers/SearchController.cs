using Microsoft.AspNetCore.Mvc;
using SearchAPI;
using System.Collections.Generic;
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

    /// <summary>
    /// Modtager en søgeforespørgsel og returnerer søgeresultater fra databasen.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required.");
        }

        var searchTerms = query.Split(' '); // Splitter input på mellemrum
        var result = _searchLogic.Search(searchTerms); // Kalder søgelogikken

        return Ok(result);
    }
}
