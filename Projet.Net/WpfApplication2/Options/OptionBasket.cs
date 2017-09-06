using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Options
{
   public class OptionBasket
    {
        private BasketOption BasketOption;
        private double prix;

        public OptionBasket(BasketOption BasketOption)
        {
            this.BasketOption = BasketOption;
        }


        public double calculePrixBasket( System.DateTime date,int nbJourParAn, double[] spots,double[] volatilities, double[,] correlation)
        {
            Pricer pricer = new Pricer();
            var result = pricer.PriceBasket(this.BasketOption, date, nbJourParAn, spots, volatilities,correlation);
            return result.Price;
        }
    }
}
