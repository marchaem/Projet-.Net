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
    public class DataHisto : AbstractData
    {
        [DllImport("wre-modeling-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]

        public static extern int WREmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );
        public override List<DataFeed> getData(Entrees input)
        {
            List<DataFeed> DataFieldCal = new List<DataFeed>();
            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();
            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().Where(el=>el.Date >= input.debutSimulation).Where(el => el.Date <= input.finSimulation ).ToList();
            reqDate.Sort();
            List<decimal> reqValue = asdc.HistoricalShareValues.Select(el => el.value).ToList();
            int i = 0;
            int j = 0;
            foreach(DateTime date in reqDate)
            {
              //  Console.WriteLine("les dates sont :" + date.ToString());
                Dictionary<string, decimal> dico = new Dictionary<string, decimal>();
                var list = asdc.HistoricalShareValues.Where(el => el.date == date).Select(el => el.id );
                i++;
                foreach (string id in list)
                {
                    
                    var valeur = asdc.HistoricalShareValues.Where(el => el.date == date).Where(el => el.id == id).Select(el => el.value).First();
                    Console.WriteLine("on a l'action " + id.ToString()+" au spot de : "+valeur.ToString()+"à la date "+ date.ToString());
                    dico.Add(id.Trim(), valeur);
                    j++;
                }
             //   Console.WriteLine("il y a " + j + "actions à la date i");
                DataFeed dataFeed = new DataFeed(date, dico);
                DataFieldCal.Add(dataFeed);
                j = 0;
            }
          //  Console.WriteLine("il y a " + i + "date");
            return DataFieldCal;
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
        public double[,] getAssetreturns(Entrees input)
        {
            
            List<DataFeed> data = this.getData(input);
            int nbAction = input.listActions.Count;
            Console.WriteLine("il y a " + nbAction.ToString() + " Actions dans input.listActions");

            int nbdate = DayCount.CountBusinessDays(input.debutSimulation,input.finSimulation)/input.pas  ; 
            int reste = ((input.finSimulation - input.debutSimulation).Days) % input.pas;
            int result = (reste == 0) ? 0 : 1;
            nbdate += result;
            double[,] Assetreturns = new double[nbdate,nbAction] ;
            int indexDebut = data.FindIndex(el => el.Date == input.debutSimulation);
            int indexFin = data.FindIndex(el => el.Date == input.finSimulation);
            double res = 0.0;
            for (int j=0; j < nbAction;j++ )
            {
                for(int i =0; i <  nbdate-1; i = i + input.pas)
                {
                    Console.WriteLine("notre priceList a pour nbdr d'éléments :" + data[indexDebut + i * input.pas].PriceList.Count.ToString());
                    Console.WriteLine("La"+j.ToString() +" action a pour Id " + input.listActions[j]);
                    Assetreturns[i, j] = (double)data[indexDebut + i * input.pas].PriceList[input.listActions[j]];

                }
                 Assetreturns[nbdate-1,j] = res = (double)data[indexFin].PriceList[input.listActions[j]];

            }
            return Assetreturns;
        }
    }
}
