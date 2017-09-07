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
        public double strike { get; set; }
        public DateTime dateDebut { get; set; }
        public List<Share> listActions { get; set; }
        public DateTime maturite { get; set; }
        public DateTime debutSimulation { get; set; }
        public DateTime finSimulation { get; set; }
        public int pas { get; set; }
        public enum typeDonnees { Historique, Simulees}
        public typeDonnees typedonnees;

        public Entrees(typeOption type,double strike, DateTime dateDebut,List<Share> liste, DateTime maturite, DateTime debutSimulation, DateTime finSimulation, int pas, typeDonnees typeDonnees)
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
