using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication2.Portfolio;
using WpfApplication2.Entree;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Computations;
using PricingLibrary.Utilities.MarketDataFeed;
using PricingLibrary.Utilities;
using WpfApplication2.Options;

namespace WpfApplication2.Simu
{
    public class Simulation
    {
        private Entrees param;
        public List<double> valeurPf { get; set; }
        public List<double> PrixOption { get; set; }

        public Simulation(Entrees param)
        {
            this.param = param;
            this.valeurPf = new List<double>();
            this.PrixOption = new List<double>();
        }

        public PricingLibrary.FinancialProducts.Option CreerOption(Entrees param)
        {
            if (param.typeoption == Entrees.typeOption.Basket)
            {
                /*Création des sous jacents*/
                List<Share> SousJacents = new List<Share>();
                foreach (var action in param.listActions)
                {
                    SousJacents.Add(new Share(action, action));
                }

                Share[] SousJacentsArray = SousJacents.ToArray();
                Double[] ListePoidsArray = param.listePoids.ToArray();

                /*Creation de l'option*/
                BasketOption option = new BasketOption(param.nomOption, SousJacentsArray, ListePoidsArray, param.maturite, param.strike);

                return option;
            }
            else if (param.typeoption == Entrees.typeOption.Vanille)
            {
                /*Création de l'action sous jacente*/
                Share SousJacent = new Share(param.listActions[0], param.listActions[0]);

                /*Création de l'option*/
                VanillaCall option = new VanillaCall(param.nomOption, SousJacent, param.maturite, param.strike);

                return option;
            }
            else
            {
                throw new Exception("Type d'option invalide !");
            }
        }

        public void Lancer()
        {
            Console.WriteLine("Lancement de la simulation");

            PricingLibrary.FinancialProducts.Option option = this.CreerOption(param);

            List<DataFeed> donnees;
            if (param.typedonnees == Entrees.typeDonnees.Simulees)
            {
                /*A modifier en utilisant DataSimu*/
                donnees = this.getDonneesSimulees(option);
            }
            else if (param.typedonnees == Entrees.typeDonnees.Simulees) {
                throw new Exception("Not yet implemented");
            }
            else
            {
                throw new Exception("Type de données invalides !");
            }


            if (param.typeoption == Entrees.typeOption.Vanille)
            {
                this.LancerVanille(donnees, (VanillaCall) option);
            }
            else if (param.typeoption == Entrees.typeOption.Basket)
            {
                this.LancerBasket(donnees, (BasketOption) option);
            }
            else
            {
                throw new Exception("Le type d'option est invalide");
            }

            Console.WriteLine("Fin de la simulation");
        }

