using System;
using System.Collections.Generic;

namespace ClinicWPF.Models;

public partial class Appointment
{
    public int Id { get; set; }

    public int Doctorid { get; set; }

    public int Patientid { get; set; }

    public int Clinicid { get; set; }

    public DateTime Appointmentdatetime { get; set; }

    public string? Reasonforvisit { get; set; }

    public virtual Clinic Clinic { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public static void RemovePastAppointments()
    {
        using (var context = new ClinicdbContext())
        {
            var currentTime = DateTime.UtcNow.AddMinutes(10);
            var pastAppointments = context.Appointments
                                          .Where(a => a.Appointmentdatetime <= currentTime)
                                          .ToList();
            context.Appointments.RemoveRange(pastAppointments);
            context.SaveChanges();
        }
    }
}
