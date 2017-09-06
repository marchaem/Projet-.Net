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

        public PorteFeuilleVanille(OptionVanille option, double[,] proportions)
        {
            this.option = option;
            this.proportions = proportions;
        }
        public void actualisationPortef(DateTime date,double spot,double volatility,double r, double prixSousJ)
        {
            int a = fonctionkerboul(date);
            double delta = option.calculDeltaVanille(date, 365, spot, volatility);
            double thuneSansRisque = pricePortefeuille(date, prixSousJ, r, spot, volatility);
            proportions[a, 0] = delta;
            proportions[a, 1] = thuneSansRisque;
        }
        
        int fonctionkerboul(DateTime date)
        {
            return 1;
        }
        public double pricePortefeuille(DateTime date , double prixSousJ ,double r,double spot, double volatility)
        {
            int a = fonctionkerboul(date);
            if (a < 0)
            {
                throw new Exception("la fonction Kerboul a renvoyé un index négatif, c'est un boyard ");
            }
            if (a == 0)
            {
                return option.calculePrixVanille(date, 365, spot, volatility);
            }
            return proportions[a-1,0] * prixSousJ + proportions[a-1,1] * (1 + r); 



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
