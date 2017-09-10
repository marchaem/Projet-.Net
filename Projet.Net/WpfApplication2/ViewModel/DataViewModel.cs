using WpfApplication2.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace WpfApplication2.ViewModel
{
    public class DataViewModel : INotifyPropertyChanged
    {
       static typeDonnees donnees1 = new typeDonnees(typeDonnees.typedonnees.Historique);
       static typeDonnees donnees2 = new typeDonnees(typeDonnees.typedonnees.Simulees);

        List<typeDonnees> data = new List<typeDonnees>() { donnees1, donnees2 };
        
        public List<typeDonnees>Data
        {
            get { return data; }
            set { data = value; }
            //INotifyPropertyChanged("Donnees"); }
        }
        private typeDonnees selectedData = new typeDonnees();

        public event PropertyChangedEventHandler PropertyChanged;

        public typeDonnees SelectedData
        {
            get { return selectedData; }
            set { selectedData = value; } //INotifyPropertyChanged("SelectedItem"); }            
        }
        
    }
}
