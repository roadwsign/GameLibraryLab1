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
    public class GamestatusController : Controller
    {
        private readonly GameLibraryDbContext _context;

        public GamestatusController(GameLibraryDbContext context)
        {
            _context = context;
        }

        // GET: Gamestatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gamestatuses.ToListAsync());
        }

        // GET: Gamestatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamestatus = await _context.Gamestatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gamestatus == null)
            {
                return NotFound();
            }

            return View(gamestatus);
        }

        // GET: Gamestatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gamestatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Statusname,Id")] Gamestatus gamestatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gamestatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gamestatus);
        }

        // GET: Gamestatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamestatus = await _context.Gamestatuses.FindAsync(id);
            if (gamestatus == null)
            {
                return NotFound();
            }
            return View(gamestatus);
        }

        // POST: Gamestatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Statusname,Id")] Gamestatus gamestatus)
        {
            if (id != gamestatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gamestatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GamestatusExists(gamestatus.Id))
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
            return View(gamestatus);
        }

        // GET: Gamestatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamestatus = await _context.Gamestatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gamestatus == null)
            {
                return NotFound();
            }

            return View(gamestatus);
        }

        // POST: Gamestatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gamestatus = await _context.Gamestatuses.FindAsync(id);
            if (gamestatus != null)
            {
                _context.Gamestatuses.Remove(gamestatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GamestatusExists(int id)
        {
            return _context.Gamestatuses.Any(e => e.Id == id);
        }
    }
}
