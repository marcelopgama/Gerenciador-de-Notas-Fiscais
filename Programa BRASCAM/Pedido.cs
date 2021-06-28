using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_BRASCAM
{
    public class Pedido
    {
        object protocolo, numDoPedido, usuário, dataDoPedido, responsávelPelaEntrega;
        object tipo, veículo, dataDeEntrega, status, quilometragem, frete, transportadora;
        object observação, quilometragemFinal;

        public object Protocolo { get => protocolo; set => protocolo = value; }
        public object NumDoPedido { get => numDoPedido; set => numDoPedido = value; }
        public object Usuário { get => usuário; set => usuário = value; }
        public object DataDoPedido { get => dataDoPedido; set => dataDoPedido = value; }
        public object ResponsávelPelaEntrega { get => responsávelPelaEntrega; set => responsávelPelaEntrega = value; }
        public object Tipo { get => tipo; set => tipo = value; }
        public object Veículo { get => veículo; set => veículo = value; }
        public object DataDeEntrega { get => dataDeEntrega; set => dataDeEntrega = value; }
        public object Status { get => status; set => status = value; }
        public object Quilometragem { get => quilometragem; set => quilometragem = value; }
        public object Frete { get => frete; set => frete = value; }
        public object Transportadora { get => transportadora; set => transportadora = value; }
        public object Observação { get => observação; set => observação = value; }
        public object QuilometragemFinal { get => quilometragemFinal; set => quilometragemFinal = value; }
    }
}
