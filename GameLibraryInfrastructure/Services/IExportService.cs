using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameLibraryDomain.Model;

namespace GameLibraryInfrastructure.Services
{
    public interface IExportService<TEntity> where TEntity : Entity
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }
}