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
using WpfApplication2.Data;

namespace WpfApplication2.Simu
{
    public class Simulation
    {
        private Entrees param;
        public ChartValues<double> valeurPf { get; set; }
        public ChartValues<double> PrixOption { get; set; }
        public List<ChartValues<double>> PrixAction { get; set; }
        public List<Portefeuille> historiquePf { get; set; }
        public int jourActuel {get; set;}
        public Options.Option option { get; set; }

        public Simulation(Entrees param)
        {
            this.param = param;
            this.valeurPf = new ChartValues<double> { };
            this.PrixOption = new ChartValues<double> { };
            this.PrixAction = new List<ChartValues<double>> { };
            for (int i=0; i<param.listActions.Count(); i++)
            {
                PrixAction.Add(new ChartValues<double> { }); 
            }
            this.historiquePf = new List<Portefeuille>();
            this.option = CreerOption(this.param);
        }

        public static Options.Option CreerOption(Entrees param)
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
                BasketOption optionPricing = new BasketOption(param.nomOption, SousJacentsArray, ListePoidsArray, param.maturite, param.strike);
                Options.Option option = new OptionBasket(optionPricing);
                return option;
            }
            else if (param.typeoption == Entrees.typeOption.Vanille)
            {
                /*Création de l'action sous jacente*/
                Share SousJacent = new Share(param.listActions[0], param.listActions[0]);
                /*Création de l'option*/
                VanillaCall optionPricing = new VanillaCall(param.nomOption, SousJacent, param.maturite, param.strike);
                Options.Option option = new OptionVanille(optionPricing);
                return option;
            }
            else
            {
                throw new Exception("Type d'option invalide !");
            }
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
            this.jourActuel = 0;
            DataSimu data = new DataSimu();
            List<DataFeed> donnees = data.getData(param);

            Portefeuille premierpf = this.InitialiserPf(option, donnees);   

            for (this.jourActuel = 0; this.jourActuel < (param.maturite - param.dateDebut).Days; this.jourActuel++)
            {
                if (this.jourActuel % param.pas == 0)
                {
                    this.RebalancerPf(param.pas, donnees, option);
                }
                this.ArchiveValeur(donnees, option);
            }
        }

        public Portefeuille InitialiserPf(Options.Option option, List<DataFeed> donnees)
        {
            double[,] cov = this.FakeEstimatorCov(option);
            double[] vol = this.FakeEstimatorVol(option);
            double prix = option.CalculerPrix(0, donnees, param.dateDebut, this.getSpotIndex(0, donnees), cov, vol);
            double[] deltas = option.CalculerDeltas(0, param.dateDebut, vol, cov, this.getSpotIndex(0, donnees));
            Portefeuille premierpf = new Portefeuille(prix, deltas, this.getSpotIndex(0, donnees), option,0);
            historiquePf.Add(premierpf);
            return premierpf;
        }

        public Portefeuille RebalancerPf(int frequence, List<DataFeed> donnees, Options.Option option)
        {
            double[,] cov = this.FakeEstimatorCov(option);
            double[] vol = this.FakeEstimatorVol(option);
            double[] spot = this.getSpotIndex(this.jourActuel, donnees);
            double tauxSansRisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(frequence, 365));
            Portefeuille pfRebalancee = new Portefeuille(spot
                , option.CalculerDeltas(this.jourActuel, param.dateDebut, vol, cov, spot)
                , historiquePf[historiquePf.Count - 1]
                , tauxSansRisque
                , option
                , this.jourActuel);
            historiquePf.Add(pfRebalancee);
            return pfRebalancee;
        }

        public double[] FakeEstimatorVol(Options.Option option)
        {
            double[] vol = new double[option.GetNbSousJacents()];
            for (int i = 0; i < option.GetNbSousJacents(); i++)
            {
                vol[i] = 0.4;
            }
            return vol;
        }

        public double[,] FakeEstimatorCov(Options.Option option)
        {
            
            double[,] cov = new double[option.GetNbSousJacents(), option.GetNbSousJacents()];
            for (int i = 0; i < option.GetNbSousJacents(); i++)
            {
                for (int j = 0; j < option.GetNbSousJacents(); j++)
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
            return cov;
        }

        public void ArchiveValeur(List<DataFeed> donnees, Options.Option option)
        {
            this.valeurPf.Add(historiquePf[historiquePf.Count - 1].getPrixPortefeuille(this.getSpotIndex(jourActuel, donnees),this.jourActuel));
            this.PrixOption.Add(option.CalculerPrix(this.jourActuel, donnees, param.dateDebut, this.getSpotIndex(jourActuel, donnees), this.FakeEstimatorCov(option), this.FakeEstimatorVol(option)));
            for (int i=0; i<option.option.UnderlyingShareIds.Count(); i++)
            {
                this.PrixAction[i].Add(this.getSpotIndex(this.jourActuel, donnees)[i]);
            }
        }

    }
}
