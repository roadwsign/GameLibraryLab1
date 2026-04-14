using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameLibraryDomain.Model;

namespace GameLibraryInfrastructure.Services
{
    public interface IImportService<TEntity> where TEntity : Entity
    {
        Task<string> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}