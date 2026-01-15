namespace MediLink.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        // Make sure these properties exist
        public string Province { get; set; }
        public string City { get; set; }
    }
}
