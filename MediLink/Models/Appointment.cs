using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediLink.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public IdentityUser Patient { get; set; }

        [Required]
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public int DoctorId { get; set; }

        [Required]
        public DateTime? AppointmentDateTime { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ProblemDescription { get; internal set; }
        public string Location { get; internal set; }
        public string PatientName { get; internal set; }
    }
}
