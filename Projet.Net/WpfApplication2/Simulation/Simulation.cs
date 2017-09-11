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

        public double[] Lancer()
        {
            AbstractData donnees = CreerDonnees(this.param, this.option);

            Portefeuille premierpf = this.InitialiserPf(option, donnees);

            for (this.jourActuel = 0; this.jourActuel < donnees.donnees.Count; this.jourActuel++)
            {
                if (this.jourActuel % param.pas == 0 && this.jourActuel!=0)
                {
                    this.RebalancerPf(donnees, option);
                }
                this.ArchiveValeur(donnees, option);
            }

            /*Renvoi du payoff et de la tracking error de la simulation*/
            double[] res = new double[2] { option.CalculerPayoff(donnees.donnees) , this.getTrackingError()};
            return res;
        }


        public Portefeuille InitialiserPf(Options.Option option, AbstractData donnees)
        {
            double[,] corr = donnees.corr(this.jourActuel);
            double[] vol = donnees.vol(this.jourActuel);
            double prix = option.CalculerPrix(0, donnees, donnees.getSpotIndex(0), corr, vol);
            double[] deltas = option.CalculerDeltas(0, donnees, vol, corr, donnees.getSpotIndex(0));
            Portefeuille premierpf = new Portefeuille(prix, deltas, donnees.getSpotIndex(0), option,0);
            historiquePf.Add(premierpf);
            return premierpf;
        }

        public Portefeuille RebalancerPf(AbstractData donnees, Options.Option option)
        {
            double[,] corr = donnees.corr(this.jourActuel);
            double[] vol = donnees.vol(this.jourActuel);
            double[] spot = donnees.getSpotIndex(this.jourActuel);
            int span = (donnees.listeDate[this.jourActuel] - donnees.listeDate[this.jourActuel-param.pas]).Days;
            double tauxSansRisque = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(DayCount.ConvertToDouble(span, 365));
            Portefeuille pfRebalancee = new Portefeuille(spot
                , option.CalculerDeltas(this.jourActuel, donnees, vol, corr, spot)
                , historiquePf[historiquePf.Count - 1]
                , tauxSansRisque
                , option
                , this.jourActuel);
            historiquePf.Add(pfRebalancee);
            return pfRebalancee;
        }

        public void ArchiveValeur(AbstractData donnees, Options.Option option)
        {
            this.valeurPf.Add(historiquePf[historiquePf.Count - 1].getPrixPortefeuille(donnees.getSpotIndex(this.jourActuel)
                ,this.jourActuel,donnees));
            this.PrixOption.Add(option.CalculerPrix(this.jourActuel
                , donnees
                , donnees.getSpotIndex(this.jourActuel)
                , donnees.corr(jourActuel)
                , donnees.vol(jourActuel)));
            for (int i=0; i<option.option.UnderlyingShareIds.Count(); i++)
            {
                this.PrixAction[i].Add(donnees.getSpotIndex(this.jourActuel)[i]);
            }
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

        public static AbstractData CreerDonnees(Entrees param, Options.Option option)
        {
            if (param.typedonnees == Data.typeDonnees.typedonnees.Simulees)
            {
                DataSimu data = new DataSimu(option);
                data.genereData(param.dateDebut);
                return data;
            }
            else if (param.typedonnees == Data.typeDonnees.typedonnees.Historique)
            {
                DataHisto data = new DataHisto(option);
                data.genereData(param.dateDebut);
                return data;
            }
            else
            {
                throw new Exception("Type de données invalide !");
            }
        }

        public double getTrackingError()
        {
            /*Retourne la tracking error à la fin de la simulation*/
            if (this.jourActuel == 0)
            {
                throw new Exception("La simulation en est au jour 0 ! La simulation doit être terminée pour calculer la tracking error");
            }
            else
            {
                return PrixOption[PrixOption.Count - 1] - valeurPf[valeurPf.Count - 1];
            }
        }


    }
}
