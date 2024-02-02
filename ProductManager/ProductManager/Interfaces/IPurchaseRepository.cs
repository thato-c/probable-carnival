using ProductManager.Models;

namespace ProductManager.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<LicencePurchase> GetByCompanyId (string companyId);

        bool Add(LicencePurchase purchase);

        bool Update(LicencePurchase purchase);
    }
}
