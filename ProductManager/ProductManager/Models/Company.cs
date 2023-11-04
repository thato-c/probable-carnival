namespace ProductManager.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set;} = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
    }
}
