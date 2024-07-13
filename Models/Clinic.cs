using System;
using System.Collections.Generic;

namespace ClinicWPF.Models;

public partial class Clinic
{
    public int Id { get; set; }

    public string Clinicname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Doctorclinic> Doctorclinics { get; set; } = new List<Doctorclinic>();
}
