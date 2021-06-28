using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_BRASCAM
{
    public class ListaDupla
    {
        List<object> item1, item2;

        public List<object> Item1 { get => item1; set => item1 = value; }
        public List<object> Item2 { get => item2; set => item2 = value; }

        public object Procurar_Equivalente(object Procurar)
        {
            object retorno;

            if (Item1.Contains(Procurar))
            {
                retorno = Item2[Item1.IndexOf(Procurar)];
            }
            else { retorno = ""; }

            return retorno;
        }
    }
}
