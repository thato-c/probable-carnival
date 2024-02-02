using ProductManager.Interfaces;
using ProductManager.Models;

namespace ProductManager.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        public bool Add(LicencePurchase purchase)
        {
            throw new NotImplementedException();
        }

        public Task<LicencePurchase> GetByCompanyId(string companyId)
        {
            throw new NotImplementedException();
        }

        public bool Update(LicencePurchase purchase)
        {
            throw new NotImplementedException();
        }
    }
}
