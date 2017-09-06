using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //InitializeComponent();
            Console.WriteLine("Démarrer");
            Console.ReadLine();

            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();

            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).Distinct().ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().ToList();
            List<decimal> reqValue = asdc.HistoricalShareValues.Select(el => el.value).Distinct().ToList();


            for (int i = 0; i < reqId.Count; ++i)
            {
                Console.WriteLine("ID: " + i + " " + reqId[i]);
                Console.WriteLine("Date: " + reqDate[i]);
                Console.WriteLine("Value: " + reqValue[i]);
            }

            Console.WriteLine("debut de la generation de data");

            var hey = new List<DataFeed>();
            DateTime abc = new DateTime(2010, 1, 1);
            DateTime abc2 = new DateTime(2009,1,1);
            Share action = new Share("accor", "accordId");
            VanillaCall vanille = new VanillaCall("lol", action, abc, 8.0);
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            hey = data.GetDataFeed(vanille, abc2);
            Console.WriteLine(hey[0].Date);
            Console.WriteLine(hey[0].PriceList.ToString());
            decimal a = 0;
            hey[350].PriceList.TryGetValue("accordId", out a);
            Console.WriteLine(a);
            Console.WriteLine("fin");
        }
    }
}
