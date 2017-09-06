using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
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
            //getShareName();

            /*Donnees a demander a l'utilisateur*/

            DateTime debut = new DateTime(2009, 01, 01);
            DateTime maturite = new DateTime(2010, 10, 10);
            double strike = 5.0;

            bool simule = true;

            /*On prend par exemple la premiere action de la liste*/
            Share action = new Share("accor", "accordId");
            Console.WriteLine("Action selectionnée " + action.Name);

            DateTime debutEstimation = new DateTime(2009,1,1);
            DateTime finEstimation = new DateTime(2010,1,1);

            /*Fin entree des donnees*/

            /*Creation de l'option*/

            VanillaCall vanille = new VanillaCall("option", action, maturite, strike);

            /*Calcul de la valeur du payoff*/
            var dataFeedCalc = new List<DataFeed>();
            double payoff;

            if (simule)
            {
                IDataFeedProvider data = new SimulatedDataFeedProvider();
                dataFeedCalc = data.GetDataFeed(vanille, debut);
                payoff = vanille.GetPayoff(dataFeedCalc[200].PriceList);
                Console.WriteLine("Le payoff de l'option " + vanille.Name + " vaut " + payoff);
            }
            else
            {
                // mettre ici le code correspondant au cas historique
            }

            /*Calcul du portefeuille de couverture*/

            Pricer pricer = new Pricer();




            //InitializeComponent();

        }
        public int dateTimeConverter(DateTime date)
        {
            bool datahist = false;
            if (datahist)
            {
                return 0;
            }
            DateTime dateDebut = new DateTime() ;
            TimeSpan temps = date -  dateDebut ;
            int a = Convert.ToInt32(temps.TotalDays);
            return a;
        }

        public void getDatahist()
        {
            /*Recuperation des donnes*/

            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();

            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).Distinct().ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().ToList();
            List<decimal> reqValue = asdc.HistoricalShareValues.Select(el => el.value).Distinct().ToList();

            Console.WriteLine("Il y a  " + reqValue.Count + " valeurs");

            List<Share> listeActions = new List<Share>();
            foreach (string nom in reqId)
            {
                listeActions.Add(new Share(nom, nom));
                Console.WriteLine("Action = " + nom);
            }

            for (int i = 0; i < reqId.Count; ++i)
            {
                Console.WriteLine("ID: " + i + " " + reqId[i]);
                Console.WriteLine("Date: " + reqDate[i]);
                Console.WriteLine("Value: " + reqValue[i]);
            }

        }

        public void getShareName()
        {
            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();
            List<string> names = asdc.ShareNames.Select(el => el.name).ToList();
            foreach (string name in names)
            {
                Console.WriteLine(name);
            }
        }
        public void getDataSimul()
        {
            Console.WriteLine("debut de la generation de data");

            var dataFeedCalc = new List<DataFeed>();
            DateTime dateDebut = new DateTime(2010, 1, 1);
            DateTime dateFin = new DateTime(2009, 1, 1);
            Share action1 = new Share("accordId", "accordId");
            VanillaCall vanille1 = new VanillaCall("accordId", action1, dateDebut, 10);
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(vanille1, dateFin);
            Console.WriteLine(dataFeedCalc[0].Date);
            Console.WriteLine(dataFeedCalc[0].PriceList.ToString());
            decimal a = 10;
            dataFeedCalc[50].PriceList.TryGetValue("accordId", out a);
            Console.WriteLine(a);
            Console.WriteLine("fin");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
