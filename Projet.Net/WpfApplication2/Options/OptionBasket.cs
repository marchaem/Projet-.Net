using PricingLibrary.Computations;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Options;
using PricingLibrary.FinancialProducts;
using WpfApplication2.Data;

namespace WpfApplication2.Options
{
    public class OptionBasket : Option 
    {
        public OptionBasket(PricingLibrary.FinancialProducts.Option option) : base(option)
        {
        }

        public override double[] CalculerDeltas(int jour, AbstractData donnees, double[] vol, double[,] corr, double[] spot)
        {
            Pricer pricer = new Pricer();
            DateTime dateAvancee = donnees.listeDate[jour];
            double[] deltas = pricer.PriceBasket((BasketOption)this.option, dateAvancee, 365, spot, vol, corr).Deltas;
            return deltas;
        }

        public override double CalculerPrix(int jour, AbstractData donnees, double[] spot, double[,] corr, double[] vol)
        {
            Pricer pricer = new Pricer();
            DateTime dateAvancee = donnees.listeDate[jour];
            double prix = pricer.PriceBasket((BasketOption) this.option, dateAvancee, 365, spot, vol, corr).Price;
            return prix;
        }

        public override int GetNbSousJacents()
        {
            return option.UnderlyingShareIds.Count();
        }
    }
}
