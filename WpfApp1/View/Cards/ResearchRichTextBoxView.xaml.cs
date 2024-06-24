using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.View.Cards
{
    public partial class ResearchRichTextBoxView : UserControl
    {
        public ResearchRichTextBoxView(ResearchRichTextBox researchRichTextBox)
        {
            InitializeComponent();
            DataContext = researchRichTextBox;
        }
    }
}
