using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_BRASCAM
{
    class NotaFiscal
    {
        object numDaNota, numDoPedido, dataDaNota, codDoCliente, statusDoPedido;
        object redecerDinheiro, tipo, dataDeEntrega, codigoDaNota, numPedidoDeEntrega;
        object valorDaNota, modoDeEntrega, tipoDeEntrega, frete;

        public object NumDaNota { get => numDaNota; set => numDaNota = value; }
        public object NumDoPedido { get => numDoPedido; set => numDoPedido = value; }
        public object DataDaNota { get => dataDaNota; set => dataDaNota = value; }
        public object CodDoCliente { get => codDoCliente; set => codDoCliente = value; }
        public object StatusDoPedido { get => statusDoPedido; set => statusDoPedido = value; }
        public object RedecerDinheiro { get => redecerDinheiro; set => redecerDinheiro = value; }
        public object Tipo { get => tipo; set => tipo = value; }
        public object DataDeEntrega { get => dataDeEntrega; set => dataDeEntrega = value; }
        public object CodigoDaNota { get => codigoDaNota; set => codigoDaNota = value; }
        public object NumPedidoDeEntrega { get => numPedidoDeEntrega; set => numPedidoDeEntrega = value; }
        public object ValorDaNota { get => valorDaNota; set => valorDaNota = value; }
        public object ModoDeEntrega { get => modoDeEntrega; set => modoDeEntrega = value; }
        public object TipoDeEntrega { get => tipoDeEntrega; set => tipoDeEntrega = value; }
        public object Frete { get => frete; set => frete = value; }
    }
}
