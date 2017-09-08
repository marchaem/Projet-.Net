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
using WpfApplication2.Simu;
using System.Windows.Media;
using System.Windows.Shapes;

using WpfApplication2.Data;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static DateTime debutEstimation = new DateTime(2009, 1, 1);
        private List<double> option;
        private List<double> portefeuille;
        private Entrees entree;
        public MainWindow()
        {
            /*Donnees a demander a l'utilisateur*/
            /*Share action = new Share("accor", "accordId");
            Pricer pricer = new Pricer();
            List<Share> liste = new List<Share>() { action };
            DateTime maturite = new DateTime(2001, 10, 10);
            Entrees donnes = new Entrees(0, 7.0, new DateTime(2000, 1, 1), liste, maturite, new DateTime(2000, 1, 1), new DateTime(2001, 1, 10), 1, 0);
            DataHisto histo = new DataHisto();
           

            Console.WriteLine("Simulation lancée avec : ");
            Console.WriteLine("K = " + strike + "€");
            Console.WriteLine("Echeance " + maturite.ToString());
          
            
            Console.WriteLine("Action selectionnée " + action.Name);          
            DateTime finEstimation = new DateTime(2010,1,1);

            VanillaCall vanille = new VanillaCall("option", action, maturite, donnes.strike);
            OptionVanille option = new OptionVanille(vanille);




            List<DataFeed> data = histo.getData(donnes);
            //Actualisation de la valeur du portefeuille
            
           /* PorteFeuilleVanille porteFeuilleVanille = new PorteFeuilleVanille(option);
            porteFeuilleVanille.actulisationPorteSimu(data, donnes);   
            int i = 0;
            DateTime date = donnes.debutSimulation;
            while (date <=  donnes.finSimulation)
            {
                i  = porteFeuilleVanille.dateTimeConverter(donnes.debutSimulation, date);
                Console.WriteLine("le call vaut : " + pricer.PriceCall(vanille, date, 365,(double) data[i].PriceList[vanille.UnderlyingShare.Id], 0.4).Price);
                Console.WriteLine("i vaut :" + i);
                Console.Write("le spot vaut : " + data[i].PriceList[vanille.UnderlyingShare.Id]);
                Console.WriteLine("le portefeuille vaut : ");
                Console.WriteLine(porteFeuilleVanille.ToString1(donnes.debutSimulation,date));
                Console.WriteLine("Tracking Error = " + porteFeuilleVanille.trackingErrors[i]  +" à la date " + date.ToString());
                date=date.AddDays(donnes.pas);
            }*/

            List<String> sousjacents = new List<string>() { "accor"};
            entree = new Entrees(Entrees.typeOption.Vanille
                , 9, new DateTime(2009, 1, 1)
                , sousjacents
                , new DateTime(2012, 1, 1)
                , new DateTime(2009, 1, 1)
                , new DateTime(2012, 1, 1)
                , 50
                , Entrees.typeDonnees.Simulees
                , "optionTest"
                , new List<double>() {1});

            Simulation sim = new Simulation(entree);
            sim.Lancer();
            option = sim.PrixOption;
            portefeuille = sim.valeurPf;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Simulation sim = new Simulation(entree);
            sim.Lancer();
            option = sim.PrixOption;
            portefeuille = sim.valeurPf;
            //Window_Loaded();

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

            PointCollection points = new PointCollection();
            var x2 = margin;
            Console.WriteLine("il y a " + option.Count + "nombre d element dans la liste ");

            foreach (double val in option)
            {
                points.Add(new Point(x2, ymax -val*20 ));
                x2= x2 +  (int)(xmax / (int)option.Count);
            }
            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = brushes[0];
            polyline.Points = points;

            canGraph.Children.Add(polyline);

            PointCollection points2 = new PointCollection();
            x2 = margin;
            foreach (double val in portefeuille)
            {
                points2.Add(new Point(x2, ymax - val * 20));
                x2 = x2 + (int)(xmax / (int)portefeuille.Count);
            }
            Polyline polyline2 = new Polyline();
            polyline2.StrokeThickness = 1;
            polyline2.Stroke = brushes[1];
            polyline2.Points = points2;

            canGraph.Children.Add(polyline2);

            //for (int data_set = 0; data_set < 3; data_set++)
            //{
            //    int last_y = rand.Next((int)ymin, (int)ymax);

            //    PointCollection points = new PointCollection();
            //    for (double x = xmin; x <= xmax; x += step)
            //    {
            //        last_y = rand.Next(last_y - 10, last_y + 10);
            //        points.Add(new Point(x, last_y));
            //    }

            //    Polyline polyline = new Polyline();
            //    polyline.StrokeThickness = 1;
            //    polyline.Stroke = brushes[data_set];
            //    polyline.Points = points;

            //    canGraph.Children.Add(polyline);
            //}
        }
    }
}
