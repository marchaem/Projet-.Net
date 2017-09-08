using System;
using PricingLibrary.FinancialProducts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Entree
{
    public class Entrees
    {
        public enum typeOption { Vanille, Basket } 
        public typeOption typeoption { get; set;}
        public double strike { get; set; }
        public DateTime dateDebut { get; set; }
        public List<String> listActions { get; set; }
        public DateTime maturite { get; set; }
        public DateTime debutSimulation { get; set; }
        public int pas { get; set; }
        public enum typeDonnees { Historique, Simulees }
        public typeDonnees typedonnees {get; set;}
        public string nomOption { get; set; }
        public List<double> listePoids { get; set; }

        public Entrees(typeOption type, double strike, DateTime dateDebut,List<String> liste, DateTime maturite, DateTime debutSimulation, int pas, typeDonnees typeDonnees, string nomOption, List<double> poids)
        {
            this.typeoption = type;
            this.strike = strike;
            this.dateDebut = dateDebut;
            this.listActions = liste;
            this.maturite = maturite;
            this.debutSimulation = debutSimulation;
            this.pas = pas;
            this.typedonnees = typeDonnees;
            this.nomOption = nomOption;
            this.listePoids = poids;
        }
    }
}
