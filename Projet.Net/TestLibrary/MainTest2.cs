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
            DateTime abc = new DateTime();
            Share action = new Share("accor", "accordId");
            VanillaCall vanille = new VanillaCall("lol", action, abc, 10.0);
            OptionVanille optionVanille = new OptionVanille(vanille);
            double result = optionVanille.calculePrixVanille(abc, 365, 10.0, 20.0);
            Debug.WriteLine("le résultat vaut :" + result);


        }
    }
}
