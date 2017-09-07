using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Entree;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.Utilities;

namespace WpfApplication2.Simulation
{
    public class Simulation
    {
        private Entrees param;

        public Simulation(Entrees param)
        {
            this.param = param;
        }

        public void Lancer()
        {
            Console.WriteLine("Lancement de la simulation");
            if (param.typeoption == Entrees.typeOption.Vanille)
            {
                this.LancerVanille();
            }
            else if (param.typeoption == Entrees.typeOption.Basket)
            {
                this.LancerBasket();
            }
            else
            {
                throw new Exception("Le type d'option est invalide");
            }
            Console.WriteLine("Fin de la simulation");
        }

        public void LancerVanille()
        {
            
        }

        public void LancerBasket()
        {
            /*Créations des actions sous jacentes*/

            List<Share> SousJacents = new List<Share>();
            foreach(var action in param.listActions)
            {
                SousJacents.Add(new Share(action, action));
            }

            /*Creation de l'option Basket*/

            Share[] SousJacentsArray = SousJacents.ToArray();
            Double[] ListePoidsArray = param.listePoids.ToArray();

            BasketOption option = new BasketOption(param.nomOption, SousJacentsArray, ListePoidsArray, param.maturite,param.strike);

            /*Simulation des données*/

            /*/!\ Données simulées pour l'instant uniquement /!\ */

            List<DataFeed> dataFeedCalc = this.getDonneesSimulees(option);

            /*Calcul du payoff*/
            double payoff = option.GetPayoff(dataFeedCalc[dataFeedCalc.Count - 1].PriceList);
            Console.WriteLine("Le payoff du basket vaut : " + payoff);

            /*Calcul des portefeuilles de couverture*/

            Pricer pricer = new Pricer();
            double[] spot = new double[] { (double) dataFeedCalc[0].PriceList["accor"] , (double) dataFeedCalc[0].PriceList["bnp"] };
            // Dans le cas simulé vaut toujours 0.4
            double[] volatility = new double[] { 0.4 , 0.4 };
            double[,] matrice = new double[,] { { 0.4 , 0.1} ,{ 0.1, 0.4} };
            double prix = pricer.PriceBasket(option, param.dateDebut, 365, spot,volatility, matrice).Price;
            double[] deltas = pricer.PriceBasket(option, param.dateDebut, 365, spot, volatility, matrice).Deltas;
            Console.WriteLine("Prix initial de l'option = " + prix + "€\n");

            /*Calcul du premier portefeuille*/
            PorteFeuilleBasket premierpf = new PorteFeuilleBasket(prix,deltas, this.getSpotIndex(0,option, dataFeedCalc),option);

            Console.WriteLine(premierpf.ToString(this.getSpotIndex(0, option, dataFeedCalc)));
            double track = premierpf.getPrixPortefeuille(this.getSpotIndex(0, option, dataFeedCalc)) - this.calculerPrixBasket(0, option, dataFeedCalc);

            /*Rebalancements*/
            int jourActuel = 0;
            int frequence = param.pas; 
            List<PorteFeuilleBasket> historiquePf = new List<PorteFeuilleBasket>();
            historiquePf.Add(premierpf);
            while (jourActuel < dataFeedCalc.Count-frequence)
            {
                double tauxSansRisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(frequence, 365));
                jourActuel += frequence;
                Console.WriteLine("Jour = " + jourActuel);
                PorteFeuilleBasket pfRebalancee = new PorteFeuilleBasket(this.getSpotIndex(jourActuel, option, dataFeedCalc)
                    , this.calculerDeltas(jourActuel, option, dataFeedCalc).ToList()
                    , this.calculerDeltas(jourActuel - frequence, option, dataFeedCalc).ToList()
                    , historiquePf[historiquePf.Count-1]
                    , tauxSansRisque 
                    , option); 
                historiquePf.Add(pfRebalancee);
                Console.WriteLine(pfRebalancee.ToString(this.getSpotIndex(jourActuel, option, dataFeedCalc)));
                Console.WriteLine("Tracking Error Jour " + jourActuel + " = " + this.calculerTrackingErrorBasket(jourActuel,option,pfRebalancee, dataFeedCalc));
                Console.WriteLine("Tracking Error Relatif Jour " + jourActuel + " = " + this.calculerTrackingErrorRelatifBasket(jourActuel, option, pfRebalancee, dataFeedCalc)+"%");
                double TrackingError = pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel, option, dataFeedCalc)) - this.calculerPrixBasket(jourActuel,option, dataFeedCalc);
            }
            // Calcul de la dernière date 
            int daySpan = (param.maturite - param.dateDebut).Days;
            int LastDay = daySpan;
            if (jourActuel < LastDay)
            {
                double tauxssrisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(LastDay-jourActuel, 365));
                Console.WriteLine("Jour = " + LastDay);
                PorteFeuilleBasket pfRebalancee = new PorteFeuilleBasket(this.getSpotIndex(LastDay, option, dataFeedCalc)
                    , this.calculerDeltas(LastDay, option, dataFeedCalc).ToList()
                    , this.calculerDeltas(LastDay - frequence, option, dataFeedCalc).ToList()
                    , historiquePf[historiquePf.Count - 1]
                    , tauxssrisque // TAUX SANS RISQUE A MODIFIER
                    , option);
                Console.WriteLine(pfRebalancee.ToString(this.getSpotIndex(LastDay, option, dataFeedCalc)));
                Console.WriteLine("Tracking Error Jour " + LastDay + " = " + this.calculerTrackingErrorBasket(LastDay, option, pfRebalancee, dataFeedCalc));
                Console.WriteLine("Tracking Error Relatif Jour " + LastDay + " = " + this.calculerTrackingErrorRelatifBasket(LastDay, option, pfRebalancee, dataFeedCalc) + "%");
            }
        }

        public List<DataFeed> getDonneesSimulees(Option option)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(option, param.dateDebut);
            return dataFeedCalc;
        }

        public double[] getSpotIndex(int i, Option option, List<DataFeed> dataFeedCalc)
        {
            decimal[] res;
            res = dataFeedCalc[i].PriceList.Values.ToList().ToArray();
            double[] newres = Array.ConvertAll(res, item => (double)item);
            return newres;
        }

        public double[] calculerDeltas(int i, BasketOption option, List<DataFeed> dataFeedCalc)
        {
            Pricer pricer = new Pricer();
            double[] volatility = new double[] { 0.4, 0.4 };
            double[,] matrice = new double[,] { { 0.4, 0.1 }, { 0.1, 0.4 } };
            DateTime dateAvancee = param.dateDebut;
            dateAvancee = dateAvancee.AddDays(i);
            double[] deltas = pricer.PriceBasket(option, dateAvancee, 365, this.getSpotIndex(i,option, dataFeedCalc), volatility, matrice).Deltas;
            return deltas;
        }

        public double calculerPrixBasket(int i, BasketOption option, List<DataFeed> dataFeedCalc)
        {
            Pricer pricer = new Pricer();
            double[] volatility = new double[] { 0.4, 0.4 };
            double[,] matrice = new double[,] { { 0.4, 0.1 }, { 0.1, 0.4 } };
            DateTime dateAvancee = param.dateDebut;
            dateAvancee =  dateAvancee.AddDays(i);
            double prix = pricer.PriceBasket(option, dateAvancee, 365, this.getSpotIndex(i, option, dataFeedCalc), volatility, matrice).Price;
            return prix;
        }

        public double calculerTrackingErrorBasket(int jour, BasketOption option, PorteFeuilleBasket pf, List<DataFeed> dataFeedCalc)
        {
            var res = 0.0;
            res = Math.Abs(this.calculerPrixBasket(jour, option, dataFeedCalc) - pf.getPrixPortefeuille(this.getSpotIndex(jour,option,dataFeedCalc)));
            return res;
        }

        public double calculerTrackingErrorRelatifBasket(int jour, BasketOption option, PorteFeuilleBasket pf, List<DataFeed> dataFeedCalc)
        {
            var res = 0.0;
            res = (this.calculerPrixBasket(jour, option, dataFeedCalc) - pf.getPrixPortefeuille(this.getSpotIndex(jour, option, dataFeedCalc))) / this.calculerPrixBasket(jour, option, dataFeedCalc);
            return res * 100.0;
        }

    }
}
