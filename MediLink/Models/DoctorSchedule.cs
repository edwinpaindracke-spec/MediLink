public class DoctorSchedule
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DateTime AvailableDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
