using System;
using System.ComponentModel.DataAnnotations;

namespace MediLink.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string PatientId { get; set; }
        [Required]
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public string Status { get; set; }
    }
}
