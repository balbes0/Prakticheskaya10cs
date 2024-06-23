namespace WpfApp1.Models
{
    public class DoctorWithSpecialityName
    {
        public int? IdDoctor { get; set; }

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Patronymic { get; set; }

        public int? SpecialityId { get; set; }

        public string EnterPassword { get; set; } = null!;

        public string WorkAddress { get; set; } = null!;

        public string SpecialityName { get; set; } = null!;
    }
}
