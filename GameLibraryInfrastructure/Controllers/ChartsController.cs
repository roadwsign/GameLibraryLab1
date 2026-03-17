using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameLibraryDomain.Model;
using GameLibraryInfrastructure;

namespace GameLibraryInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private record CountByYearResponseItem(string Year, int Count);
        private record CountByGenreResponseItem(string Genre, int Count);

        private readonly GameLibraryDbContext _context;

        public ChartsController(GameLibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet("gamesByYear")]
        public async Task<JsonResult> GetGamesByYearAsync(CancellationToken cancellationToken)
        {
            var rawData = await _context.Games
                .GroupBy(g => g.Releaseyear)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderBy(x => x.Year)
                .ToListAsync(cancellationToken);

            var responseItems = rawData
                .Select(g => new CountByYearResponseItem(g.Year.ToString(), g.Count))
                .ToList();

            return new JsonResult(responseItems);
        }

        [HttpGet("gamesByGenre")]
        public async Task<JsonResult> GetGamesByGenreAsync(CancellationToken cancellationToken)
        {
            var rawData = await _context.Games
                .Include(g => g.Genre)
                .GroupBy(g => g.Genre.Name)
                .Select(g => new { Genre = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var responseItems = rawData
                .Select(g => new CountByGenreResponseItem(g.Genre, g.Count))
                .ToList();

            return new JsonResult(responseItems);
        }
    }
}