using PricingLibrary.Utilities;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WpfApplication2.Entree;
namespace WpfApplication2.Data
{
    static class Compute_Tools
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


        //returns the matrix with the asset's values (spots)
        static public double[,] getAssetValues(Entrees input)
        {
            DataSimu histo = new DataSimu();
            List<DataFeed> data = histo.getData(input);
            int nbAction = input.listActions.Count;
            Console.WriteLine("il y a " + nbAction.ToString() + " Actions dans input.listActions");
            int nbdate = DayCount.CountBusinessDays(input.debutSimulation, input.maturite) / input.pas;
            int reste = ((input.maturite - input.debutSimulation).Days) % input.pas;
            int result = (reste == 0) ? 0 : 1;
            nbdate += result;
            double[,] Assetreturns = new double[nbdate, nbAction];
            int indexDebut = data.FindIndex(el => el.Date == input.debutSimulation);
            int indexFin = data.FindIndex(el => el.Date == input.maturite);
            Console.WriteLine("Index de début : " + indexDebut + " index de fin : " + indexFin);
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


        //compute the covariance matrix
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


        //compute the matrix with the Assets returns
        public static double[,] getAssetReturns(double[,] returns)
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


        //returns the array with all the volatilities
        public static double[] tabVolatilite(double[,] covMatrix)
        {
            double[] tab = new double[covMatrix.GetLength(0)];
            for (int i = 0; i < covMatrix.GetLength(0); i++)
            {
                tab[i] = covMatrix[i, i]*Math.Sqrt(365);
            }
            return tab;
        }

        public static void dispMatrix(double[,] myCovMatrix)
        {
            int n = myCovMatrix.GetLength(0);
            int m = myCovMatrix.GetLength(1);

            Console.WriteLine("Covariance matrix:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(myCovMatrix[i, j] + "\t");
                }
                Console.Write("\n");
            }
        }
    }
}
    

