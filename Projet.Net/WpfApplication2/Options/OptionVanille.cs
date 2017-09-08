using PricingLibrary.Computations;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Options
{
    class OptionVanille : Option
    {
        public OptionVanille(PricingLibrary.FinancialProducts.Option option) : base(option)
        {
        }

        public override double[] CalculerDeltas(int jour, DateTime dateDebut, double[] vol, double[,] cov, double[] spot)
        {
            Pricer pricer = new Pricer();
            DateTime dateAvancee = dateDebut;
            dateAvancee = dateAvancee.AddDays(jour);
            double[] deltas = pricer.PriceCall((VanillaCall)this.option, dateAvancee, 365, spot[0], vol[0]).Deltas;
            return deltas;
        }

        public override double CalculerPrix(int jour, List<DataFeed> donees, DateTime dateDebut, double[] spot, double[,] cov, double[] vol)
        {
            Pricer pricer = new Pricer();
            //double volatility = 0.4; 
            DateTime dateAvancee = dateDebut;
            dateAvancee = dateAvancee.AddDays(jour);
            double prix = pricer.PriceCall( (VanillaCall) this.option, dateAvancee, 365, spot[0], vol[0]).Price;
            return prix;
        }

        public override int GetNbSousJacents()
        {
            return 1;
        }
    }
}
