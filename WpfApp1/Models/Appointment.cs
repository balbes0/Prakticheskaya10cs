namespace WpfApp1.Models
{
    public class Appointment
    {
        public int IdAppointment { get; set; }
        public DateOnly AppointmentDate {  get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public long OMS { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public int StatusId { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {Patronymic} {LastName}";
            }
        }
    }
}
