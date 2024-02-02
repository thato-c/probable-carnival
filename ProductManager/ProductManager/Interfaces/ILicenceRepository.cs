using ProductManager.Models;

namespace ProductManager.Interfaces
{
    public interface ILicenceRepository
    {
        Task<IEnumerable<Licence>> GetAll();
    }
}
