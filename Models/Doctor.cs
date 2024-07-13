using System;
using System.Collections.Generic;

namespace ClinicWPF.Models;

public partial class Doctor
{
    internal static Doctor CurrentUser;

    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Specialty { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public bool? Ismaindoctor { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Doctorclinic> Doctorclinics { get; set; } = new List<Doctorclinic>();

    public string FullInfo => $"{Firstname} {Lastname}, {Specialty}";
}
