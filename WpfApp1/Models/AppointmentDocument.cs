using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public partial class AppointmentDocument
    {
        public int? AppointmentId { get; set; }

        public string Rtf { get; set; } = null!;
    }
}