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
using LiveCharts;
using WpfApplication2.Options;

namespace WpfApplication2.Simu
{
    public class Simulation
    {
        private Entrees param;
        public ChartValues<double> valeurPf { get; set; }
        public ChartValues<double> PrixOption { get; set; }

        public Simulation(Entrees param)
        {
            this.param = param;
            this.valeurPf = new ChartValues<double> { };
            this.PrixOption = new ChartValues<double> { };
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

        public List<DataFeed> getDonneesSimulees(PricingLibrary.FinancialProducts.Option option)
        {
            var dataFeedCalc = new List<DataFeed>();
            IDataFeedProvider data = new SimulatedDataFeedProvider();
            dataFeedCalc = data.GetDataFeed(option, param.dateDebut);
            return dataFeedCalc;
        }

        public double[] getSpotIndex(int i, List<DataFeed> dataFeedCalc)
        {
            decimal[] res;
            res = dataFeedCalc[i].PriceList.Values.ToList().ToArray();
            double[] newres = Array.ConvertAll(res, item => (double)item);
            return newres;
        }

        public void Lancer()
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

            double[] vol = new double[option.GetNbSousJacents()];
            double[,] cov = new double[option.GetNbSousJacents(), option.GetNbSousJacents()];

            // UNIQUEMENT POUR DES DONNEES SIMULEES

            for (int i =0; i<option.GetNbSousJacents(); i++)
            {
                vol[i] = 0.4;
                for (int j=0; j<option.GetNbSousJacents(); j++)
                {
                    if (i == j)
                    {
                        cov[i, j] = 0.4;
                    }
                    else
                    {
                        cov[i, j] = 0.1;
                    }
                }
            }

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
                double TrackingError = pfRebalancee.getPrixPortefeuille(spot) - option.CalculerPrix(jourActuel, donnees, param.dateDebut, spot, cov, vol);
                Console.WriteLine("Tracking Error Jour " + jourActuel + " = " + TrackingError);
                Console.WriteLine("Tracking Error Relatif Jour " + jourActuel + " = " + TrackingError / option.CalculerPrix(jourActuel, donnees, param.dateDebut, spot, cov, vol)*100 + "%");
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
                this.valeurPf.Add(pfRebalancee.getPrixPortefeuille(spotfin));
            }


        }


    }
}
