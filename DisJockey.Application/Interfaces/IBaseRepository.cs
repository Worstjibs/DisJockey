namespace DisJockey.Application.Interfaces;

public interface IBaseRepository
{
    Task<bool> SaveChangesAsync();
}
