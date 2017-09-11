using System;
using System.Collections.Generic;
using WpfApplication2.Data;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

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
        public typeDonnees.typedonnees typedonnees {get; set;}
        public string nomOption { get; set; }
        public List<double> listePoids { get; set; }

        public Entrees(){}

        public Entrees(typeOption type, double strike, DateTime dateDebut,List<String> liste, DateTime maturite, DateTime debutSimulation, int pas, typeDonnees.typedonnees typeDonnees, string nomOption, List<double> poids)
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

        public override string ToString()
        {
            var res = "";
            res += "Type Option = " + this.typeoption.ToString() + "\n";
            res += "Strike = " + this.strike +"\n";
            res += "Date Debut = " + this.dateDebut.ToString() + "\n";
            res += "Actions =";
            foreach(var action in listActions)
            {
                res += " " + action + " ";
            }
            res += "\n";
            res += "Date maturité = " + this.maturite.ToString() + "\n";
            res += "Date debut simulation = " + this.debutSimulation.ToString() + "\n";
            res += "Frequence de rebalancement =" + this.pas + "\n";
            res += "Type de données = " + this.typedonnees.ToString() + "\n";
            res += "Nom de l'option : " + this.nomOption + "\n";
            res += "Poids =";
            foreach (var poids in listePoids)
            {
                res += " " + poids.ToString() + " ";
            }
            return res;
        }

        public Entrees(string file)
        {
            var acopier = decodeJson();
            this.typeoption = acopier.typeoption;
            this.strike = acopier.strike;
            this.dateDebut = acopier.dateDebut;
            this.listActions = acopier.listActions;
            this.maturite = acopier.maturite;
            this.debutSimulation = acopier.debutSimulation;
            this.pas =  acopier.pas;
            this.typedonnees = acopier.typedonnees;
            this.nomOption = acopier.nomOption;
            this.listePoids = acopier.listePoids;


            /*string[] lines = System.IO.File.ReadAllLines(file);
            if (lines[0] == "basket")
            {
                this.typeoption = typeOption.Basket;
            } else if (lines[0] == "vanille")
            {
                this.typeoption = typeOption.Vanille;
            } else
            {
                throw new Exception("Impossible de parser le type de l'option");
            }
            this.strike = Convert.ToDouble(lines[1]);
            this.dateDebut = DateTime.Parse(lines[2]);
            this.listActions = new List<string>(lines[3].Split(',').ToList());
            Console.WriteLine("LONGUEUR = " + this.listActions.Count);
            this.maturite = DateTime.Parse(lines[4]);
            this.debutSimulation = DateTime.Parse(lines[5]);
            this.pas = Convert.ToInt32(lines[6]);
            if (lines[7] == "simulees")
            {
                this.typedonnees = typeDonnees.typedonnees.Simulees;
            }
            else if (lines[7] == "historique")
            {
                this.typedonnees = typeDonnees.typedonnees.Historique;
            }
            else
            {
                throw new Exception("Impossible de parser le type de donnees");
            }
            this.nomOption = lines[8];
            this.listePoids = new List<double>(lines[9].Split(';').Select(double.Parse).ToList());*/

        }
        private readonly static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            MissingMemberHandling = MissingMemberHandling.Error
        };
        public void generatJson () {
            File.WriteAllText(@"serialized.json", JsonConvert.SerializeObject(this, settings));
        }
        public Entrees decodeJson()
        {
            return JsonConvert.DeserializeObject<Entrees>(File.ReadAllText(@"serialized.json"), settings);
        }
    }
}
