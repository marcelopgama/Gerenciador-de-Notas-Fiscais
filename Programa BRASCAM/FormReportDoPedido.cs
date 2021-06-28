using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Programa_BRASCAM
{
    public partial class FormReportDoPedido : Form
    {
        public string protocolo;
        bool arrastar = false;
        int posiçãoX, posiçãoY;

        public FormReportDoPedido()
        {
            InitializeComponent();

           
        }

        private void FormRelatórioDoPedido_Load(object sender, EventArgs e)
        {
            ReportDataSource dataSource1 = CarregarDataSource1();
            ReportDataSource dataSource2 = CarregarDataSource2();
            ReportDataSource dataSource3 = CarregarDataSource3();

            reportViewer1.LocalReport.ReportPath = Path.Combine(Application.StartupPath, "ReportPedido.rdlc");           
            reportViewer1.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;

            reportViewer1.LocalReport.DataSources.Add(dataSource1);
            reportViewer1.LocalReport.DataSources.Add(dataSource2);
            reportViewer1.LocalReport.DataSources.Add(dataSource3);

            reportViewer1.RefreshReport();
            reportViewer1.Show();

            panel1.Controls.Add(reportViewer1);
            reportViewer1.Dock = DockStyle.Fill;
            PageSettings settings = new PageSettings();
            settings.Margins.Bottom = 0;
            settings.Margins.Left = 0;
            settings.Margins.Right = 0;
            settings.Margins.Top = 0;
            settings.PrinterSettings.DefaultPageSettings.Margins.Bottom = 0;
            settings.PrinterSettings.DefaultPageSettings.Margins.Left = 0;
            settings.PrinterSettings.DefaultPageSettings.Margins.Right = 0;
            settings.PrinterSettings.DefaultPageSettings.Margins.Top = 0;

            reportViewer1.SetPageSettings(settings);
        }

        ReportDataSource CarregarDataSource1() 
        {
            string dataFilter;

            DateTime data = DateTime.Today.AddDays(-60);
            dataFilter = data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            SqlCommand command = new SqlCommand("SELECT Protocolo, UsuarioEmissor, " +
                "DataDoPedido, ResponsavelPelaEntrega, Tipo, DataDeEntrega, StatusDoPedido, Veiculo, " +
                "Quilometragem, Frete, Transportadora, Observacao FROM dbo.BRASCAMpedidos WHERE " +
                "DataDoPedido>='" + dataFilter + "' AND Protocolo=" + protocolo, connection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

            DataTable data1 = new DataTable();
            sqlDataAdapter.Fill(data1);

            ReportDataSource dataSource1 = new ReportDataSource("DataSet1", data1);

            return dataSource1;
        }
        ReportDataSource CarregarDataSource2()
        {   string dataFilter;

            DateTime data = DateTime.Today.AddDays(-60);
            dataFilter = data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            SqlCommand command = new SqlCommand("SELECT CodigoDaNota, NumDaNota, NumDoPedido, " +
               "CodigoPedidoDeEntrega, DataDaNota, DataDeEntrega, CodDoCliente, StatusDoPedido," +
               " Tipo, ReceberDinheiro, ValorDaNota, Cliente, Responsavel,TipoDeEntrega,ModoDeEntrega FROM BRASCAMnotas WHERE " +
               "CodigoPedidoDeEntrega="+protocolo, connection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

            DataTable data2 = new DataTable();
            sqlDataAdapter.Fill(data2);

            ReportDataSource dataSource2 = new ReportDataSource("DataSet2", data2);

            return dataSource2;
        }
        ReportDataSource CarregarDataSource3()
        {
            string dataFilter;

            DateTime data = DateTime.Today.AddDays(-60);
            dataFilter = data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            SqlCommand command = new SqlCommand("SELECT CodigoDaNota, NumDaNota, NumDoPedido, " +
               "CodigoPedidoDeEntrega, DataDaNota, DataDeEntrega, CodDoCliente, StatusDoPedido," +
               " Tipo, ReceberDinheiro, ValorDaNota, Cliente, Responsavel,TipoDeEntrega,ModoDeEntrega FROM BRASCAMpedDeEntrega WHERE " +
               "CodigoPedidoDeEntrega=" + protocolo, connection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

            DataTable data3 = new DataTable();
            sqlDataAdapter.Fill(data3);

            ReportDataSource dataSource3 = new ReportDataSource("DataSet3", data3);

            return dataSource3;
        }
        //Clicar e arrastar
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            arrastar = true;
            posiçãoX = e.X;
            posiçãoY = e.Y;
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            arrastar = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastar == true)
            {
                Location = new Point(MousePosition.X - posiçãoX, MousePosition.Y - posiçãoY);
            }
        }

    }
}
