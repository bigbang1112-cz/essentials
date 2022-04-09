namespace BigBang1112.Data;

public class MockUnitOfWork : IUnitOfWork
{
    public void Save()
    {
        
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
