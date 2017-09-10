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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for ajoutBasket.xaml
    /// </summary>
    public partial class ajoutBasket : Window
    {
        public ajoutBasket()
        {
            InitializeComponent();
          //  base.DataContext=
        }

        private void ajouter_Click(object sender, RoutedEventArgs e)
        {
            DateTime maturité = Convert.ToDateTime(maturity.Text);
            double strike = Convert.ToInt32(Strike.Text);
            //ShareNames share =
            this.Close();
        }

        private void annuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
