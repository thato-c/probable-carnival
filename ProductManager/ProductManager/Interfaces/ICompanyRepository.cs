using ProductManager.Models;

namespace ProductManager.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllAsync();

        Task<Company> GetByIdAsync(int companyId);

        Task<string> GetAdminByIdAsync(int companyId);

        Task<Company> GetByAnyProperty(string Name, string Email, string AdminEmail, string PhoneNumber );

        bool Add(Company company);

        bool Save();
    }
}
