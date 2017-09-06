using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TestLibrary;
using WpfApplication2.Portfolio;
using PricingLibrary.FinancialProducts;
using WpfApplication2.Options;

namespace TestLibrary
{
    [TestClass]
    public class Prog
    {
        [TestMethod]
        public void Main()
        {
            DateTime debut = new DateTime(2012,10,10);
            DateTime echeance = new DateTime(2013, 10, 10);
            Share action = new Share("accor", "accordId");
            VanillaCall vanille = new VanillaCall("lol", ,action, echeance, 10.0);
            OptionVanille optionVanille = new OptionVanille(vanille);
            double result = optionVanille.calculePrixVanille(debut, 365, 10.0, 0.1);
            Debug.WriteLine("le résultat vaut :" + result);
        }
    }
}
