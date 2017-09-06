using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Mesure
{
    class ListeMesures
    {
        List<Mesure> mesures;

        public ListeMesures(DataHistoriqueDataContext data)
        {
            //this.mesures = new List<Mesure>() { data.HistoricalShareValues.Select(el => el.id) };

        }
    }
}
