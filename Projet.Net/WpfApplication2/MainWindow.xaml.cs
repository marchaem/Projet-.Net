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





        public MainWindow()
        {

            /*Share share1 = new Share("EDF FP", "EDF FP");
            Share share2 = new Share("BNP FP", "BNP FP");
            List<String> sousjacents = new List<string>() { "EDF FP", "BNP FP" };
            Share[] listBasket = new Share[] { share1, share2 };
            List<DataFeed> list = new List<DataFeed>();

            BasketOption bask = new BasketOption("basket", listBasket, new double[] { 0.5, 0.5 }, new DateTime(2013, 1, 10), 5.0);
            OptionBasket basket = new OptionBasket(bask);
            //    OptionVanille option = new OptionVanille(vanille);*/
            /*entree = new Entrees(Entrees.typeOption.Basket
                , 5, new DateTime(2009, 1, 1)
                , sousjacents
                , new DateTime(2010, 1, 1)
                , new DateTime(2009, 1, 1)
                , 100
                , Entrees.typeDonnees.Simulees
                , "optionTest"
                , new List<double>() { 0.7, 0.3 });
            DataHisto dataHisto = new DataHisto();
            dataHisto.getData(entree);

            list = dataHisto.getData(entree);
            double[,] valuesHisto = Compute_Tools.getAssetValues(entree);
            Compute_Tools.dispMatrix(valuesHisto);
            DataSimu dataSimu = new DataSimu(basket);
            dataSimu.getData(entree);
            double[,] valuesSimu = dataSimu.getAssetValues(entree);
            Compute_Tools.dispMatrix(valuesSimu);
            double[,] mat = dataSimu.getAssetReturns(valuesSimu);
            Compute_Tools.dispMatrix(mat);
            double[,] matrixAsset = DataSimu.computeCov(mat);
            Compute_Tools.dispMatrix(matrixAsset);*/
            //   Console.WriteLine("la vol vaut " + Math.Sqrt(matrixAsset[0,0]));*/

            //   Console.WriteLine(matrixAsset.ToString());*/

            /*Simulation sim = new Simulation(entree);
            sim.Lancer();
            option = sim.PrixOption;
            portefeuille = sim.valeurPf;*/
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<String> sousjacents = new List<string>() { "accor", "bnp" };
            Entrees entree = new Entrees(Entrees.typeOption.Basket
                , 5, new DateTime(2009, 1, 1)
                , sousjacents
                , new DateTime(2010, 1, 1)
                , new DateTime(2009, 1, 1)
                , 10
                , Entrees.typeDonnees.Simulees
                , "optionTest"
                , new List<double>() { 0.7, 0.3 });
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

    /*internal class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<actionSelection> actionList { get; private set; }

        public ICommand ClickCommand { get; private set; }

        public MainWindowViewModel()
        {
            actionList = new ObservableCollection<actionSelection>()
            {
                new actionSelection() {Name = "Axa", IsSelected = false},
                new actionSelection() {Name = "Accor", IsSelected = false},
                new actionSelection() {Name = "Bnp", IsSelected = false},
                new actionSelection() {Name = "Vivendi", IsSelected = false},
                new actionSelection() {Name = "Dexia", IsSelected = false},
                new actionSelection() {Name = "Carrefour", IsSelected = false}
            };

            ClickCommand = new DelegateCommand(ExtractComponents);

        }*/

        /*private void ExtractComponents()
        {
            foreach (var comp in actionList)
            {
                if (comp.IsSelected)
                {
                    Console.WriteLine(comp.Name);
                }
            }
        }*/
    }
