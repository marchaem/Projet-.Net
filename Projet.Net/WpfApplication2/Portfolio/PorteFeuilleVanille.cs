using PricingLibrary.Computations;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Entree;

namespace WpfApplication2.Portfolio
{
    public class PorteFeuilleVanille 
    {
        private VanillaCall option { get; set; }

        double partAction;

        double partSansRisque; 

        public PorteFeuilleVanille(double spot, double delta, PorteFeuilleVanille pfPrec, double r, VanillaCall option)
        {
            this.partAction = delta;
            this.option = option;
            double valeurActu = pfPrec.actu(spot, r, pfPrec);
            this.partSansRisque = valeurActu;
            partSansRisque -= delta * spot;
        }

        public PorteFeuilleVanille(double prixOption, double delta, double spot, VanillaCall option)
        {
            // Constructeur qui créer le premier portefeuille à l'instant 0
            this.option = option;
            this.partAction = delta;
            this.partSansRisque = prixOption - delta * spot;
        }

        public double getPrixPortefeuille(double spot)
        {
            // Recupere la valeur du portefeuille à l'instant actuel
            var result = 0.0;
            result = partAction * spot + partSansRisque;
            return result;
        }

        public double actu(double spot, double r, PorteFeuilleVanille pfPrec)
        {
            // Actualise la valeur du portefeuille au temps n jusqu'au temps n+1
            // r = taux sans risque
            double anciensDelta = pfPrec.partAction;
            var valeur = 0.0;
            valeur += anciensDelta * spot;

            valeur += pfPrec.partSansRisque * r;

            return valeur;
        }

        public string ToString(double spot)
        {
            var res = "";
            res += "##########PORTEFEUILLE##########\n";
            res += "Part des actions \n";
            res += partAction + " actions " + option.UnderlyingShare.Name + " (Cours " + spot + "€ )\n";
            res += "Montant dans le sans risque : " + partSansRisque + "€\n";
            res += "Valeur totale du portefeuille = " + this.getPrixPortefeuille(spot) + "€\n";
            res += "##########FIN PORTEFEUILLE##########";
            return res;
        }
    }
}
