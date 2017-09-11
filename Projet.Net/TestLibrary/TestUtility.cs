using System;
using System.Collections.Generic;
using System.Text;
using WpfApplication2.Simu;
using WpfApplication2.Entree;
using System.Runtime.InteropServices;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApplication2.Data;
using WpfApplication2.Data;

namespace TestLibrary
{
    [TestClass]
   public class TestUtility
    {
        [TestMethod]
        public void TestMain()
        {
            /*List<String> sousjacents = new List<string>(){ "accor", "bnp" };
            Entrees entree = new Entrees(Entrees.typeOption.Basket
                , 9, new DateTime(2009, 1, 1)
                , sousjacents
                , new DateTime(2010, 1, 1)
                , new DateTime(2009, 1, 1)
                , 10
                , Entrees.typeDonnees.Simulees
                , "optionTest"
                ,new List<double>() { 0.7, 0.3 });
            Simulation sim = new Simulation(entree);*/

            List<String> sousjacent = new List<string>() { "EDF FP", "BNP FP" };
            Entrees entree = new Entrees(Entrees.typeOption.Basket
                , 9, new DateTime(2009, 1, 1)
                , sousjacent
                , new DateTime(2011, 1, 1)
                , new DateTime(2009, 1, 1)
                , 100
                , typeDonnees.typedonnees.Simulees
                , "optionTest"
                , new List<double>() {0.5,0.5});
            Simulation sim = new Simulation(entree);
            DataSimu dataSimu = new DataSimu(Simulation.CreerOption(entree));
            dataSimu.genereData(entree.debutSimulation);

            //sim.Lancer();
           // DataHisto data = new DataHisto(Simulation.CreerOption(entree));
            //data.genereData(entree.dateDebut);
            //Console.WriteLine("Vol = " + data.vol(0)[1]);
            //List<DataFeed> dataSimul = data.genereData(entree);
            //DataHisto.matriceCovariance(entree, dataSimul);

            //DataHisto data = new DataHisto();
            //data.getData(entree);
            //Compute_Tools.getAssets(dataSimul, entree);
        }
    }
}
