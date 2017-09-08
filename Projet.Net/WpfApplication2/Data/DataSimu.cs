using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Utilities.MarketDataFeed;
using WpfApplication2.Parametres;

namespace WpfApplication2.Data
{
    public class DataSimu : AbstractData
    {
       
        public override List<DataFeed> getData(Entrees input)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(option,input.debutSimulation);
            return dataFeedCalc;
        }
    }
}
