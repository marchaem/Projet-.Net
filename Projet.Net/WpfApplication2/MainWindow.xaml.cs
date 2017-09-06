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
using PricingLibrary.FinancialProducts;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Console.WriteLine("Démarrer");
            Console.ReadLine();

            /*Recuperation des donnes*/

            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();

            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).Distinct().ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().ToList();
            List<decimal> reqValue = asdc.HistoricalShareValues.Select(el => el.value).Distinct().ToList();

            /*Calcul des donnees simulées*/

            Console.WriteLine("debut de la generation de data");

            var hey = new List<DataFeed>();
            DateTime abc = new DateTime(2010, 1, 1);
            DateTime abc2 = new DateTime(2009, 1, 1);
            Share action1 = new Share("accor", "accordId");
            VanillaCall vanille1 = new VanillaCall("Accor", action1, abc, 10);
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            hey = data.GetDataFeed(vanille1, abc2);
            Console.WriteLine(hey[0].Date);
            Console.WriteLine(hey[0].PriceList.ToString());
            decimal a = 10;
            hey[50].PriceList.TryGetValue("accorId", out a);
            Console.WriteLine(a);
            Console.WriteLine("fin");

            /*Fin données simulées*/

            Console.WriteLine("Il y a  " + reqValue.Count + " valeurs");

            /*Recuperation liste*/

            List<Share> listeActions = new List<Share>();
            foreach(string nom in reqId) {
                listeActions.Add(new Share(nom, nom));
                Console.WriteLine("Action = " + nom);
            }

            /*Donnees a demander a l'utilisateur*/

            DateTime debut = new DateTime(2015, 01, 01);

            DateTime maturite = new DateTime(2015, 10, 10);
            double strike = 10.0;

            bool simule = true;

            /*On prend par exemple la premiere action de la liste*/
            Share action = listeActions[0];
            Console.WriteLine("Action selectionné " + action.Name);

            DateTime debutEstimation;
            DateTime finEstimation;

            /*Fin entree des donnees*/

            /*Creation de l'option*/

            VanillaCall vanille = new VanillaCall("option", action, maturite, strike);

            /*Calcul de la valeur du payoff*/

            double payoff = vanille.GetPayoff(hey[0].PriceList);
            Console.WriteLine("La payoff de l'option vaut " + payoff);

            //vanille.GetPayoff();

            /*Calcul du portefeuille de couverture*/


            //InitializeComponent();


            for (int i = 0; i < reqId.Count; ++i)
            {
                Console.WriteLine("ID: " + i + " " + reqId[i]);
                Console.WriteLine("Date: " + reqDate[i]);
                Console.WriteLine("Value: " + reqValue[i]);
            }

        }
    }
}
