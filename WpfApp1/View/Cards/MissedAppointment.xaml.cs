using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.ViewModel;

namespace WpfApp1.View.Cards
{
    public partial class MissedAppointment : UserControl
    {
        public MissedAppointment(Appointment appointment)
        {
            InitializeComponent();
            DataContext = appointment;
        }
    }
}
