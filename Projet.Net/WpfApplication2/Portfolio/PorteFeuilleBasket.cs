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
        private uint nombreAction { get; set; }
        private double thuneDansLeSansRisque { get; set; }

        public PorteFeuilleVanille(Share action, uint nombreAction, double thune)
        {
            this.action = action;
            this.nombreAction = nombreAction;
            this.thuneDansLeSansRisque = thune;
        }
    }
}
