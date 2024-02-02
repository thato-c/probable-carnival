using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Interfaces;
using ProductManager.Models;

namespace ProductManager.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDBContext _context;

        public CompanyRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public bool Add(Company company)
        {
            _context.Companies.Add(company);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task<string> GetAdminByIdAsync(int companyId)
        {
            var result = await _context.Companies
             .Where(c => c.CompanyId == companyId)
             .Select(c => c.AdminEmail)
             .FirstAsync();

            return result;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company> GetByAnyProperty(string Name, string Email, string AdminEmail, string PhoneNumber)
        {
            var existingCompany = await _context.Companies.FirstOrDefaultAsync(c =>
                        c.CompanyName == Name ||
                        c.CompanyEmail == Email ||
                        c.AdminEmail == AdminEmail ||
                        c.CompanyPhoneNumber == PhoneNumber);

            return existingCompany;
        }

        public async Task<Company> GetByIdAsync(int companyId)
        {
            var result = await _context.Companies
                .Where(c => c.CompanyId == companyId)
                .FirstOrDefaultAsync();

            return result;
        }

        
    }
}
