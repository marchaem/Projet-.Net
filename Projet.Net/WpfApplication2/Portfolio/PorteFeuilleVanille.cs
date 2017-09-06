using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Portfolio
{
    public class PorteFeuilleVanille : Portefeuille
    {
        private Share action { get; set; }
        private double nombreAction { get; set; }
        private double thuneDansLeSansRisque { get; set; }

        public PorteFeuilleVanille(Share action, double nombreAction, double prixCall, double spot) 
        {
            this.action = action;
            this.nombreAction = nombreAction;
            this.thuneDansLeSansRisque = prixCall - (nombreAction*spot);
        }

        public override string ToString()
        {
            var res = "";
            res = "Composition du portefeuille : " 
                + nombreAction 
                + " actions " 
                + action.Name 
                + " et " 
                + thuneDansLeSansRisque 
                + " € dans le sans risque";
            return res;
        }

        public double getValeurActu(int nbPeriodes, double spot, double tauxSansRisque)
        {
            // spot = prix de l'action au bout de nbPeriodes
            var res = 0.0;
            res = thuneDansLeSansRisque * Math.Pow(1 + tauxSansRisque, nbPeriodes) + spot * nombreAction;
            return res;
        }
    }
}
