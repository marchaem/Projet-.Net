using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PricingLibrary.FinancialProducts;
using System.Threading.Tasks;

namespace WpfApplication2.Mesure
{
    public class Mesure
    {
        private Share action { get; set; }
        private double valeur { get; set; }
        private DateTime date { get; set; }

        public Mesure(Share action, double valeur, DateTime date)
        {
            this.action = action;
            this.valeur = valeur;
            this.date = date;
        }

        public Mesure(string name, DateTime date)
        {

        }


}
}
