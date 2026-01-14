using System;
using System.ComponentModel.DataAnnotations;

namespace MediLink.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; }
    }
}
