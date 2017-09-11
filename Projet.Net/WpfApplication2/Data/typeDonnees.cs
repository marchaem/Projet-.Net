using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Data
{
    public class typeDonnees
    {
        public enum typedonnees
        {
            Historique, Simulees
        }
        public typedonnees donnees { get; set; }
        public typeDonnees()
        {

        }

        public typeDonnees(typedonnees data)
        {
            this.donnees = data;
        }
        public override string ToString()
        {
            if (donnees.Equals(typeDonnees.typedonnees.Historique))
            {
                return "Historique";
            }
            else
            {
                return "Simulées";
            }
        }



    }
    

    

   /* public virtual event PropertyChangedEventHandler PropertyChanged;
    protected virtual void NotifyPropertyChanged(params string[] propertyNames)
    {
        if (PropertyChanged != null)
        {
            foreach (string propertyName in propertyNames) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged(this, new PropertyChangedEventArgs("HasError"));
        }
    }*/
}
