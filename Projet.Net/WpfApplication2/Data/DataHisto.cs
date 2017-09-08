using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary.Utilities.MarketDataFeed;
using WpfApplication2.Parametres;

namespace WpfApplication2.Data
{
    public class DataHisto : AbstractData
    {
        public override List<DataFeed> getData(Entrees input)
        {
            List<DataFeed> DataFieldCal = new List<DataFeed>();
            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();
            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().ToList();
            reqDate.Sort();
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
                    
                    var valeur = asdc.HistoricalShareValues.Where(el => el.date == date).Where(el => el.id == id).Select(el => el.value).First();
                    Console.WriteLine("on a l'action" + id.ToString()+"au spot de : "+valeur.ToString()+"à la date "+ date.ToString());
                    dico.Add(id, valeur);
                    j++;
                }
                Console.WriteLine("il y a " + j + "actions à la date i");
                DataFeed dataFeed = new DataFeed(date, dico);
                DataFieldCal.Add(dataFeed);
                j = 0;
            }
            Console.WriteLine("il y a " + i + "date");
            return DataFieldCal;
        }
    }
}
