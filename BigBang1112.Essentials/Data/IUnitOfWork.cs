
namespace BigBang1112.Data;

public interface IUnitOfWork
{
    void Save();
    Task SaveAsync(CancellationToken cancellationToken = default);
}
