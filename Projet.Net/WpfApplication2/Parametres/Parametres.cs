using System;
using PricingLibrary.FinancialProducts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Parametres
{
    public class Entrees
    {
        public enum typeOption { Vanille, Basket }
        public typeOption typeoption;
        private double strike { get; set; }
        private DateTime dateDebut { get; set; }
        private List<Share> listActions { get; set; }
        private DateTime maturite { get; set; }
        private DateTime debutSimulation { get; set; }
        private DateTime finSimulation { get; set; }
        private double pas { get; set; }
        public enum typeDonnees { Historique, Simulees}
        private typeDonnees typedonnees;

        public Parametres(typeOption type,double strike, DateTime dateDebut,List<Share> liste, DateTime maturite, DateTime debutSimulation, DateTime finSimulation, double pas, typeDonnees typeDonnees)
        {
            this.typeoption = type;
            this.strike = strike;
            this.dateDebut = dateDebut;
            this.listActions = liste;
            this.maturite = maturite;
            this.debutSimulation = debutSimulation;
            this.finSimulation = finSimulation;
            this.pas = pas;
            this.typedonnees = typeDonnees;
           

        }
    }
}
