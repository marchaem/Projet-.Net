using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Options;

namespace WpfApplication2.Portfolio
{
    public class PorteFeuilleVanille : Portefeuille
    {
        private OptionVanille option { get; set; }


        // proportions est matrice qui à une date (ligne ) associe les proportions (colonnes)
        private double[,] proportions;

        public PorteFeuilleVanille(OptionVanille option)
        {
            this.option = option;
            this.proportions = new double[1000000,2];
        }
        public void actualisationPortef(DateTime debutSimulation,DateTime date,double spot,double volatility,double r)
        {
            int a = fonctionkerboul(date);
            Console.WriteLine("a vaut " + a.ToString());
            if (a < 0)
            {
                
                throw new Exception("mon a est négatif, merci kerboul");
            }
            double delta = option.calculDeltaVanille(date, 365, spot, volatility);
            double thuneSansRisque = pricePortefeuille(debutSimulation,date, r, spot, volatility);
            proportions[a, 0] = delta;
            proportions[a, 1] = thuneSansRisque;
            
        }
        
        int fonctionkerboul(DateTime date)
        {
            return 1;
        }
        //j'ai enlevé prixSousJ car c'est la même chose que le spot
        public double pricePortefeuille(DateTime debutEstimation, DateTime date  ,double r,double spot, double volatility)
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
                return proportions[a - 1, 0] * spot + proportions[a - 1, 1] * (1 + r);
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
