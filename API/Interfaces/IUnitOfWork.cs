using System.Threading.Tasks;

namespace API.Interfaces {
    public interface IUnitOfWork {
        IUserRepository UserRepository { get; }
        ITrackRepository TrackRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}