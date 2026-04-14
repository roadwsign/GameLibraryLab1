using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using GameLibraryDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryInfrastructure.Services
{
    public class GameImportService : IImportService<Game>
    {
        private readonly GameLibraryDbContext _context;

        public GameImportService(GameLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<string> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));

            int addedCount = 0;
            int updatedCount = 0;
            int skippedCount = 0;

            using (var workBook = new XLWorkbook(stream))
            {
                var worksheet = workBook.Worksheets.FirstOrDefault();
                if (worksheet == null) return "Помилка: Excel-файл порожній.";

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var title = row.Cell(1).Value.ToString();
                    var rawGenre = row.Cell(4).Value.ToString();
                    var rawDev = row.Cell(5).Value.ToString();

                    var genreName = rawGenre.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
                    var devName = rawDev.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();

                    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(genreName) || string.IsNullOrWhiteSpace(devName))
                    {
                        skippedCount++;
                        continue;
                    }

                    var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == genreName.ToLower(), cancellationToken);
                    if (genre == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    var dev = await _context.Developers.FirstOrDefaultAsync(d => d.Name.ToLower() == devName.ToLower(), cancellationToken);
                    if (dev == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    var description = row.Cell(2).Value.ToString();
                    int.TryParse(row.Cell(3).Value.ToString(), out int year);
                    var posterUrl = row.Cell(6).Value.ToString();

                    var existingGame = await _context.Games.FirstOrDefaultAsync(g => g.Title == title, cancellationToken);

                    if (existingGame != null)
                    {
                        bool isChanged = false;

                        if (existingGame.Description != description) { existingGame.Description = description; isChanged = true; }
                        if (existingGame.Genreid != genre.Id) { existingGame.Genreid = genre.Id; isChanged = true; }
                        if (existingGame.Developerid != dev.Id) { existingGame.Developerid = dev.Id; isChanged = true; }
                        if (existingGame.Posterurl != posterUrl) { existingGame.Posterurl = posterUrl; isChanged = true; }

                        if (year > 1950 && existingGame.Releaseyear != year)
                        {
                            existingGame.Releaseyear = year;
                            isChanged = true;
                        }

                        if (isChanged)
                        {
                            existingGame.Updatedat = DateTime.Now;
                            _context.Games.Update(existingGame);
                            updatedCount++;
                        }
                    }
                    else
                    {
                        var game = new Game
                        {
                            Title = title,
                            Description = description,
                            Releaseyear = year > 1950 ? year : DateTime.Now.Year,
                            Genreid = genre.Id,
                            Developerid = dev.Id,
                            Posterurl = posterUrl,
                            Createdat = DateTime.Now,
                            Updatedat = DateTime.Now
                        };

                        _context.Games.Add(game);
                        addedCount++;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return $"Імпорт завершено! Додано нових: {addedCount}. Оновлено: {updatedCount}. Пропущено: {skippedCount}.";
        }
    }
}