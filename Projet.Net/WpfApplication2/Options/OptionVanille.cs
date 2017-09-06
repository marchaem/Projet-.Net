using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Options
{
   public class OptionVanille
    {
        private VanillaCall vanillaCall;
        private double prix;

        public OptionVanille(VanillaCall vanille)
        {
            this.vanillaCall = vanille;
        }


        public double calculePrixVanille( System.DateTime date,int nbJourParAn, double spot,double volatility)
        {
            Pricer pricer = new Pricer();
            return pricer.PriceCall(this.vanillaCall, date, nbJourParAn, spot, volatility).Price;
        }
    }
}
