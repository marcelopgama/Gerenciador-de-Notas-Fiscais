using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_BRASCAM
{
    class Usuários
    {
        object nome, permissão, código, senha;

        public object Nome { get => nome; set => nome = value; }
        public object Permissão { get => permissão; set => permissão = value; }
        public object Código { get => código; set => código = value; }
        public object Senha { get => senha; set => senha = value; }
    }
}
