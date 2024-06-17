namespace WpfApp1.Models
{
    public class Appointment
    {
        public long OMS { get; set; }
        public string FullName { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
    }
}
