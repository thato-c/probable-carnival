namespace ProductManager.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public Company Company { get; set; }
    }
}
