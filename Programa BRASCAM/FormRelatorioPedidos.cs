using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop;
using System.Data.SqlClient;
using System.Threading;

namespace Programa_BRASCAM
{
    public partial class FormRelatorioPedidos : Form
    {
        bool arrastar = false;
        int posiçãoX, posiçãoY;
        string pasta, endereço;
        DataView pedidos;
        public DataView Pedidos { get => pedidos; set => pedidos = value; }

        public FormRelatorioPedidos()
        {
            InitializeComponent();            
        }
        private void FormRelatorioPedidos_Load(object sender, EventArgs e)
        {
            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet8.BRASCAMveiculos'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMveiculosTableAdapter.Fill(this.rSYS2003DataSet8.BRASCAMveiculos);
            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet7.BRASCAMresponsavel'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMresponsavelTableAdapter.Fill(this.rSYS2003DataSet7.BRASCAMresponsavel);
            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet6.BRASCAMusuarios'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMusuariosTableAdapter.Fill(this.rSYS2003DataSet6.BRASCAMusuarios);

            comboBox1.Text = String.Empty;
            comboBox2.Text = String.Empty;
            comboBox3.Text = String.Empty;
            comboBox4.Text = String.Empty;
        }

        //Paint
        private void DesenharLinhaInferior(object sender, PaintEventArgs e)
        {
            Label label = (Label)sender;

            Pen caneta = new Pen(Color.FromArgb(179, 179, 179));
            caneta.Width = 3;
            e.Graphics.DrawLine(caneta, 0, label.Height - 1, label.Width, label.Height - 1);

        }
        private void BordaCustomizada(object sender, PaintEventArgs e)
        {
            Pen caneta = new Pen(Color.FromArgb(208, 208, 209));
            caneta.Width = 3;

            foreach (Control controle in Controls)
            {
                if (controle.Name.Contains("textBox"))
                {
                    TextBox txt = (TextBox)controle;
                    txt.BorderStyle = BorderStyle.None;
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);
                }
                else if (controle.Name.Contains("comboBox"))
                {

                    ComboBox txt = (ComboBox)controle;
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);

                }
                else if (controle.Name.Contains("dataGridView"))
                {
                    DataGridView txt = (DataGridView)controle;
                    txt.BorderStyle = BorderStyle.None;
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);
                }

            }
        }

        //Aumentar e diminuir botão
        private void Apmliar_button_MouseEnter(object sender, EventArgs e)
        {
            Button botão = (Button)sender;
            botão.Size = new Size(botão.Size.Width + 4, botão.Size.Height + 4);
            botão.Location = new Point(botão.Location.X - 2, botão.Location.Y - 2);
        }
        private void Ampliar_button_MouseLeave(object sender, EventArgs e)
        {
            Button botão = (Button)sender;
            botão.Size = new Size(botão.Size.Width - 4, botão.Size.Height - 4);
            botão.Location = new Point(botão.Location.X + 2, botão.Location.Y + 2);

        }

        //Fechar
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
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
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastar == true)
            {
                Location = new Point(MousePosition.X - posiçãoX, MousePosition.Y - posiçãoY);                
            }
        }

        //CheckChanged
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            textBox11.ReadOnly=!textBox11.ReadOnly;
            textBox12.ReadOnly = !textBox12.ReadOnly;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox21.ReadOnly = !textBox21.ReadOnly;
            textBox22.ReadOnly = !textBox22.ReadOnly;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox31.ReadOnly = !textBox31.ReadOnly;
            textBox32.ReadOnly = !textBox32.ReadOnly;
        }
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            textBox81.ReadOnly = !textBox81.ReadOnly;
            textBox82.ReadOnly = !textBox82.ReadOnly;
        }
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            textBox91.ReadOnly = !textBox91.ReadOnly;
            textBox92.ReadOnly = !textBox92.ReadOnly;
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !comboBox1.Enabled;
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = !comboBox2.Enabled;
        }
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = !comboBox3.Enabled;
        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            comboBox4.Enabled = !comboBox4.Enabled;
        }

        //Gerar Relatório        
        DataView Filtrar()
        {
            DataTable data = pedidos.ToTable();

            DataView dataView = new DataView();
            dataView = data.AsDataView();           

            if (checkBox1.Checked == true)//N° do Pedido
            {
                if(textBox11.Text!=String.Empty && textBox12.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Protocolo>=" + textBox11.Text +" AND Protocolo<="+ textBox12.Text; } catch { }
                }
                else if (textBox11.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Protocolo>=" + textBox11.Text; } catch { }
                }
                else if (textBox12.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Protocolo<=" + textBox12.Text; } catch { }
                }
                
            }
            if (checkBox2.Checked == true)//Data do Pedido
            {
                if (textBox21.Text != String.Empty && textBox22.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDoPedido>='" + textBox21.Text + "' AND DataDoPedido<='" + textBox22.Text+"'"; } catch { }
                }
                else if (textBox21.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDoPedido>='" + textBox21.Text; } catch { }
                }
                else if (textBox22.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDoPedido<='" + textBox22.Text; } catch { }
                }
            }
            if (checkBox3.Checked == true)//Data de entrega
            {
                if (textBox31.Text != String.Empty && textBox32.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDeEntrega>='" + textBox31.Text + "' AND DataDeEntrega<='" + textBox32.Text+"'"; } catch { }
                }
                else if (textBox31.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDeEntrega>='" + textBox31.Text; } catch { }
                }
                else if (textBox32.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDeEntregao<='" + textBox32.Text; } catch { }
                }
            }
            if (checkBox4.Checked == true)//Usuario
            {
                try { dataView.RowFilter = "UsuarioEmissor='" + comboBox1.Text+"'"; } catch { }                
            }
            if (checkBox5.Checked == true)//Status
            {
                try { dataView.RowFilter = "StatusDoPedido=" + (comboBox2.Items.IndexOf(comboBox2.Text)+1).ToString(); } catch { }
            }
            if (checkBox6.Checked == true)//Responsavel
            {
                try { dataView.RowFilter = "ResponsavelPelaEntrega='" + comboBox3.Text + "'"; } catch { }
            }
            if (checkBox7.Checked == true)//Veiculo
            {
                try { dataView.RowFilter = "Veiculo='" + comboBox4.Text + "'"; } catch { }
            }
            if (checkBox8.Checked == true)//Quilometragem
            {
                if (textBox81.Text != String.Empty && textBox82.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Quilometragem>=" + textBox81.Text + " AND Quilometragem<=" + textBox82.Text; } catch { }
                }
                else if (textBox81.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Quilometragem>=" + textBox81.Text; } catch { }
                }
                else if (textBox82.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Quilometragem<=" + textBox82.Text; } catch { }
                }
            }
            if (checkBox9.Checked == true)//Frete
            {
                if (textBox91.Text != String.Empty && textBox92.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Frete>=" + textBox91.Text + " AND Frete<=" + textBox92.Text; } catch { }
                }
                else if (textBox91.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Frete>=" + textBox91.Text; } catch { }
                }
                else if (textBox92.Text != String.Empty)
                {
                    try { dataView.RowFilter = "Frete<=" + textBox92.Text; } catch { }
                }
            }

            return dataView;

        }
        void Exportar()
        {

            DataView dataView = Filtrar();
            DataTable dataTable = dataView.ToTable("Relatório");
            dataTable.Namespace = "Relatório";


            FormReportDosPedidos form = new FormReportDosPedidos();
            form.fontDeDados = dataTable;
            form.ShowDialog();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            Exportar();
        }

        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

       
    }
}
