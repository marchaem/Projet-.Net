using PricingLibrary.FinancialProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Portfolio;

namespace WpfApplication2
{
    public class PorteFeuilleBasket : Portefeuille
    {
        BasketOption optionBasket;
        List<double> PartActions;
        double tauxSansRisque;

        public PorteFeuilleBasket(double[] spot, List<double> deltas, List<double> deltasPrec, PorteFeuilleBasket pfPrec, double r, BasketOption option)
        {
            // Constructeur qui crée un portefeuille rebalancé
            this.PartActions = new List<double>(deltas);
            this.optionBasket = option;
            double valeurActu = pfPrec.actu(spot,r, pfPrec);
            this.tauxSansRisque = valeurActu;
            int i = 0;
            foreach(var delta in deltas)
            {
                tauxSansRisque -= delta * spot[i];
                i++;
            }      
        }

        public PorteFeuilleBasket(double prixOption, double[] deltas, double[] spot, BasketOption option) 
        {
            // Constructeur qui créer le premier portefeuille à l'instant 0
            this.optionBasket = option;
            this.PartActions = new List<double>(deltas);
            double montantAction = 0;
            if (deltas.Count() != spot.Count())
            {
                throw new Exception("La taille des actions et des deltas ne correspond pas !");
            }
            for (int i=0; i< deltas.Count(); i++)
            {
                montantAction += deltas[i] * spot[i];
            }
            tauxSansRisque = prixOption - montantAction;
        }

        public double actu(double[] spot, double r, PorteFeuilleBasket pfPrec)
        {
            // Actualise la valeur du portefeuille au temps n jusqu'au temps n+1
            // r = taux sans risque
            List<double> anciensDeltas = pfPrec.PartActions;
            var valeur = 0.0;
            int i = 0;
            foreach (var delta in anciensDeltas)
            {

                valeur += delta * spot[i]; 
                i++;
            }

            valeur += pfPrec.tauxSansRisque * r;
            
            return valeur;
        }

        public double getPrixPortefeuille(double[] spot)
        {
            // Recupere la valeur du portefeuille à l'instant actuel
            var result = 0.0;
            int i = 0;
            foreach(var par in PartActions)
            {
                result += par * spot[i]; 
                i++;
            }
            result += tauxSansRisque;
            return result;
        }

        public string ToString(double[] spot)
        {
            var res = "";
            res += "##########PORTEFEUILLE##########\n";
            res += "Part des actions \n" ;
            int i = 0;
            foreach(var nb in PartActions)
            {
                res += nb + " actions " + optionBasket.UnderlyingShareIds[i] + " (Cours " + spot[i] + "€ )\n" ;
                i++;
            }
            res += "Montant dans le sans risque : " + tauxSansRisque + "€\n";
            res += "Valeur totale du portefeuille = " + this.getPrixPortefeuille(spot) + "€\n";
            res += "##########FIN PORTEFEUILLE##########";
            return res;

        }


    }
}
