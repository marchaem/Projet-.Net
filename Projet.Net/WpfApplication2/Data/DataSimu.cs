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

        public DataSimu(Options.Option option)
        {
            this.option = option;
        }


        public override List<DataFeed> genereData(DateTime debut)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(this.option.option, debut);
            this.donnees = dataFeedCalc;
            this.listeDate = new List<DateTime>();
            var dates = new List<DateTime>();
            for (var dt = debut; dt <= option.option.Maturity; dt = dt.AddDays(1))
            {
                this.listeDate.Add(dt);
            }
            return dataFeedCalc;
        }

        public override double[] vol(int date)
        {
            double[] vol = new double[option.GetNbSousJacents()];
            for (int i = 0; i < option.GetNbSousJacents(); i++)
            {
                vol[i] = 0.4;
            }
            return vol;
        }

        public override double[,] cov(int date)
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
                        cov[i, j] = 0.1; // Valeur jamais utilisée 
                    }
                }
            }
            return cov;
        }

        public override double[,] corr(int date)
        {
            double[,] cov = new double[option.GetNbSousJacents(), option.GetNbSousJacents()];
            for (int i = 0; i < option.GetNbSousJacents(); i++)
            {
                for (int j = 0; j < option.GetNbSousJacents(); j++)
                {
                    if (i == j)
                    {
                        cov[i, j] = 1;
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
