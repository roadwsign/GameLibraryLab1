using System;
using GameLibraryDomain.Model;

namespace GameLibraryInfrastructure.Services
{
    public class GameDataPortServiceFactory : IDataPortServiceFactory<Game>
    {
        private readonly GameLibraryDbContext _context;

        public GameDataPortServiceFactory(GameLibraryDbContext context)
        {
            _context = context;
        }

        public IImportService<Game> GetImportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new GameImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for games with content type {contentType}");
        }

        public IExportService<Game> GetExportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new GameExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for games with content type {contentType}");
        }
    }
}