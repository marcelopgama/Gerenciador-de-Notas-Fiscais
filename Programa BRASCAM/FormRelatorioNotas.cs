using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Office.Interop;
using System.Threading;

namespace Programa_BRASCAM
{
    public partial class FormRelatorioNotas : Form
    {
        bool arrastar = false;
        int posiçãoX, posiçãoY;
        DataView notas;
        
        public DataView Notas { get => notas; set => notas = value; }

        public FormRelatorioNotas()
        {
            InitializeComponent();
        }

        private void FormRelatorioNotas_Load(object sender, EventArgs e)
        {            
            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet9.BRASCAMresponsavel'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMresponsavelTableAdapter.Fill(this.rSYS2003DataSet9.BRASCAMresponsavel);

            comboBox1.Items.AddRange(Program.Clientes.ToArray());

            comboBox1.Text = string.Empty;
            comboBox2.Text = string.Empty;
            comboBox3.Text = string.Empty;
        }

        //Paint
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

            }
        }
        private void DesenharLinhaInferior(object sender, PaintEventArgs e)
        {
            Label label = (Label)sender;

            Pen caneta = new Pen(Color.FromArgb(179, 179, 179));
            caneta.Width = 3;
            e.Graphics.DrawLine(caneta, 0, label.Height-1, label.Width, label.Height-1);

        }

        //Ampliar e diminuir botão
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
            textBox41.ReadOnly = !textBox41.ReadOnly;
            textBox42.ReadOnly = !textBox42.ReadOnly;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox11.ReadOnly = !textBox11.ReadOnly;
            textBox12.ReadOnly = !textBox12.ReadOnly;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !comboBox1.Enabled;
        }               
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = !comboBox3.Enabled;
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            textBox51.ReadOnly = !textBox51.ReadOnly;
            textBox52.ReadOnly = !textBox52.ReadOnly;
        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = !comboBox2.Enabled;
        }

     
        DataView Filtrar()
        {
            DataTable data = notas.ToTable();

            DataView dataView = new DataView();
            dataView = data.AsDataView();

            if (checkBox1.Checked == true)//N° do Pedido
            {               

                 if (textBox11.Text != String.Empty && textBox12.Text != String.Empty)
                {
                    try { dataView.RowFilter = "NumDaNota>=" + textBox11.Text+" AND NumDaNota<="+ textBox12.Text; } catch { }
                }
                else if (textBox11.Text != String.Empty)
                {
                    try { dataView.RowFilter = "NumDaNota>=" + textBox11.Text; } catch { }
                }
                else if (textBox12.Text != String.Empty)
                {
                    try { dataView.RowFilter = "NumDaNota<=" + textBox12.Text; } catch { }
                }                              
                
            }
            if (checkBox2.Checked == true)//Cliente
            {
                try { dataView.RowFilter = "Cliente='" + comboBox1.Text + "'"; } catch { }               
            }
            if (checkBox3.Checked == true)//Entregador
            {
                try { dataView.RowFilter = "Responsavel>='" + comboBox3.Text + "'"; } catch { }
            }
            if (checkBox4.Checked == true)//Dada da NF
            {
                if (textBox41.Text != String.Empty && textBox42.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDaNota>='" + textBox41.Text + "' AND DataDaNota<='" + textBox42.Text+"'"; } catch { }
                }
                else if (textBox41.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDaNota>='" + textBox41.Text+"'"; } catch { }
                }
                else if (textBox42.Text != String.Empty)
                {
                    try { dataView.RowFilter = "DataDaNota<='" + textBox42.Text + "'"; } catch { }
                }
            }
            if (checkBox5.Checked == true)//Valor
            {
                if (textBox51.Text != String.Empty && textBox52.Text != String.Empty)
                {
                    try { dataView.RowFilter = "ValorDaNota>=" + textBox51.Text + " AND ValorDaNota<=" + textBox52.Text; } catch { }
                }
                else if (textBox51.Text != String.Empty)
                {
                    try { dataView.RowFilter = "ValorDaNota>=" + textBox51.Text; } catch { }
                }
                else if (textBox52.Text != String.Empty)
                {
                    try { dataView.RowFilter = "ValorDaNota<=" + textBox52.Text; } catch { }
                }
            }
            if (checkBox6.Checked == true)//Responsavel
            {
                try { dataView.RowFilter = "StatusDoPedido=" + (comboBox2.Items.IndexOf(comboBox2.Text) + 1).ToString(); } catch { }
            }
            return dataView;

        }
        void Exportar()
        {

            DataView dataView = Filtrar();
            DataTable dataTable = dataView.ToTable("Relatório");
            dataTable.Namespace = "Relatório";

            FormReportDasNotas form = new FormReportDasNotas();
            form.fontDeDados = dataTable;
            form.ShowDialog();
        }

        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            Exportar();
        }

    }
}
