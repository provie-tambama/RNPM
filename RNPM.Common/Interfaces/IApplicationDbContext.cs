using System.Threading;
using System.Threading.Tasks;

namespace RNPM.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}