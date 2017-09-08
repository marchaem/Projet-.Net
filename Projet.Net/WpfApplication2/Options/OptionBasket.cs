using PricingLibrary.Computations;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Options;
using PricingLibrary.FinancialProducts;

namespace WpfApplication2.Options
{
    public class OptionBasket : Option 
    {
        public OptionBasket(PricingLibrary.FinancialProducts.Option option) : base(option)
        {
        }

        public override double[] CalculerDeltas(int jour, DateTime dateDebut, double[] vol, double[,] cov, double[] spot)
        {
            Pricer pricer = new Pricer();
            DateTime dateAvancee = dateDebut;
            dateAvancee = dateAvancee.AddDays(jour);
            double[] deltas = pricer.PriceBasket((BasketOption)this.option, dateAvancee, 365, spot, vol, cov).Deltas;
            return deltas;
        }

        public override double CalculerPrix(int jour, List<DataFeed> donees, DateTime dateDebut, double[] spot, double[,] cov, double[] vol)
        {
            Pricer pricer = new Pricer();
            //double[] volatility = new double[] { 0.4, 0.4 };
            //double[,] matrice = new double[,] { { 0.4, 0.1 }, { 0.1, 0.4 } };
            DateTime dateAvancee = dateDebut;
            dateAvancee = dateAvancee.AddDays(jour);
            double prix = pricer.PriceBasket((BasketOption) this.option, dateAvancee, 365, spot, vol, cov).Price;
            return prix;
        }

        public override int GetNbSousJacents()
        {
            //  throw new NotImplementedException();
            return 1;
        }
    }
}
