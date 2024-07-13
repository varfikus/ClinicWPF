using System;
using System.Collections.Generic;

namespace ClinicWPF.Models;

public partial class Doctorclinic
{
    public int Id { get; set; }

    public int Doctorid { get; set; }

    public int Clinicid { get; set; }

    public bool? Ismaindoctor { get; set; }

    public virtual Clinic Clinic { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
