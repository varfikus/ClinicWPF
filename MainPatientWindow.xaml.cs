using ClinicWPF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClinicWPF
{
    /// <summary>
    /// Логика взаимодействия для MainPatientWindow.xaml
    /// </summary>
    public partial class MainPatientWindow : Window
    {
        private DispatcherTimer _timer;

        public MainPatientWindow()
        {
            InitializeComponent();
            Appointment.RemovePastAppointments();
            LoadAppointments();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is NewAppointmentWindow)
                {
                    window.Activate();
                    return;
                }
            }

            NewAppointmentWindow newAppointmentWindow = new NewAppointmentWindow();
            newAppointmentWindow.Show();
        }

        public void LoadAppointments()
        {
            using (var context = new ClinicdbContext())
            {
                var userAppointments = context.Appointments
                                              .Include(a => a.Doctor)
                                              .Include(a => a.Clinic)
                                              .Where(a => a.Patientid == Patient.CurrentUser.Id);

                var appointments = userAppointments
                    .Select(a => new
                    {
                        DoctorName = a.Doctor.Lastname + " " + a.Doctor.Firstname,
                        ClinicName = a.Clinic.Clinicname,
                        a.Appointmentdatetime,
                        a.Reasonforvisit
                    })
                    .ToList();

                AppointmentsDataGrid.ItemsSource = appointments;
            }
        }
    }
}
