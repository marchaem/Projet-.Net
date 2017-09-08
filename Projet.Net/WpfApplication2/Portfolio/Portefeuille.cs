using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PricingLibrary.FinancialProducts;
using System.Linq;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.Utilities;

namespace WpfApplication2.Portfolio
{
   public class Portefeuille
    {
        Options.Option option;
        List<double> nbActions;
        double tauxSansRisque;
        int date;

        public Portefeuille(double[] spot, double[] deltas, Portefeuille pfPrec, double r, Options.Option option, int date)
        {
            // Constructeur qui crée un portefeuille rebalancé
            this.nbActions = deltas.ToList();
            this.option = option;
            double valeurActu = pfPrec.actu(spot, r, pfPrec);
            this.tauxSansRisque = valeurActu;
            for (int i=0; i<deltas.Length; i++)
            {
                this.tauxSansRisque -= deltas[i] * spot[i];
            }
        }

        public Portefeuille(double prixOption, double[] deltas, double[] spot, Options.Option option, int date)
        {
            this.option = option;
            this.nbActions = new List<double>(deltas);
            double montantAction = 0;
            if (deltas.Count() != spot.Count())
            {
                throw new Exception("La taille des actions et des deltas ne correspond pas !");
            }
            for (int i = 0; i < deltas.Count(); i++)
            {
                montantAction += deltas[i] * spot[i];
            }
            tauxSansRisque = prixOption - montantAction;
        }

        public double actu(double[] spot, double r, Portefeuille pfPrec)
        {
            // Actualise la valeur du portefeuille au temps n jusqu'au temps n+1
            // r = taux sans risque
            List<double> anciensDeltas = pfPrec.nbActions;
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

        public double getPrixPortefeuille(double[] spot, int now)
        {
            // Recupere la valeur du portefeuille à l'instant actuel
            var result = 0.0;
            int i = 0;
            foreach (var par in this.nbActions)
            {
                result += par * spot[i];
                i++;
            }
            result += tauxSansRisque * RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(now-date, 365)) ;
            return result;
        }

        public string ToString(double[] spot, int date)
        {
            var res = "";
            res += "##########PORTEFEUILLE##########\n";
            res += "Part des actions \n";
            int i = 0;
            foreach (var nb in this.nbActions)
            {
                res += nb + " actions " + i + " (Cours " + spot[i] + "€ )\n";
                i++;
            }
            res += "Montant dans le sans risque : " + tauxSansRisque + "€\n";
            res += "Valeur totale du portefeuille = " + this.getPrixPortefeuille(spot,date) + "€\n";
            res += "##########FIN PORTEFEUILLE##########";
            return res;

        }

    }
}
