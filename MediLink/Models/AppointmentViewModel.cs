using System.ComponentModel.DataAnnotations;

namespace MediLink.Models
{
    public class AppointmentViewModel
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }

        [Required]
        [Display(Name = "Patient Name")]
        public string PatientName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Describe your problem")]
        public string ProblemDescription { get; set; }
        [Required]
        [Display(Name = "Appointment Date & Time")]
        public DateTime AppointmentDateTime { get; set; }

    }
}
