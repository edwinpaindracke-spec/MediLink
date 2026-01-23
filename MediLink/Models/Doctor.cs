public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Specialty { get; set; }
    public string Hospital { get; set; }

     // ✅ REQUIRED FOREIGN KEY
    public int HospitalId { get; set; }

    // ✅ NAVIGATION PROPERTY
   
}

