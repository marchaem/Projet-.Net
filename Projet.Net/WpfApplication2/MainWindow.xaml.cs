using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApplication2.Portfolio;
using WpfApplication2.Options;
using WpfApplication2.Entree;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static DateTime debutEstimation = new DateTime(2009, 1, 1);
        public MainWindow()
        {
            /*Donnees a demander a l'utilisateur*/
            Share action = new Share("accor", "accordId");
            Pricer pricer = new Pricer();
            List<Share> liste = new List<Share>() { action };
            DateTime maturite = new DateTime(2001, 10, 10);
            List< String > listeAction = new List<String> { "Accor"};
            Entrees donnes = new Entrees(0, 7.0, new DateTime(2000, 1, 1), listeAction, maturite, new DateTime(2000, 1, 1), new DateTime(2001, 1, 10), 1, 0,"option", new List<double>() { 1.0 });

           

            Console.WriteLine("Simulation lancée avec : ");
            Console.WriteLine("K = " + strike + "€");
            Console.WriteLine("Echeance " + maturite.ToString());
          
            

            /*On rentre en dur la valeur de l'action pour l'instant*/
            
            Console.WriteLine("Action selectionnée " + action.Name);          
            DateTime finEstimation = new DateTime(2010,1,1);

            /*Fin entree des donnees*/

            /*Creation de l'option*/

            VanillaCall vanille = new VanillaCall("option", action, maturite, donnes.strike);
            OptionVanille option = new OptionVanille(vanille);

            /*Calcul du portefeuille de couverture*/




            //Actualisation de la valeur du portefeuille
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(vanille, new DateTime(2000,1,1));
            PorteFeuilleVanille porteFeuilleVanille = new PorteFeuilleVanille(option);
            porteFeuilleVanille.actulisationPorteSimu(dataFeedCalc, donnes);   
            int i = 0;
            DateTime date = donnes.debutSimulation;
            while (date <=  donnes.finSimulation)
            {
                i  = porteFeuilleVanille.dateTimeConverter(donnes.debutSimulation, date);
                Console.WriteLine("le call vaut : " + pricer.PriceCall(vanille, date, 365,(double) dataFeedCalc[i].PriceList[vanille.UnderlyingShare.Id], 0.4).Price);
                Console.WriteLine("i vaut :" + i);
                Console.Write("le spot vaut : " + dataFeedCalc[i].PriceList[vanille.UnderlyingShare.Id]);
                Console.WriteLine("le portefeuille vaut : ");
                Console.WriteLine(porteFeuilleVanille.ToString1(donnes.debutSimulation,date));
                Console.WriteLine("Tracking Error = " + porteFeuilleVanille.trackingErrors[i]  +" à la date " + date.ToString());
                date=date.AddDays(donnes.pas);
            }
           

            
            
            



            //InitializeComponent();

        }
        public int dateTimeConverter(DateTime date)
        {
            bool datahist = false;
            if (datahist)
            {
                return 0;
            }
           
            DateTime dateDebut = debutEstimation;
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
        // Draw a simple graph.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            const double margin = 10;
            double xmin = margin;
            double xmax = canGraph.Width - margin;
            double ymin = margin;
            double ymax = canGraph.Height - margin;
            const double step = 10;

            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(canGraph.Width, ymax)));
            for (double x = xmin + step;
                x <= canGraph.Width - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            canGraph.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, canGraph.Height)));
            for (double y = step; y <= canGraph.Height - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            canGraph.Children.Add(yaxis_path);

            // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            Random rand = new Random();
            for (int data_set = 0; data_set < 3; data_set++)
            {
                int last_y = rand.Next((int)ymin, (int)ymax);

                PointCollection points = new PointCollection();
                for (double x = xmin; x <= xmax; x += step)
                {
                    last_y = rand.Next(last_y - 10, last_y + 10);
                    if (last_y < ymin) last_y = (int)ymin;
                    if (last_y > ymax) last_y = (int)ymax;
                    points.Add(new Point(x, last_y));
                }

                Polyline polyline = new Polyline();
                polyline.StrokeThickness = 1;
                polyline.Stroke = brushes[data_set];
                polyline.Points = points;

                canGraph.Children.Add(polyline);
            }
        }
    }
}
