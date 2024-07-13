using ClinicWPF.Models;
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
using System.Windows.Shapes;

namespace ClinicWPF
{
    /// <summary>
    /// Логика взаимодействия для NewAppointmentWindow.xaml
    /// </summary>
    public partial class NewAppointmentWindow : Window
    {
        public NewAppointmentWindow()
        {
            InitializeComponent();
            DatePicker1.DisplayDateStart = DateTime.Now;
            DatePicker1.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Now.AddDays(-1)));
            UpdateClinicsList();
        }

        private void DatePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePicker1.SelectedDate.HasValue)
            {
                UpdateTimeList();
            }
        }

        private void UpdateDoctorsList()
        {
            int selectedClinicId = (int)ClinicsComboBox.SelectedValue;

            using (var context = new ClinicdbContext())
            {
                var doctors = context.Doctorclinics
                    .Where(dc => dc.Clinicid == selectedClinicId)
                    .Select(dc => dc.Doctor)
                    .ToList();

                DoctorsComboBox.ItemsSource = doctors;
                DoctorsComboBox.DisplayMemberPath = "FullInfo";
                DoctorsComboBox.SelectedValuePath = "Id";
            }
        }

        private void UpdateClinicsList()
        {
            using (var context = new ClinicdbContext())
            {
                ClinicsComboBox.ItemsSource = context.Clinics.ToList();
                ClinicsComboBox.DisplayMemberPath = "Clinicname";
                ClinicsComboBox.SelectedValuePath = "Id";
            }
        }

        private void ClinicsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDoctorsList();
        }

        private void UpdateTimeList()
        {
            DateTime selectedDate = DatePicker1.SelectedDate.Value.Date;
            DateTime selectedDateUtc = selectedDate.ToUniversalTime();
            List<TimeSpan> availableTimes = new List<TimeSpan>();

            for (int hour = 8; hour <= 18; hour++)
            {
                availableTimes.Add(new TimeSpan(hour, 0, 0));
            }

            using (var context = new ClinicdbContext())
            {
                var appointments = context.Appointments
                    .Where(a => a.Appointmentdatetime.Date == selectedDateUtc.Date)
                    .ToList();

                foreach (var appointment in appointments)
                {
                    TimeSpan appointmentTime = appointment.Appointmentdatetime.TimeOfDay;
                    availableTimes.Remove(appointmentTime);
                }
            }

            if (selectedDate == DateTime.Today)
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                availableTimes = availableTimes.Where(time => time > currentTime).ToList();
            }

            TimeComboBox.Items.Clear();
            foreach (var time in availableTimes)
            {
                TimeComboBox.Items.Add(time.ToString(@"hh\:mm"));
            }
        }

        private void AppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorsComboBox.SelectedValue == null || DatePicker1.SelectedDate == null || TimeComboBox.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Заполните все поля");
                return;
            }

            using (var context = new ClinicdbContext())
            {
                int selectedDoctorId = (int)DoctorsComboBox.SelectedValue;
                int selectedPatientId = Patient.CurrentUser.Id;
                int selectedClinicId = (int)ClinicsComboBox.SelectedValue;
                DateTime selectedDate = DatePicker1.SelectedDate.Value;
                string selectedTime = TimeComboBox.SelectedItem.ToString();
                string reason = "Плановый осмотр";
                if (ReasonTextBox.Text != "") 
                { 
                     reason = ReasonTextBox.Text;
                }

                DateTime appointmentDateTime = DateTime.Parse($"{selectedDate.ToShortDateString()} {selectedTime}");

                appointmentDateTime = DateTime.SpecifyKind(appointmentDateTime, DateTimeKind.Local);
                appointmentDateTime = appointmentDateTime.ToUniversalTime();

                var newAppointment = new Appointment
                {
                    Doctorid = selectedDoctorId,
                    Patientid = selectedPatientId,
                    Clinicid = selectedClinicId,
                    Appointmentdatetime = appointmentDateTime,
                    Reasonforvisit = reason
                };

                context.Appointments.Add(newAppointment);
                context.SaveChanges();

                System.Windows.MessageBox.Show("Вы успешно записались");

                new MainPatientWindow().Show();
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is MainPatientWindow)
                {
                    window.Activate();
                }
            }
        }
    }
}