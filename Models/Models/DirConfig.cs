namespace Models.Models
{
    public class DirConfig
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Extensions { get; set; }
        public string SkipScan { get; set; }
        public string Name { get; set; }

        // Foreign key
        public int ProtectedId { get; set; }  // Foreign key to Protected

        // Navigation property
        public Protected Protected { get; set; }  // Reference to Protected
    }
}
