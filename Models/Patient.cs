using System;
using System.Collections.Generic;

namespace ClinicWPF.Models;

public partial class Patient
{
    internal static Patient CurrentUser;

    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public DateOnly? Dateofbirth { get; set; }

    public string Gender { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
