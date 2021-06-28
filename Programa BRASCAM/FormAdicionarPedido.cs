using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Programa_BRASCAM
{
    public partial class FormAdicionarPedido : Form
    {
        bool arrastar = false;
        int posiçãoX, posiçãoY;
        public int CodigoDaNota;
        public int Tipo;
        public int Status;
        public string Cliente;
        public int NumeroDaNota;
        public int NumeroDoPedido;
        public int ReceberDinheiro;
        public object Frete;
        public string CodCliente;
        public DateTime DataDaNota;

        public int Notas = 0;
        public FormAdicionarPedido()
        {
            InitializeComponent();
            comboBox1.Text = comboBox1.Items[1].ToString();
            comboBox2.Text = comboBox2.Items[1].ToString();
            checkBox8.Checked = true;
            CarregarClientes();
           
        }       

        void CarregarClientes() 
        {
            string[] listaClientes = new string[Program.Clientes.Count];

            for (int n = 0; n < Program.Clientes.Count; n++)
            {
                listaClientes[n] = Program.Clientes[n];
            }

            textBox1.AutoCompleteCustomSource.AddRange(listaClientes);
        }


        //ComboBox
        private void comboBoxTextChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string text = comboBox.Text;

            if (comboBox.Items.Contains(comboBox.Text)==false)
            {
                if (comboBox == comboBox1)
                {
                    comboBox.Text = comboBox.Items[0].ToString();
                }
                else
                {
                    comboBox.Text = comboBox.Items[1].ToString();
                }
            }
          
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
            caneta.Width = 1;

            foreach (Control controle in Controls)
            {
                if (controle.Name.Contains("textBox"))
                {
                    SolidBrush solidBrush = new SolidBrush(Color.White);

                    TextBox txt = (TextBox)controle;
                    txt.BorderStyle = BorderStyle.None;
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width+1, controle.Height - 1+7);
                    Rectangle rect2 = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1+7);
                    //e.Graphics.DrawRectangle(caneta, rect);
                    e.Graphics.FillRectangle(solidBrush, rect2);
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
        private void button1_Click(object sender, EventArgs e)
        {

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

        //Receber dinheiro?
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked == true) { checkBox1.Checked = false; }
            else if (checkBox8.Checked == false) { checkBox1.Checked = true; }
        }
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { checkBox8.Checked = false; }
            else if (checkBox1.Checked == false) { checkBox8.Checked = true; }
        }

        //Fechar
        private void Close_button_Click(object sender, EventArgs e)
        {
            Close();
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
      

        //Confirmar
        private void button4_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(comboBox1.Text)==false & String.IsNullOrEmpty(textBox1.Text)==false 
                & String.IsNullOrEmpty(textBox6.Text)==false & String.IsNullOrEmpty(comboBox2.Text)==false)
            {
                try
                {
                    NumeroDoPedido = Convert.ToInt32(textBox6.Text);
                }
                catch
                {
                    MessageBox.Show("O número do pedido deve conter apenas números.");
                    return;
                }

                Cliente = textBox1.Text;

                try
                {
                    if (Cliente != null | Cliente != "")
                    {

                        DateTime data = DateTime.Today.AddDays(-60);
                        string dataFilter = data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");

                        SqlConnection connection = new SqlConnection(Properties.Settings.Default.RSYS2003ConnectionString);

                        SqlCommand command = new SqlCommand("SELECT CodDoCliente FROM BRASCAMpedDeEntrega WHERE " +
                            "(DataDaNota>='" + dataFilter + "' AND NumDoPedido=" +
                            textBox6.Text + ") ", connection);
                        try
                        {
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {                               
                                try
                                {
                                    CodCliente = reader.GetString(0);
                                }
                                catch { }                                
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("Erro de conexão");
                            Close();
                            return;
                        }
                    }
                }
                catch { MessageBox.Show("Não foi possível encontrar o cliente especificado");return; }

                if (checkBox8.Checked == true)
                {
                    ReceberDinheiro = 1;
                }
                else if (checkBox1.Checked == true)
                {
                    ReceberDinheiro = 0;
                }

                if (comboBox1.Text.Contains(comboBox1.Items[0].ToString())) { Tipo = 1; }
                else if (comboBox1.Text.Contains(comboBox1.Items[1].ToString())) { Tipo = 2; }
                else if (comboBox1.Text.Contains(comboBox1.Items[2].ToString())) { Tipo = 3; }
                else if (comboBox1.Text.Contains(comboBox1.Items[3].ToString())) { Tipo = 4; }
                else if (comboBox1.Text.Contains(comboBox1.Items[4].ToString())) { Tipo = 5; }

                if (comboBox2.Text.Contains(comboBox2.Items[0].ToString())) { Status = 1; }
                else if (comboBox2.Text.Contains(comboBox2.Items[1].ToString())) { Status = 2; }
                else if (comboBox2.Text.Contains(comboBox2.Items[2].ToString())) { Status = 3; }
                else if (comboBox2.Text.Contains(comboBox2.Items[3].ToString())) { Status = 4; }

                DialogResult = DialogResult.OK;
            }
            else { MessageBox.Show("Preencha todos os campos."); }            
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        //Encontrar entidade por n° do pedido
        private void textBox6_Leave(object sender, EventArgs e)
        {
            
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Thread buscar = new Thread(ProcurarCliente);
            buscar.Start();
        }
        void ProcurarCliente()
        {
            if (textBox6.Text != String.Empty)
            {
                Notas = 0;

                DateTime data = DateTime.Today.AddDays(-60);
                string dataFilter = data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");

                SqlConnection connection = new SqlConnection(Properties.Settings.Default.RSYS2003ConnectionString);

                SqlCommand command = new SqlCommand("SELECT Cliente, NumDaNota, CodigoDaNota, Frete, DataDaNota FROM BRASCAMpedDeEntrega WHERE " +
                    "(DataDaNota>='" + dataFilter + "' AND NumDoPedido=" +
                    textBox6.Text + ") ", connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            textBox1.Invoke((MethodInvoker)delegate { textBox1.Text = reader.GetString(0); });
                        }
                        catch { }
                        try
                        {
                            NumeroDaNota = reader.GetInt32(1);                            
                        }
                        catch { }
                        try
                        {
                            CodigoDaNota = reader.GetInt32(2);                            
                        }
                        catch { }
                        try
                        {
                            Frete = reader.GetValue(3);                            
                        }
                        catch { Frete=0; }
                        try
                        {
                            DataDaNota = reader.GetDateTime(4);                            
                        }
                        catch { }

                        Notas++;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão");
                    Close();
                    return;
                }
            }         
        }

        //Bloquear Caracteres       
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            string permitidos = "0123456789";

            if (Char.IsControl(e.KeyChar)) 
            {
                e.Handled = false;
            
            }

            else if (permitidos.Contains(e.KeyChar.ToString()) == false)
            {
                e.Handled = true;
            }
            
        }

    }
}
