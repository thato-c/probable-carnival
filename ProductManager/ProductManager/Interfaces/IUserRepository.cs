using ProductManager.Models;

namespace ProductManager.Interfaces
{
    public interface IUserRepository
    {
        bool Add(User user);
    }
}
