using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BigBang1112.Data;

public class UnitOfWork : IDisposable, IUnitOfWork
{
    private bool disposed;

    private readonly DbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(DbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void Save()
    {
        _context.SaveChanges();
        _logger.LogInformation("Unit of work save executed.");
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Unit of work save (async) executed.");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            _context.Dispose();
        }

        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
