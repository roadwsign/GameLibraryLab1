using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using GameLibraryDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryInfrastructure.Services
{
    public class GameExportService : IExportService<Game>
    {
        private readonly GameLibraryDbContext _context;

        public GameExportService(GameLibraryDbContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("Input stream is not writable");

            var games = await _context.Games
                .Include(g => g.Genre)
                .Include(g => g.Developer)
                .ToListAsync(cancellationToken);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Бібліотека Ігор");

                worksheet.Cell(1, 1).Value = "Назва гри";
                worksheet.Cell(1, 2).Value = "Опис";
                worksheet.Cell(1, 3).Value = "Рік випуску";
                worksheet.Cell(1, 4).Value = "Жанр";
                worksheet.Cell(1, 5).Value = "Розробник";
                worksheet.Cell(1, 6).Value = "Посилання на постер";

                worksheet.Row(1).Style.Font.Bold = true;

                int rowIndex = 2;
                foreach (var game in games)
                {
                    worksheet.Cell(rowIndex, 1).Value = game.Title;
                    worksheet.Cell(rowIndex, 2).Value = game.Description;
                    worksheet.Cell(rowIndex, 3).Value = game.Releaseyear;
                    worksheet.Cell(rowIndex, 4).Value = game.Genre?.Name;
                    worksheet.Cell(rowIndex, 5).Value = game.Developer?.Name;
                    worksheet.Cell(rowIndex, 6).Value = game.Posterurl;

                    rowIndex++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }
        }
    }
}