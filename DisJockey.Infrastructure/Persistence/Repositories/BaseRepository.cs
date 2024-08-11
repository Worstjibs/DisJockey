using DisJockey.Application.Interfaces;

namespace DisJockey.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    protected readonly DataContext _context;

    protected BaseRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
