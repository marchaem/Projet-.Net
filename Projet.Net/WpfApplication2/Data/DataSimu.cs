using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Utilities.MarketDataFeed;
using WpfApplication2.Entree;
using PricingLibrary.Utilities;
using PricingLibrary.FinancialProducts;
using System.Runtime.InteropServices;
using WpfApplication2.Options;

namespace WpfApplication2.Data
{
    public class DataSimu : AbstractData
    {
        public DataSimu()
        {

        }

        public DataSimu(Options.Option share)
        {
            this.option = share;
        }

        public override List<DataFeed> getData(Entrees input)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(Simu.Simulation.CreerOption(input).option,input.dateDebut);
            return dataFeedCalc;
        }

        public override double[] vol(Options.Option option, DateTime date)
        {
            double[] vol = new double[option.GetNbSousJacents()];
            for (int i = 0; i < option.GetNbSousJacents(); i++)
            {
                vol[i] = 0.4;
            }
            return vol;
        }

        public override double[,] cov(Options.Option option, DateTime date)
        {
            double[,] cov = new double[option.GetNbSousJacents(), option.GetNbSousJacents()];
            for (int i = 0; i < option.GetNbSousJacents(); i++)
            {
                for (int j = 0; j < option.GetNbSousJacents(); j++)
                {
                    if (i == j)
                    {
                        cov[i, j] = 0.4;
                    }
                    else
                    {
                        cov[i, j] = 0.1;
                    }
                }
            }
            return cov;
        }
    }
}