        public void LancerVanille(List<DataFeed> donnees, VanillaCall option)
        {

            /*Calcul du payoff*/
            double payoff = option.GetPayoff(donnees[donnees.Count - 1].PriceList);
            Console.WriteLine("Le payoff du VanillaCall vaut : " + payoff);

            /*Calcul des portefeuilles de couverture*/

            Pricer pricer = new Pricer();
            double spot = (double) donnees[0].PriceList.Values.ToList()[0];

            double volatility = 0.4;
            double prix = pricer.PriceCall(option, param.dateDebut, 365, spot, volatility).Price;
            double delta = pricer.PriceCall(option, param.dateDebut, 365, spot, volatility).Deltas[0];
            Console.WriteLine("Prix initial de l'option = " + prix + "€\n");

            PorteFeuilleVanille premierpf = new PorteFeuilleVanille(prix, delta, this.getSpotIndex(0, option, donnees)[0], option);

            Console.WriteLine(premierpf.ToString(this.getSpotIndex(0, option, donnees)[0]));

            this.valeurPf.Add(premierpf.getPrixPortefeuille(this.getSpotIndex(0, option, donnees)[0]));
            this.PrixOption.Add(prix);

            /*Rebalancement*/

            int jourActuel = 0;
            int frequence = param.pas; // A modifier ensuite avec les parametres de l'entrée ...
            List<PorteFeuilleVanille> historiquePf = new List<PorteFeuilleVanille>();
            historiquePf.Add(premierpf);

            while (jourActuel < donnees.Count - frequence)
            {
                double tauxSansRisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(frequence, 365));
                jourActuel += frequence;
                Console.WriteLine("Jour = " + jourActuel);
                PorteFeuilleVanille pfRebalancee = new PorteFeuilleVanille(this.getSpotIndex(jourActuel, option, donnees)[0]
                    , this.calculerDeltasVanille(jourActuel, option, donnees)
                    , historiquePf[historiquePf.Count - 1]
                    , tauxSansRisque
                    , option);
                historiquePf.Add(pfRebalancee);
                Console.WriteLine(pfRebalancee.ToString(this.getSpotIndex(jourActuel, option, donnees)[0]));
                Console.WriteLine("Tracking Error Jour " + jourActuel + " = " + this.calculerTrackingErrorVanille(jourActuel, option, pfRebalancee, donnees));
                Console.WriteLine("Tracking Error Relatif Jour " + jourActuel + " = " + this.calculerTrackingErrorRelatifVanille(jourActuel, option, pfRebalancee, donnees) + "%");
                double TrackingError = pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel, option, donnees)[0]) - this.calculerPrixVanille(jourActuel, option, donnees);

