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
    public class DataHisto : AbstractData
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

        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]

        public static extern int WREmodelingCorr(
        ref int nbValues,
        ref int nbAssets,
        double[,] assetReturns,
        double[,] corr,
        ref int info
        );

        public DataHisto(Options.Option option)
        {
            this.option = option;
        }


        public override List<DataFeed> genereData(DateTime debut)
        {
            List<DataFeed> DataFieldCal = new List<DataFeed>();
            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();
            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().Where(el=>el.Date >= debut).Where(el => el.Date <= this.option.option.Maturity ).ToList();
            reqDate.Sort();
            this.listeDate = new List<DateTime>(reqDate);
            List<decimal> reqValue = asdc.HistoricalShareValues.Select(el => el.value).ToList();
            int i = 0;
            int j = 0;
            foreach(DateTime date in reqDate)
            {
                Dictionary<string, decimal> dico = new Dictionary<string, decimal>();
                var list = asdc.HistoricalShareValues.Where(el => el.date == date).Select(el => el.id );
                i++;
                foreach (string id in list)
                {
                    if (option.option.UnderlyingShareIds.Contains(id.Trim())) {
                        var valeur = asdc.HistoricalShareValues.Where(el => el.date == date).Where(el => el.id == id).Select(el => el.value).First();
                        dico.Add(id.Trim(), valeur);
                        j++;
                    }
                }
                DataFeed dataFeed = new DataFeed(date, dico);
                DataFieldCal.Add(dataFeed);
                j = 0;
            }
            Console.WriteLine("il y a " + DataFieldCal.Count + " dates");
            this.donnees = DataFieldCal;
            return DataFieldCal;
        }

       public override double[,] cov(int date)
        {
            double[,] assetValues = this.getAssets();
            double[,] assetReturns = this.getAssetReturns(assetValues, date);
            double[,] covMatrix = this.computeCov(assetReturns);
            
            for (int i = 0; i< this.option.GetNbSousJacents(); i++)
            {
                for (int j = 0; j<this.option.GetNbSousJacents(); j++)
                {
                    covMatrix[i,j] = Math.Sqrt(covMatrix[i,j]*365);
                }
            }
            return covMatrix;
        }

        public override double[,] corr(int date)
        {
            double[,] assetValues = this.getAssets();
            double[,] assetReturns = this.getAssetReturns(assetValues, date);
            double[,] corrMatrix = this.computeCorr(assetReturns);
            return corrMatrix;
        }

        public override double[] vol(int date)
        {
            double[,] cov = this.cov(date);
            double[] vol = new double[cov.GetLength(0)];
            for (int i=0; i<cov.GetLength(0); i++)
            {
                vol[i] = cov[i, i];
            }
            return vol;
        }

        public double[,] getAssets()
        {
            int nbAction = this.option.GetNbSousJacents();
            int nbDate = this.donnees.Count;
            String[] tabActions = option.option.UnderlyingShareIds;
            double[,] Assets = new double[nbDate, nbAction];
            for (int i = 0; i < nbDate; i++)
            {
                for (int j = 0; j < nbAction; j++)
                {
                    Assets[i, j] = (double)donnees[i].PriceList[tabActions[j]];
                }
            }
            return Assets;
        }

        public double[,] computeCov(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int res;
            res = WREmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            if (res != 0)
            {
                Console.WriteLine("ERROR " + res);
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCov encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCov encountred a problem. See info parameter for more details");
            }
            return covMatrix;
        }


        public double[,] getAssetReturns(double[,] returns, int date)
        {
            int dataSize = returns.GetLength(0);
            int nbValues = returns.GetLength(1);
            int horizon = date + 1;
            if (horizon == returns.GetLength(0))
            {
                horizon = horizon - 1;
            }
            int info = 0;
            int res;
            double[,] assetReturns = new double[dataSize, nbValues];
            res = WREmodelingLogReturns(ref dataSize, ref nbValues, returns, ref horizon, assetReturns, ref info);
            if (res != 0)
            {
                Console.WriteLine("ERROR " + res);
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingLogReturns encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingLogReturns encountred a problem. See info parameter for more details");
            }
            return assetReturns;
        }

        public double[,] computeCorr(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] CorrMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int res;
            res = WREmodelingCorr(ref dataSize, ref nbAssets, returns, CorrMatrix, ref info);
            if (res != 0)
            {
                Console.WriteLine("ERROR " + res);
                if (res < 0)
                    throw new Exception("ERROR: WREmodelingCorr encountred a problem. See info parameter for more details");
                else
                    throw new Exception("WARNING: WREmodelingCorr encountred a problem. See info parameter for more details");
            }
            return CorrMatrix;
        }



    }
}
