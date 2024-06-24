using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.View.Cards
{
    public partial class AppointmentRichTextBoxView : UserControl
    {
        public AppointmentRichTextBoxView(AppointmentRichTextBox appointmentRichTextBox)
        {
            InitializeComponent();
            DataContext = appointmentRichTextBox;
        }
    }
}
