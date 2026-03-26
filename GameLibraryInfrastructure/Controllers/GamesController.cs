using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameLibraryDomain.Model;
using GameLibraryInfrastructure;

namespace GameLibraryInfrastructure.Controllers
{
    public class GamesController : Controller
    {
        private readonly GameLibraryDbContext _context;

        public GamesController(GameLibraryDbContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index(int? developerId, int? genreId, int? year, string? searchString)
        {
            ViewBag.Developerid = new SelectList(_context.Developers, "Id", "Name", developerId);
            ViewBag.Genreid = new SelectList(_context.Genres, "Id", "Name", genreId);

            var years = Enumerable.Range(1950, DateTime.Now.Year - 1949).OrderByDescending(y => y).ToList();
            ViewBag.Years = new SelectList(years, year);

            var gamesQuery = _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Genre)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var lowerSearch = searchString.ToLower().Trim();
                gamesQuery = gamesQuery.Where(s => s.Title.ToLower().Contains(lowerSearch));
                ViewBag.CurrentFilter = searchString;
            }

            if (developerId.HasValue) gamesQuery = gamesQuery.Where(g => g.Developerid == developerId);
            if (genreId.HasValue) gamesQuery = gamesQuery.Where(g => g.Genreid == genreId);
            if (year.HasValue) gamesQuery = gamesQuery.Where(g => g.Releaseyear == year);

            ViewBag.FilterName = "Каталог ігор";
            return View(await gamesQuery.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["Developerid"] = new SelectList(_context.Developers, "Id", "Name");
            ViewData["Genreid"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Releaseyear,Genreid,Developerid,Posterurl,Id")] Game game)
        {
            ModelState.Remove("Createdat");
            ModelState.Remove("Updatedat");
            ModelState.Remove("Genre");
            ModelState.Remove("Developer");
            if (ModelState.IsValid)
            {
                game.Createdat = DateTime.Now;
                game.Updatedat = DateTime.Now;
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Developerid"] = new SelectList(_context.Developers, "Id", "Name", game.Developerid);
            ViewData["Genreid"] = new SelectList(_context.Genres, "Id", "Name", game.Genreid);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["Developerid"] = new SelectList(_context.Developers, "Id", "Name", game.Developerid);
            ViewData["Genreid"] = new SelectList(_context.Genres, "Id", "Name", game.Genreid);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Releaseyear,Genreid,Developerid,Createdat,Updatedat,Posterurl,Id")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Genre");
            ModelState.Remove("Developer");
            ModelState.Remove("Updatedat");

            if (ModelState.IsValid)
            {
                try
                {
                    game.Updatedat = DateTime.Now;
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Developerid"] = new SelectList(_context.Developers, "Id", "Name", game.Developerid);
            ViewData["Genreid"] = new SelectList(_context.Genres, "Id", "Name", game.Genreid);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
