using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PricingLibrary.FinancialProducts;

namespace WpfApplication2.Portfolio
{
   public abstract class Portefeuille
    {
        //[DllImport("wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]
       
        
        public double getPrixPorteFeuille()
        {
            //   nombreAction*prix de l'action
            return 0.0;
        } 

    }
}
