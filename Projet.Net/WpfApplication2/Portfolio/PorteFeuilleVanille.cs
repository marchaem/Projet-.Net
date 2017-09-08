using PricingLibrary.Computations;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Options;
using WpfApplication2.Parametres;

namespace WpfApplication2.Portfolio
{
    public class PorteFeuilleVanille : Portefeuille
    {
        private OptionVanille option { get; set; }

        public List<double> trackingErrors { get; }

        // proportions est matrice qui à une date (ligne ) associe les proportions (colonnes)
        private double[,] proportions;// composition portefeuille

        public PorteFeuilleVanille(OptionVanille option)
        {
            this.option = option;
            this.proportions = new double[1000000,2]; // à améliorer
            this.trackingErrors = new List<double>();
        }
        public void actualisationPortef(DateTime debutSimulation,DateTime date,double spot,double volatility,double couponCouru)//actualisationSimulation
        {
            int a = dateTimeConverter(debutSimulation,date);
            
            if (a < 0)
            {
                
                throw new Exception("abruti rentre des bonnes dates");
            }
            double delta = option.calculDeltaVanille(date, 365, spot, volatility);
            double thuneSansRisque = pricePortefeuille(debutSimulation,date, couponCouru, spot, volatility)-delta*spot;
            proportions[a, 0] = delta;
            proportions[a, 1] = thuneSansRisque;
            trackingErrors.Add ( pricePortefeuille(debutSimulation, date, couponCouru, spot, volatility) - option.calculePrixVanille(date, 365, spot, volatility));



        }
        

        public void actulisationPorteSimu(List<DataFeed> simulation, Entrees input)// fillCompositionPortefeuille
        {

            int i=0;
            foreach(DataFeed val in simulation)
            {
                actualisationPortef(input.debutSimulation, val.Date,(double) val.PriceList[input.listActions[0].Id], 0.4,RiskFreeRateProvider.GetRiskFreeRateAccruedValue(input.pas/365.0));
                i++;
            }
        }
        

        public double pricePortefeuille(DateTime debutEstimation, DateTime date  ,double couponCouru,double spot, double volatility)
        {
            int a = dateTimeConverter(debutEstimation,date);
            
            if (a < 0)
            {
                throw new Exception("la fonction Kerboul a renvoyé un index négatif, c'est un boyard ");
            }
            else if (a == 0)
            {
                return option.calculePrixVanille(date, 365, spot, volatility);
            }
            else
            {
                return proportions[a - 1, 0] * spot + proportions[a - 1, 1] * couponCouru;
            }



        }



        public int dateTimeConverter(DateTime debutEstimation, DateTime date)
        {
            bool datahist = false;
            if (datahist)
            {
                return 0;
            }

            DateTime dateDebut = debutEstimation;
            TimeSpan temps = date - dateDebut;
            int a = Convert.ToInt32(temps.TotalDays);
            return a;

        }

        public  string ToString1(DateTime dateDebut, DateTime date)
        {
            string message = "la composition du portefeuille est : " + this.proportions[dateTimeConverter(dateDebut, date), 0] + " et " + this.proportions[dateTimeConverter(dateDebut, date), 1];
            return message;
        }



        /*
        public double getValeurActu(int nbPeriodes, double spot, double tauxSansRisque)
        {
            // spot = prix de l'action au bout de nbPeriodes
            var res = 0.0;
            res = thuneDansLeSansRisque * Math.Pow(1 + tauxSansRisque, nbPeriodes) + spot * nombreAction;
            return res;
        }
        */
    }
}
