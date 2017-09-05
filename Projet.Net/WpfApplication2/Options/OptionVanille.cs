using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    class OptionVanille
    {
        private VanillaCall vanillaCall;
        private double prix;


        public double calculePrixVanille(VanillaCall vanille, System.DateTime date,int nbJourParAn, double spot,double volatility)
        {
            Pricer pricer = new Pricer();
            return pricer.PriceCall(vanille, date, nbJourParAn, spot, volatility).Price;
        }
    }
}
