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
        abstract public List<DataFeed> getData(Entrees input);
    }
}
