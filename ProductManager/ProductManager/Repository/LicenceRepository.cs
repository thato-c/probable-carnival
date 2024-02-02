using ProductManager.Interfaces;
using ProductManager.Models;

namespace ProductManager.Repository
{
    public class LicenceRepository : ILicenceRepository
    {
        public Task<IEnumerable<Licence>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
