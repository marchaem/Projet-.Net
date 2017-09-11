using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Entree;

namespace WpfApplication2.Data
{
    public abstract class AbstractData
    {
        public Options.Option option;
        public List<DataFeed> donnees;
        public List<DateTime> listeDate;
        abstract public List<DataFeed> genereData(DateTime debut);
        abstract public double[] vol(int date);
        abstract public double[,] cov(int date);
        abstract public double[,] corr(int date);
        public double[] getSpotIndex(int i)
        {
            decimal[] res;
            res = this.donnees[i].PriceList.Values.ToList().ToArray();
            double[] newres = Array.ConvertAll(res, item => (double)item);
            return newres;
        }
    }
}
