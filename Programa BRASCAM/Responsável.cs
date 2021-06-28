using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_BRASCAM
{
    class Responsável
    {
        object codigo, nome, telefone, empresa, cnh;

        public object Codigo { get => codigo; set => codigo = value; }
        public object Nome { get => nome; set => nome = value; }
        public object Telefone { get => telefone; set => telefone = value; }
        public object Empresa { get => empresa; set => empresa = value; }
        public object Cnh { get => cnh; set => cnh = value; }
    }
}
