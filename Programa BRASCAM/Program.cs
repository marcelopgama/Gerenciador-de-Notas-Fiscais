using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data;

namespace Programa_BRASCAM
{
    static class Program
    {
        public static List<string> Clientes;
        public static List<string> CodClientes;        
        public static DataView Notas;
        public static DataView Pedidos;
        public static DataView PedidosDeEntrega;
        public static LoadingForm loadingForm;
        public static DialogResult result;
        public static List<string> Colunas1;
        public static List<string> Colunas2;

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
              Application.Run(new FormPrincipal());
              
            }
            catch { return; } 
        } 
        
        public static void StartLoading() 
        {
            Task.Run(() => 
            {
                loadingForm = new LoadingForm();
                loadingForm.ShowDialog();
            });
            
        }
        public static void EndLoading() 
        {
            Program.loadingForm.Invoke((MethodInvoker)delegate
            {
                
              loadingForm.Close();
               
            });
        }
    }
   
}
