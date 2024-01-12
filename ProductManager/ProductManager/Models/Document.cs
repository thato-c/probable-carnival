namespace ProductManager.Models
{
    public class Document
    {
        public int DocumentId { get; set; }

        public string Name { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; }

        public string FileURL { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
