using GameLibraryDomain.Model;
using GameLibraryInfrastructure;
using GameLibraryInfrastructure.Models;
using GameLibraryInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameLibraryInfrastructure.Controllers
{
    public class GamesController : Controller
    {
        private readonly GameLibraryDbContext _context;
        private readonly IDataPortServiceFactory<Game> _gameDataPortServiceFactory;
        private readonly UserManager<User> _userManager;

        public GamesController(GameLibraryDbContext context, IDataPortServiceFactory<Game> gameDataPortServiceFactory, UserManager<User> userManager)
        {
            _context = context;
            _gameDataPortServiceFactory = gameDataPortServiceFactory;
            _userManager = userManager;
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
            string currentUserId = _userManager.GetUserId(User);
            var userLibraryEntry = await _context.Userlibraries
                .Include(ul => ul.Status)
                .FirstOrDefaultAsync(ul => ul.Gameid == id && ul.Userid == currentUserId);

            ViewBag.UserLibraryEntry = userLibraryEntry;
            ViewBag.Statuses = new SelectList(_context.Gamestatuses, "Id", "Statusname");
            var statusHistory = await _context.Statushistories
                .Include(sh => sh.Newstatus)
                .Include(sh => sh.Oldstatus)
                .Where(sh => sh.Userlibrary.Gameid == id && sh.Userlibrary.Userid == currentUserId)
                .OrderByDescending(sh => sh.Changedate)
                .ToListAsync();

            ViewBag.StatusHistory = statusHistory;
            return View(game);
        }

        // GET: Games/Create
        [Authorize(Roles = "Admin, SuperAdmin")]
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
        [Authorize(Roles = "Admin, SuperAdmin")]
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
        [Authorize(Roles = "Admin, SuperAdmin")]
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
        [Authorize(Roles = "Admin, SuperAdmin")]
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
        [Authorize(Roles = "Admin, SuperAdmin")]
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
        [Authorize(Roles = "Admin, SuperAdmin")]
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

        //add to library
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToLibrary(int gameId, int statusId, bool isFavorite, int? rating, string? review)
        {
            string? currentUserId = _userManager.GetUserId(User);

            var existingEntry = await _context.Userlibraries
            .FirstOrDefaultAsync(ul => ul.Gameid == gameId && ul.Userid == currentUserId);

            if (existingEntry != null)
            {
                if (existingEntry.Statusid != statusId)
                {
                    var history = new Statushistory
                    {
                        Userlibraryid = existingEntry.Id,
                        Oldstatusid = existingEntry.Statusid,
                        Newstatusid = statusId,
                        Changedate = DateTime.Now
                    };
                    _context.Statushistories.Add(history);
                }
                existingEntry.Statusid = statusId;
                existingEntry.Isfavorite = isFavorite;
                existingEntry.Rating = rating;
                existingEntry.Review = review;
                existingEntry.Updatedat = DateTime.Now;
                _context.Update(existingEntry);
            }
            else
            {
                var newLibraryEntry = new Userlibrary
                {
                    Userid = currentUserId,
                    Gameid = gameId,
                    Statusid = statusId,
                    Isfavorite = isFavorite,
                    Rating = rating,
                    Review = review,
                    Addedat = DateTime.Now
                };
                _context.Userlibraries.Add(newLibraryEntry);
                await _context.SaveChangesAsync();

                var history = new Statushistory
                {
                    Userlibraryid = newLibraryEntry.Id,
                    Oldstatusid = null,
                    Newstatusid = statusId,
                    Changedate = DateTime.Now
                };
                _context.Statushistories.Add(history);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = gameId });
        }

        //remove from library
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromLibrary(int gameId)
        {
            string? currentUserId = _userManager.GetUserId(User);

            var entry = await _context.Userlibraries
                .FirstOrDefaultAsync(ul => ul.Gameid == gameId && ul.Userid == currentUserId);

            if (entry != null)
            {
                _context.Userlibraries.Remove(entry);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = gameId });
        }

        // Work with excel files 
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                TempData["ImportMessage"] = "Помилка: Ви не обрали файл.";
                return RedirectToAction(nameof(Index));
            }

            var importService = _gameDataPortServiceFactory.GetImportService(fileExcel.ContentType);

            using var stream = fileExcel.OpenReadStream();

            string resultMessage = await importService.ImportFromStreamAsync(stream, cancellationToken);

            TempData["ImportMessage"] = resultMessage;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", CancellationToken cancellationToken = default)
        {

            var exportService = _gameDataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"games_library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }
    }
}