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


            /*Recuperation des donnees pour calculer le payoff */



            /*Calcul de la valeur du payoff*/

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
