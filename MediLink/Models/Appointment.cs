public class Appointment
{
    public int Id { get; set; }
    public string PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
}
