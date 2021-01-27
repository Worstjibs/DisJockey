using System.Threading.Tasks;

namespace API.Interfaces {
    public interface IUnitOfWork {
        IUserRepository _userRepository { get; }
        ITrackRepository _trackRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}