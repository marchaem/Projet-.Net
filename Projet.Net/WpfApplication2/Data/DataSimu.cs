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

namespace WpfApplication2.Data
{
    public class DataSimu : AbstractData
    {

  
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingLogReturns", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREmodelingLogReturns(
        ref int nbValues,
        ref int nbAssets,
        double[,] assetsValues,
        ref int horizon,
        double[,] assetsReturns,
        ref int info
            );

        public DataSimu(Options.Option share)
        {
            this.option = share;
        }
        public override List<DataFeed> getData(Entrees input)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(option.option,input.debutSimulation);
            return dataFeedCalc;
        }



        public double[,] getAssetValues(Entrees input)
        {

            List<DataFeed> data = this.getData(input);
            int nbAction = input.listActions.Count;
            Console.WriteLine("il y a " + nbAction.ToString() + " Actions dans input.listActions");

            int nbdate = DayCount.CountBusinessDays(input.debutSimulation, input.maturite) / input.pas;
            int reste = ((input.maturite - input.debutSimulation).Days) % input.pas;
            int result = (reste == 0) ? 0 : 1;
            nbdate += result;
            double[,] Assetreturns = new double[nbdate, nbAction];
            int indexDebut = data.FindIndex(el => el.Date == input.debutSimulation);
            int indexFin = data.FindIndex(el => el.Date == input.maturite);
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

        public static double[,] computeCov(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int res;
            res = WREmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return covMatrix;
        }

        public double[,] getAssetReturns(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbValues = returns.GetLength(1);
            double[,] assetReturns = new double[dataSize, nbValues];
            int info = 0;
            int horizon = 1;
            int res;
            res = WREmodelingLogReturns(ref dataSize, ref nbValues, returns, ref horizon, assetReturns, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return assetReturns;
        }
    }
}
