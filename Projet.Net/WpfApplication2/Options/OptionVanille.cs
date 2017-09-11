using PricingLibrary.Computations;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Data;

namespace WpfApplication2.Options
{
    class OptionVanille : Option
    {
        public OptionVanille(PricingLibrary.FinancialProducts.Option option) : base(option)
        {
        }

        public override double[] CalculerDeltas(int jour, AbstractData donnees, double[] vol, double[,] corr, double[] spot)
        {
            Pricer pricer = new Pricer();
            DateTime  dateAvancee = donnees.listeDate[jour];
            double[] deltas = pricer.PriceCall((VanillaCall)this.option, dateAvancee, 365, spot[0], vol[0]).Deltas;
            return deltas;
        }

        public override double CalculerPrix(int jour, AbstractData donnees, double[] spot, double[,] corr, double[] vol)
        {
            Pricer pricer = new Pricer();
            DateTime dateAvancee = donnees.listeDate[jour];
            double prix = pricer.PriceCall( (VanillaCall) this.option, dateAvancee, 365, spot[0], vol[0]).Price;
            return prix;
        }

        public override int GetNbSousJacents()
        {
            return 1;
        }
    }
}
