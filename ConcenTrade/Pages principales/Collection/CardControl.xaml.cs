using System.Windows.Controls;

namespace Concentrade.Pages_principales.Collection
{
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public void SetCardName(string name)
        {
            if (CardNameText != null)
            {
                CardNameText.Text = name;
            }
        }
    }
} 