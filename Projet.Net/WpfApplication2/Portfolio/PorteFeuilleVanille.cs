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


// proportions est un dictionnaire qui à une date donnée associe delta et (prix - delta)
        private Dictionary<System.DateTime, double[]> proportions;

        public PorteFeuilleVanille(OptionVanille option, Dictionary<System.DateTime,double[]> proportions)
        {
            this.option = option;
            this.proportions = proportions;
        }
        public void actualisationPortef(DateTime date,double spot,double volatility)
        {
            double nbAction = option.calculDeltaVanille(date, 365, spot,  volatility);
            double thuneSansrisque = 1; 
        }
        
        public double pricePortefeuille(System.DateTime datePrec, double prixSousJ ,double r)
        {
            double[] proportion = new double[] { 0, 0 };
            this.proportions.TryGetValue(datePrec, out proportion);
            return proportion[0] * prixSousJ + proportion[1] * (1 + r); 



        }
    }
    public double getValeurActu(int nbPeriodes, double spot, double tauxSansRisque)
    {
        // spot = prix de l'action au bout de nbPeriodes
        var res = 0.0;
        res = thuneDansLeSansRisque * Math.Pow(1 + tauxSansRisque, nbPeriodes) + spot * nombreAction;
        return res;
    }
}
