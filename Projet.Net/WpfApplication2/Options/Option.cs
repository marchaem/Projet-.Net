using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Options
{
    public abstract class Option
    {
        public PricingLibrary.FinancialProducts.Option option {get; set;}

        public Option(PricingLibrary.FinancialProducts.Option option)
        {
            this.option = option;
        }

        abstract public double CalculerPrix(int jour, List<DataFeed> donees, DateTime dateDebut, double[] spot, double[,] cov, double[] vol);
        abstract public double[] CalculerDeltas(int jour, DateTime dateDebut, double[] vol, double[,] cov, double[] spot);
        abstract public int GetNbSousJacents();
        public double CalculerPayoff(List<DataFeed> donnees)
        {
            return option.GetPayoff(donnees[donnees.Count - 1].PriceList);
        }

    }
}
