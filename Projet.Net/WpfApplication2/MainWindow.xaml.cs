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
using System.Linq;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 
 
    public partial class MainWindow : Window
    {

        public List<Options.Option> listOption { get; set; }
        static DateTime debutEstimation = new DateTime(2009, 1, 1);
        private ChartValues<double> option;
        private ChartValues<double> portefeuille;
        private List<ChartValues<double>> action;
        public Entrees entree;
        public string maturity { get; set; }

        public ICommand ClickCommand { get; private set; }





        public MainWindow()
        {

        }


        private void Button_Start(object sender, RoutedEventArgs e)
        {
            DateTime mat = DateTime.Parse(MaturiteUI.Text);
            double strike = Convert.ToDouble(StrikeUI.Text);
            int pas = Convert.ToInt32(freqUI.Text);
            DateTime debut = DateTime.Parse(debutUI.Text);
            List<String> listeActions = new List<string>();
            listeActions = ActionUI.Text.Split(';').ToList();
            List<double> listePoids = new List<double>() {  };
            listePoids = PoidsUI.Text.Split(';').Select(double.Parse).ToList();
            Entree.Entrees.typeOption typeOption;
            if (typeOpionUI.Text == "Vanille")
            {
               typeOption = Entrees.typeOption.Vanille;
            }
            else if (typeOpionUI.Text == "Basket")
            {
                typeOption = Entrees.typeOption.Basket;
            }
            else
            {
                throw new Exception("Type option invalide");
            }

            typeDonnees.typedonnees typedon;

            if (TypeDonneesUI.Text == "Historique")
            {
                typedon = typeDonnees.typedonnees.Historique;
            } else if (TypeDonneesUI.Text == "Simulees")
            {
                typedon = typeDonnees.typedonnees.Simulees;
            }
            else
            {
                throw new Exception("Type donnees invalide");
            }


            this.entree = new Entrees(typeOption
                , strike, debut
                , listeActions
                , mat
                , debut
                , pas
                , typedon
                , "optionTest"
                , listePoids);
            this.entree = new Entrees(FichierUI.Text);
            Simulation sim = new Simulation(entree);
            sim.Lancer();
            option = sim.PrixOption;
            portefeuille = sim.valeurPf;
            action = sim.PrixAction;
            
            RoutedEventArgs temp = new RoutedEventArgs();
            object temp2 = new object();
            PointShapeLineExample();
            
        }

        private void Button_file(object sender, RoutedEventArgs e)
        {
            this.entree = new Entrees(FichierUI.Text);
            Simulation sim = new Simulation(entree);
            double[] res = sim.Lancer();
            option = sim.PrixOption;
            portefeuille = sim.valeurPf;
            action = sim.PrixAction;
            RoutedEventArgs temp = new RoutedEventArgs();
            object temp2 = new object();
            PointShapeLineExample();
            ErrorUI.Text = Convert.ToString(res[1]);
            PayoffUI.Text = Convert.ToString(res[0]);
        }

        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            SeriesCollection.Clear();
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

            for (int i=0; i<entree.listActions.Count; i++)
            {
                SeriesCollection.Add(new LineSeries { Title = entree.listActions[i], Values = action[i] });
            }

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void vanille_Click(object sender, RoutedEventArgs e)
        {
            ajoutVanille vanille = new ajoutVanille();
            vanille.ShowDialog();
        }

        private void basket_Click(object sender, RoutedEventArgs e)
        {
            ajoutBasket basket = new ajoutBasket();
            basket.ShowDialog();
        }

        private void retirer_Click(object sender, RoutedEventArgs e )
        {

        }

       

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
 }
