﻿using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApplication2.Portfolio;
using WpfApplication2.Options;

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
            
            //getShareName();
         //   DateTime date1 = new DateTime(1,1,5);
          //  var a = dateTimeConverter(date1);
        //    Console.WriteLine("la date vaux : "+a);
            /*Donnees a demander a l'utilisateur*/

          //  DateTime debut = new DateTime(2009, 01, 01);
            DateTime maturite = new DateTime(2010, 10, 10);
            double strike = 9.0;

            double tauxSansRisque = 0.01;

            Console.WriteLine("Simulation lancée avec : ");
            Console.WriteLine("K = " + strike + "€");
            Console.WriteLine("Echeance " + maturite.ToString());
          //  Console.WriteLine("Date Courrante " + debut.ToString());

            bool simule = true;

            /*On rentre en dur la valeur de l'action pour l'instant*/
            Share action = new Share("accor", "accordId");
            Console.WriteLine("Action selectionnée " + action.Name);

            
            DateTime finEstimation = new DateTime(2010,1,1);

            /*Fin entree des donnees*/

            /*Creation de l'option*/

            VanillaCall vanille = new VanillaCall("option", action, maturite, strike);
            OptionVanille option = new OptionVanille(vanille);

            /*Calcul de la valeur du payoff*/
            var dataFeedCalc = new List<DataFeed>();
            

           
            
            

            /*Calcul du portefeuille de couverture*/

            Pricer pricer = new Pricer();

            /*  var result = pricer.PriceCall(vanille, maturite, 366, (double) dataFeedCalc[dataFeedCalc.Count-1].PriceList[vanille.UnderlyingShare.Id], 0.4);
              Console.WriteLine("Le delta à la fin vaut : " + result.Deltas[0]);
              Console.WriteLine("Le prix du call à l'écheance : " + result.Price);

              var resultdebut = pricer.PriceCall(vanille, debut, 366, (double)dataFeedCalc[0].PriceList[vanille.UnderlyingShare.Id], 0.4);
              Console.WriteLine("Le delta au début vaut : " + resultdebut.Deltas[0]);
              Console.WriteLine("Le prix du call au début : " + resultdebut.Price);
              */


            //Actualisation de la valeur du portefeuille
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(vanille, new DateTime(2000,1,1));
            PorteFeuilleVanille porteFeuilleVanille = new PorteFeuilleVanille(option);
            Console.WriteLine(porteFeuilleVanille.ToString());
            DateTime[] date = new DateTime[] { new DateTime(2000,1,1), new DateTime(2000, 1, 2), new DateTime(2000, 1, 3), new DateTime(2000, 1, 4), new DateTime(2000, 1, 5) };
            double payoff = vanille.GetPayoff(dataFeedCalc[dataFeedCalc.Count - 1].PriceList);

            int kerboul;
            double trackingError;
            int i = 0;
            
            foreach (DateTime dates in date)
            {
                kerboul = porteFeuilleVanille.dateTimeConverter(date[0], dates);
                porteFeuilleVanille.actualisationPortef(date[0], dates, (double) dataFeedCalc[i].PriceList[vanille.UnderlyingShare.Id],0.4,tauxSansRisque);
                Console.Write("le spot vaut : " + dataFeedCalc[i].PriceList[vanille.UnderlyingShare.Id]);
                payoff = pricer.PriceCall(vanille, dates, 365, (double)dataFeedCalc[i].PriceList[vanille.UnderlyingShare.Id], 0.4).Price;
                trackingError = porteFeuilleVanille.pricePortefeuille(date[0], dates, tauxSansRisque, (double)dataFeedCalc[dataFeedCalc.Count - 1].PriceList[vanille.UnderlyingShare.Id], 0.4) - payoff;
                Console.WriteLine("Tracking Error = " + trackingError +" à la date " + dates.ToString());
                i++;
            }
           

            // Calcul de la tracking error
            
            



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
    }
}
