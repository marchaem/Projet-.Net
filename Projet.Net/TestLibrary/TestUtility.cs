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

            List<String> sousjacent = new List<string>() { "accor" };
            Entrees entree = new Entrees(Entrees.typeOption.Vanille
                , 9, new DateTime(2009, 1, 1)
                , sousjacent
                , new DateTime(2012, 1, 1)
                , new DateTime(2009, 1, 1)
                , 100
                , Entrees.typeDonnees.Simulees
                , "optionTest"
                , new List<double>() {1});
            Simulation sim = new Simulation(entree);
            //sim.Lancer();

            DataHisto data = new DataHisto();
            data.getData(entree);

        }
    }
}
