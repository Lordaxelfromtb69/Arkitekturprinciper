using System;
using System.Collections.Generic;

public class SearchLogic
{
    private readonly DatabaseService _dbService;

    public SearchLogic()
    {
        _dbService = new DatabaseService();
    }

    /// <summary>
    /// Søger i SQLite-databasen efter de angivne søgetermer.
    /// </summary>
    /// <param name="searchTerms">Liste af søgetermer</param>
    /// <returns>Liste over dokumenttitler der matcher søgningen</returns>
    public List<string> Search(string[] searchTerms)
    {
        var results = new List<string>();

        foreach (var term in searchTerms)
        {
            var searchResults = _dbService.Search(term);
            results.AddRange(searchResults);
        }

        return results;
    }
}
