using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Data;

namespace WpfApplication2.Options
{
    public abstract class Option
    {
        public PricingLibrary.FinancialProducts.Option option {get; set;}

        public Option(PricingLibrary.FinancialProducts.Option option)
        {
            this.option = option;
        }

        abstract public double CalculerPrix(int jour, AbstractData donnees, double[] spot, double[,] corr, double[] vol);
        abstract public double[] CalculerDeltas(int jour, AbstractData donnees, double[] vol, double[,] cov, double[] spot);
        abstract public int GetNbSousJacents();
        public double CalculerPayoff(List<DataFeed> donnees)
        {
            return option.GetPayoff(donnees[donnees.Count - 1].PriceList);
        }

    }
}