                this.PrixOption.Add(this.calculerPrixVanille(jourActuel, option, donnees));
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel, option, donnees)[0]));
            }

            // Calcul de la dernière date 
            int daySpan = (param.maturite - param.dateDebut).Days;
            int LastDay = daySpan;
            if (jourActuel < LastDay)
            {
                double tauxssrisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(LastDay - jourActuel, 365));
                Console.WriteLine("Jour = " + LastDay);
                PorteFeuilleVanille pfRebalancee = new PorteFeuilleVanille(this.getSpotIndex(LastDay, option, donnees)[0]
                    , this.calculerDeltasVanille(LastDay, option, donnees)
                    , historiquePf[historiquePf.Count - 1]
                    , tauxssrisque
                    , option);
                Console.WriteLine(pfRebalancee.ToString(this.getSpotIndex(LastDay, option, donnees)[0]));
                Console.WriteLine("Tracking Error Jour " + LastDay + " = " + this.calculerTrackingErrorVanille(LastDay, option, pfRebalancee, donnees));
                Console.WriteLine("Tracking Error Relatif Jour " + LastDay + " = " + this.calculerTrackingErrorRelatifVanille(LastDay, option, pfRebalancee, donnees) + "%");

                this.PrixOption.Add(this.calculerPrixVanille(LastDay, option, donnees));
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(this.getSpotIndex(LastDay, option, donnees)[0]));
            }




        }
        public void LancerBasket(List<DataFeed> donnees, BasketOption option)
        {

            /*Calcul du payoff*/
            double payoff = option.GetPayoff(donnees[donnees.Count - 1].PriceList);
            Console.WriteLine("Le payoff du basket vaut : " + payoff);

            /*Calcul des portefeuilles de couverture*/

            Pricer pricer = new Pricer();
            double[] spot = new double[] { (double) donnees[0].PriceList["accor"] , (double) donnees[0].PriceList["bnp"] };
            // Dans le cas simulé vaut toujours 0.4
            double[] volatility = new double[] { 0.4 , 0.4 };
            double[,] matrice = new double[,] { { 0.4 , 0.1} ,{ 0.1, 0.4} };
            double prix = pricer.PriceBasket(option, param.dateDebut, 365, spot,volatility, matrice).Price;
            double[] deltas = pricer.PriceBasket(option, param.dateDebut, 365, spot, volatility, matrice).Deltas;
            Console.WriteLine("Prix initial de l'option = " + prix + "€\n");


            /*Calcul du premier portefeuille*/
            PorteFeuilleBasket premierpf = new PorteFeuilleBasket(prix,deltas, this.getSpotIndex(0,option, donnees),option);

            Console.WriteLine(premierpf.ToString(this.getSpotIndex(0, option, donnees)));

            this.valeurPf.Add(premierpf.getPrixPortefeuille(this.getSpotIndex(0, option, donnees)));
            this.PrixOption.Add(prix);

            double track = premierpf.getPrixPortefeuille(this.getSpotIndex(0, option, donnees)) - this.calculerPrixBasket(0, option, donnees);

            /*Rebalancements*/
            int jourActuel = 0;
            int frequence = param.pas;
            List<PorteFeuilleBasket> historiquePf = new List<PorteFeuilleBasket>();
            historiquePf.Add(premierpf);
            while (jourActuel < donnees.Count-frequence)
            {
                double tauxSansRisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(frequence, 365));
                jourActuel += frequence;
                Console.WriteLine("Jour = " + jourActuel);
                PorteFeuilleBasket pfRebalancee = new PorteFeuilleBasket(this.getSpotIndex(jourActuel, option, donnees)
                    , this.calculerDeltasBasket(jourActuel, option, donnees).ToList()
                    , this.calculerDeltasBasket(jourActuel - frequence, option, donnees).ToList()
                    , historiquePf[historiquePf.Count-1]
                    , tauxSansRisque 
                    , option); 
                historiquePf.Add(pfRebalancee);
                Console.WriteLine(pfRebalancee.ToString(this.getSpotIndex(jourActuel, option, donnees)));
                Console.WriteLine("Tracking Error Jour " + jourActuel + " = " + this.calculerTrackingErrorBasket(jourActuel,option,pfRebalancee, donnees));
                Console.WriteLine("Tracking Error Relatif Jour " + jourActuel + " = " + this.calculerTrackingErrorRelatifBasket(jourActuel, option, pfRebalancee, donnees)+"%");
                double TrackingError = pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel, option, donnees)) - this.calculerPrixBasket(jourActuel,option, donnees);

                this.PrixOption.Add(this.calculerPrixBasket(jourActuel,option,donnees));
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel,option,donnees)));
            }
            // Calcul de la dernière date 
            int daySpan = (param.maturite - param.dateDebut).Days;
            int LastDay = daySpan;
            if (jourActuel < LastDay)
            {
                double tauxssrisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(LastDay-jourActuel, 365));
                Console.WriteLine("Jour = " + LastDay);
                PorteFeuilleBasket pfRebalancee = new PorteFeuilleBasket(this.getSpotIndex(LastDay, option, donnees)
                    , this.calculerDeltasBasket(LastDay, option, donnees).ToList()
                    , this.calculerDeltasBasket(LastDay - frequence, option, donnees).ToList()
                    , historiquePf[historiquePf.Count - 1]
                    , tauxssrisque 
                    , option);
                Console.WriteLine(pfRebalancee.ToString(this.getSpotIndex(LastDay, option, donnees)));
                Console.WriteLine("Tracking Error Jour " + LastDay + " = " + this.calculerTrackingErrorBasket(LastDay, option, pfRebalancee, donnees));
                Console.WriteLine("Tracking Error Relatif Jour " + LastDay + " = " + this.calculerTrackingErrorRelatifBasket(LastDay, option, pfRebalancee, donnees) + "%");

                this.PrixOption.Add(this.calculerPrixBasket(LastDay, option, donnees));
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(this.getSpotIndex(LastDay, option, donnees)));
            }
        }

        public List<DataFeed> getDonneesSimulees(PricingLibrary.FinancialProducts.Option option)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(option, param.dateDebut);
            return dataFeedCalc;
        }

        public double[] getSpotIndex(int i, PricingLibrary.FinancialProducts.Option option, List<DataFeed> dataFeedCalc)
        {
            decimal[] res;
            res = dataFeedCalc[i].PriceList.Values.ToList().ToArray();
            double[] newres = Array.ConvertAll(res, item => (double)item);
            return newres;
        }

        public double[] getSpotIndex(int i, List<DataFeed> dataFeedCalc)
        {
            decimal[] res;
            res = dataFeedCalc[i].PriceList.Values.ToList().ToArray();
            double[] newres = Array.ConvertAll(res, item => (double)item);
            return newres;
        }


        public double[] calculerDeltasBasket(int i, BasketOption option, List<DataFeed> dataFeedCalc)
        {
            Pricer pricer = new Pricer();
            double[] volatility = new double[] { 0.4, 0.4 };
            double[,] matrice = new double[,] { { 0.4, 0.1 }, { 0.1, 0.4 } };
            DateTime dateAvancee = param.dateDebut;
            dateAvancee = dateAvancee.AddDays(i);
            double[] deltas = pricer.PriceBasket(option, dateAvancee, 365, this.getSpotIndex(i,option, dataFeedCalc), volatility, matrice).Deltas;
            return deltas;
        }

        public double calculerDeltasVanille(int i, VanillaCall option, List<DataFeed> dataFeedCalc)
        {
            Pricer pricer = new Pricer();
            double volatility = 0.4;
            DateTime dateAvancee = param.dateDebut;
            dateAvancee = dateAvancee.AddDays(i);
            double delta = pricer.PriceCall(option, dateAvancee, 365, this.getSpotIndex(i, option, dataFeedCalc)[0], volatility).Deltas[0];
            return delta;
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

        public double calculerPrixVanille(int i, VanillaCall option, List<DataFeed> dataFeedCalc)
        {
            Pricer pricer = new Pricer();
            double volatility = 0.4;
            DateTime dateAvancee = param.dateDebut;
            dateAvancee = dateAvancee.AddDays(i);
            double prix = pricer.PriceCall(option, dateAvancee, 365, this.getSpotIndex(i, option, dataFeedCalc)[0], volatility).Price;
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

        public double calculerTrackingErrorVanille(int jour, VanillaCall option, PorteFeuilleVanille pf, List<DataFeed> dataFeedCalc)
        {
            var res = 0.0;
            res = (this.calculerPrixVanille(jour, option, dataFeedCalc) - pf.getPrixPortefeuille(this.getSpotIndex(jour, option, dataFeedCalc)[0])) / this.calculerPrixVanille(jour, option, dataFeedCalc);
            return res;
        }

        public double calculerTrackingErrorRelatifVanille(int jour, VanillaCall option, PorteFeuilleVanille pf, List<DataFeed> dataFeedCalc)
        {
            var res = 0.0;
            res = (this.calculerPrixVanille(jour, option, dataFeedCalc) - pf.getPrixPortefeuille(this.getSpotIndex(jour, option, dataFeedCalc)[0])) / this.calculerPrixVanille(jour, option, dataFeedCalc);
            return res * 100.0;
        }

        public void LancerUnique()
        {
            Options.Option option;
            if (param.typeoption == Entrees.typeOption.Basket)
            {
                option = new OptionBasket(this.CreerOption(param));
            }
            else 
            {
                option = new OptionVanille(this.CreerOption(param));
            }

            List<DataFeed> donnees = this.getDonneesSimulees(option.option);

            double payoff = option.CalculerPayoff(donnees);
            Console.WriteLine("Le payoff de l'option vaut : " + payoff);


            double[] vol = {0.4, 0.4} ; // A Modifier
            double[,] cov = { { 0.4, 0.1}, {0.1, 0.4 } }; // A Modifier

            double prix = option.CalculerPrix(0,donnees,param.dateDebut,this.getSpotIndex(0,donnees),cov,vol);
            Console.WriteLine("Prix initial de l'option = " + prix + "€\n");

            /*Calcul du premier portefeuille*/

            double[] deltas = option.CalculerDeltas(0, param.dateDebut, vol, cov, this.getSpotIndex(0, donnees));

            Portefeuille premierpf = new Portefeuille(prix, deltas, this.getSpotIndex(0, donnees), option);

            Console.WriteLine(premierpf.ToString(this.getSpotIndex(0,donnees)));

            int jourActuel = 0;
            int frequence = param.pas;
            List<Portefeuille> historiquePf = new List<Portefeuille>();
            historiquePf.Add(premierpf);

            this.PrixOption.Add(option.CalculerPrix(jourActuel, donnees, param.dateDebut, this.getSpotIndex(0, donnees), cov, vol));
            this.valeurPf.Add(premierpf.getPrixPortefeuille(this.getSpotIndex(0, donnees)));


            /*Calcul des portefeuilles suivants*/
            while (jourActuel < donnees.Count - frequence)
            {
                jourActuel += frequence;
                double[] spot = this.getSpotIndex(jourActuel, donnees);
                double tauxSansRisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(frequence, 365));
                Console.WriteLine("Jour = " + jourActuel);
                Portefeuille pfRebalancee = new Portefeuille(spot
                    , option.CalculerDeltas(jourActuel,param.dateDebut,vol,cov,spot)
                    , historiquePf[historiquePf.Count-1]
                    , tauxSansRisque
                    , option);
                historiquePf.Add(pfRebalancee);
                Console.WriteLine(pfRebalancee.ToString(spot));
                //Console.WriteLine("Tracking Error Jour " + jourActuel + " = " + this.calculerTrackingErrorVanille(jourActuel, option, pfRebalancee, donnees));
                //Console.WriteLine("Tracking Error Relatif Jour " + jourActuel + " = " + this.calculerTrackingErrorRelatifVanille(jourActuel, option, pfRebalancee, donnees) + "%");
                double TrackingError = pfRebalancee.getPrixPortefeuille(spot) - option.CalculerPrix(jourActuel, donnees, param.dateDebut, spot, cov, vol);
                Console.WriteLine("Tracking Error Jour " + jourActuel + " = " + TrackingError);
                Console.WriteLine("Tracking Error Relatif Jour " + jourActuel + " = " + TrackingError / option.CalculerPrix(jourActuel, donnees, param.dateDebut, spot, cov, vol) + "%");
                this.PrixOption.Add(option.CalculerPrix(jourActuel, donnees, param.dateDebut, spot, cov, vol));
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel, donnees)));
            }

            /*Calcul du dernier portefeuille*/
            int daySpan = (param.maturite - param.dateDebut).Days;
            int LastDay = daySpan;
            if (jourActuel < LastDay)
            {
                Console.WriteLine("Jour = " + LastDay);
                double[] spotfin = this.getSpotIndex(LastDay, donnees);
                double tauxssrisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(LastDay - jourActuel, 365));
                Console.WriteLine("Jour = " + LastDay);
                Portefeuille pfRebalancee = new Portefeuille(spotfin
                    , option.CalculerDeltas(LastDay, param.dateDebut, vol, cov, spotfin)
                    , historiquePf[historiquePf.Count - 1]
                    , tauxssrisque
                    , option);
                double TrackingError = pfRebalancee.getPrixPortefeuille(spotfin) - option.CalculerPrix(LastDay, donnees, param.dateDebut, spotfin, cov, vol);
                Console.WriteLine(pfRebalancee.ToString(spotfin));
                Console.WriteLine("Tracking Error Jour " + LastDay + " = " + TrackingError);
                Console.WriteLine("Tracking Error Relatif Jour " + LastDay + " = " + TrackingError / option.CalculerPrix(LastDay, donnees, param.dateDebut, spotfin, cov, vol) *100 + "%");
                this.PrixOption.Add(option.CalculerPrix(LastDay, donnees, param.dateDebut, spotfin, cov, vol));
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(this.getSpotIndex(jourActuel, donnees)));
            }


        }


    }
}
