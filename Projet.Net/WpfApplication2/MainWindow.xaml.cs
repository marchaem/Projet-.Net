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

        
    }

}
