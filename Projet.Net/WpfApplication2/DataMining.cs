using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WpfApplication2;

namespace dataMining
{
    class Program
    {
        static void Main2(string[] args)
        {
            Console.WriteLine("Démarrer");
            Console.ReadLine();
            
            DataHistoriqueDataContext asdc = new DataHistoriqueDataContext();

            List<string> reqId = asdc.HistoricalShareValues.Select(el => el.id).Distinct().ToList();
            List<System.DateTime> reqDate = asdc.HistoricalShareValues.Select(el => el.date).Distinct().ToList();
            List<decimal> reqValue = asdc.HistoricalShareValues.Select(el => el.value).Distinct().ToList();


            for (int i = 0; i < reqId.Count; ++i)
            {
                Console.WriteLine("ID: " + i + " " + reqId[i]);
                Console.WriteLine("Date: " + reqDate[i]);
                Console.WriteLine("Value: " + reqValue[i]);
            }

        }


       

        static void LinqSQL()
        {
            Console.WriteLine("Récupération à l'aide de LINQ; syntaxe à la SQL");
            using (DataHistoriqueDataContext asdc = new DataHistoriqueDataContext())
            {
                var q2 = (from lignes in asdc.HistoricalShareValues
                          select lignes.id).Distinct();
                foreach (string nom in q2)
                {
                    Console.WriteLine("Nom: {0}", nom);
                }
            }
            Console.ReadLine();
        }


        static void LinqLambda()
        {
            Console.WriteLine("Récupération à l'aide de LINQ; syntaxe 'lambda-calcul'");
            using (DataHistoriqueDataContext asdc = new DataHistoriqueDataContext())
            {
                var q3 = asdc.HistoricalShareValues.Select(ligne => ligne.id).Distinct();

                foreach (string nom in q3)
                {
                    Console.WriteLine("Nom: {0}", nom);
                }
                Console.ReadLine();
            }
        }

    }
}
