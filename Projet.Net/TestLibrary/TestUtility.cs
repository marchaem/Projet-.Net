using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PricingLibrary.FinancialProducts;

using PricingLibrary.Utilities.MarketDataFeed;

namespace TestLibrary
{
   static class TestUtility
    {
        // import WRE dlls
        [DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]

        // declaration
        public static extern int WREmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );




        public static double[,] computeCovarianceMatrix(double[,] returns)
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




        public static void dispMatrix(double[,] myCovMatrix)
        {
            int n = myCovMatrix.GetLength(0);

            Console.WriteLine("Covariance matrix:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(myCovMatrix[i, j] + "\t");
                }
                Console.Write("\n");
            }
        }

        public static void getDataSimul()
        {
            Console.WriteLine("debut de la generation de data");
            var hey = new List<DataFeed>();
            DateTime abc = new DateTime();
            Share action = new Share("accor", "accordId");
            VanillaCall vanille = new VanillaCall("lol", action, abc, 10.0);
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            hey = data.GetDataFeed(vanille, abc);
            hey.ForEach(Console.WriteLine);
        }
    }
}
