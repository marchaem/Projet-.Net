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
//using WpfApplication2.Parametres;
using WpfApplication2.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static DateTime debutEstimation = new DateTime(2009, 1, 1);
        private ChartValues<double> option;
        private ChartValues<double> portefeuille;
        private Entrees entree;
        public string maturity { get; set; }

        public ICommand ClickCommand { get; private set; }


        //public ObservableCollection<actionSelect> actionList { get; private set; }


        public MainWindow()
        {
            
            List<String> sousjacents = new List<string>() { "accor", "bnp" };
            entree = new Entrees(Entrees.typeOption.Basket
                , 5, new DateTime(2009, 1, 1)
                , sousjacents
                , new DateTime(2012, 1, 1)
                , new DateTime(2009, 1, 1)
                , new DateTime(2012, 1, 1)
                , 50
                , Entrees.typeDonnees.Simulees
                , "optionTest"
                , new List<double>() { 0.7, 0.3 });


            //actionList = new ObservableCollection<actionSelect>()
            //{
            //    new actionSelect() {Name = "Axa", IsSelected = false},
            //    new actionSelect() {Name = "Accor", IsSelected = false},
            //    new actionSelect() {Name = "Bnp", IsSelected = false},
            //    new actionSelect() {Name = "Vivendi", IsSelected = false},
            //    new actionSelect() {Name = "Dexia", IsSelected = false},
            //    new actionSelect() {Name = "Carrefour", IsSelected = false}
            //};

            //ClickCommand = new DelegateCommand(ExtractComponents);
        
    }
        //private void ExtractComponents()
        //{
        //    foreach (var comp in ComponentInfoList)
        //    {
        //        if (comp.IsSelected)
        //        {
        //            Console.WriteLine(comp.Name);
        //        }
        //    }
        //}


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Simulation sim = new Simulation(entree);
            sim.Lancer();
            option = sim.PrixOption;
            portefeuille = sim.valeurPf;
            RoutedEventArgs temp = new RoutedEventArgs();
            object temp2 = new object();
            //Window_Loaded(temp2, temp);
            Console.WriteLine("la maturity est de"+ maturity);
            PointShapeLineExample();

        }
        public void PointShapeLineExample()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "option",
                    Values = option
                },
                new LineSeries
                {
                    Title = "portefeuille",
                    Values = portefeuille,
                    PointGeometry = null
                },
            };

            var Values = new ChartValues<double> { 6, 7, 3, 4, 6 };
            Values.Add(10);
            Console.WriteLine(Values[5] + " c est ca les valeurs");

            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter = value => value.ToString("C");

            

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        // Draw a simple graph.
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    const double margin = 10;
        //    double xmin = margin;
        //    double xmax = canGraph.Width - margin;
        //    //double ymin = margin;
        //    double ymax = canGraph.Height - margin;
        //    const double step = 10;

        //    // Make the X axis.
        //    GeometryGroup xaxis_geom = new GeometryGroup();
        //    xaxis_geom.Children.Add(new LineGeometry(
        //        new Point(0, ymax), new Point(canGraph.Width, ymax)));
        //    for (double x = xmin + step;
        //        x <= canGraph.Width - step; x += step)
        //    {
        //        xaxis_geom.Children.Add(new LineGeometry(
        //            new Point(x, ymax - margin / 2),
        //            new Point(x, ymax + margin / 2)));
        //    }

        //    Path xaxis_path = new Path();
        //    xaxis_path.StrokeThickness = 1;
        //    xaxis_path.Stroke = Brushes.Black;
        //    xaxis_path.Data = xaxis_geom;

        //    canGraph.Children.Add(xaxis_path);

        //    // Make the Y ayis.
        //    GeometryGroup yaxis_geom = new GeometryGroup();
        //    yaxis_geom.Children.Add(new LineGeometry(
        //        new Point(xmin, 0), new Point(xmin, canGraph.Height)));
        //    for (double y = step; y <= canGraph.Height - step; y += step)
        //    {
        //        yaxis_geom.Children.Add(new LineGeometry(
        //            new Point(xmin - margin / 2, y),
        //            new Point(xmin + margin / 2, y)));
        //    }

        //    Path yaxis_path = new Path();
        //    yaxis_path.StrokeThickness = 1;
        //    yaxis_path.Stroke = Brushes.Black;
        //    yaxis_path.Data = yaxis_geom;

        //    canGraph.Children.Add(yaxis_path);

        //    // Make some data sets.
        //    Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
        //    Random rand = new Random();

        //    PointCollection points = new PointCollection();
        //    var x2 = margin;
        //    Console.WriteLine("il y a " + option.Count + "nombre d element dans la liste ");

        //    foreach (double val in option)
        //    {
        //        points.Add(new Point(x2, ymax -val*20 ));
        //        x2= x2 +  (int)(xmax / (int)option.Count);
        //    }
        //    Polyline polyline = new Polyline();
        //    polyline.StrokeThickness = 1;
        //    polyline.Stroke = brushes[0];
        //    polyline.Points = points;

        //    canGraph.Children.Add(polyline);

        //    PointCollection points2 = new PointCollection();
        //    x2 = margin;
        //    foreach (double val in portefeuille)
        //    {
        //        points2.Add(new Point(x2, ymax - val * 20));
        //        x2 = x2 + (int)(xmax / (int)portefeuille.Count);
        //    }
        //    Polyline polyline2 = new Polyline();
        //    polyline2.StrokeThickness = 1;
        //    polyline2.Stroke = brushes[1];
        //    polyline2.Points = points2;

        //    canGraph.Children.Add(polyline2);


        //}
    }
    public partial class PointShapeLineExample : UserControl
    {
      
    }
}
