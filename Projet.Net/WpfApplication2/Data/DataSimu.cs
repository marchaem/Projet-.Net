using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Utilities.MarketDataFeed;
using WpfApplication2.Entree;
using PricingLibrary.Utilities;
using PricingLibrary.FinancialProducts;

namespace WpfApplication2.Data
{
    public class DataSimu : AbstractData
    {
       
        public DataSimu(Option share)
        {
            this.option = share;
        }
        public override List<DataFeed> getData(Entrees input)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(option,input.debutSimulation);
            return dataFeedCalc;
        }



        public double[,] getAssetreturns(Entrees input)
        {

            List<DataFeed> data = this.getData(input);
            int nbAction = input.listActions.Count;
            Console.WriteLine("il y a " + nbAction.ToString() + " Actions dans input.listActions");

            int nbdate = DayCount.CountBusinessDays(input.debutSimulation, input.finSimulation) / input.pas;
            int reste = ((input.finSimulation - input.debutSimulation).Days) % input.pas;
            int result = (reste == 0) ? 0 : 1;
            nbdate += result;
            double[,] Assetreturns = new double[nbdate, nbAction];
            int indexDebut = data.FindIndex(el => el.Date == input.debutSimulation);
            int indexFin = data.FindIndex(el => el.Date == input.finSimulation);
            double res = 0.0;
            for (int j = 0; j < nbAction; j++)
            {
                for (int i = 0; i < nbdate - 1; i = i + input.pas)
                {
                    Console.WriteLine("notre priceList a pour nbdr d'éléments :" + data[indexDebut + i * input.pas].PriceList.Count.ToString());
                    Console.WriteLine("La" + j.ToString() + " action a pour Id " + input.listActions[j]);
                    Assetreturns[i, j] = (double)data[indexDebut + i * input.pas].PriceList[input.listActions[j]];

                }
                Assetreturns[nbdate - 1, j] = res = (double)data[indexFin].PriceList[input.listActions[j]];

            }
            return Assetreturns;
        }
    }
}
