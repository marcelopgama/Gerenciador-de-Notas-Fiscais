using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Programa_BRASCAM
{
    public partial class FormCadastroTransportadora : Form
    {
        bool arrastar, editando = false, adicionando, carregando;

        public bool Adicionando { get => adicionando; set => adicionando = value; }

        private void FormCadastroTransportadora_Load(object sender, EventArgs e)
        {
            if (adicionando == true)
            {
                textBox1.ReadOnly = false;
                textBox6.ReadOnly = false;
                dataGridView1.ReadOnly = true;
            }
        }

        int posiçãoX, posiçãoY;
        int codigoSelecionado;

        public FormCadastroTransportadora()
        {
            InitializeComponent();
            PreencherDataGrid();
        }

        void PreencherDataGrid()
        {
            carregando = true;

            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
            string command = "SELECT CodigoDaTransportadora,Transportadora,Telefone FROM BRASCAMtransportadora WHERE CodigoDaTransportadora IS NOT NULL";

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command, connection);
            DataTable dataTable = new DataTable();

            try
            {
                connection.Open();
                dataAdapter.Fill(dataTable);
                connection.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro de conexão");
                Close();
                return;
            }

            int numeroDelinhas = dataTable.Rows.Count;

            if (numeroDelinhas > 0)
            {
                dataGridView1.Rows.Add(numeroDelinhas);
            }

            for (int n = 0; n < numeroDelinhas; n++)
            {

                dataGridView1.Rows[n].Cells[0].Value = dataTable.Rows[n][0].ToString();
                dataGridView1.Rows[n].Cells[1].Value = dataTable.Rows[n][1].ToString();
                dataGridView1.Rows[n].Cells[2].Value = dataTable.Rows[n][2].ToString();
            }

            dataGridView1.ClearSelection();
            carregando = false;
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
                else if (controle.Name.Contains("label"))
                {
                    if (controle.BackColor == Color.White)
                    {
                        Label txt = (Label)controle;
                        txt.BorderStyle = BorderStyle.None;
                        Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                        e.Graphics.DrawRectangle(caneta, rect);

                    }

                }

            }
        }

        //Clicar e arrastar
        private void arrastar_MouseDown(object sender, MouseEventArgs e)
        {
            arrastar = true;
            posiçãoX = e.X;
            posiçãoY = e.Y;
        }
        private void arrastar_MouseUp(object sender, MouseEventArgs e)
        {
            arrastar = false;
        }
        private void arrastar_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastar == true)
            {
                Location = new Point(MousePosition.X - posiçãoX, MousePosition.Y - posiçãoY);
            }
        }

        //Fechar
        private void Close_button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            editando = false; adicionando = false;

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

        //Editar        
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (editando == false & adicionando == false & carregando == false)
            {
                int n = dataGridView1.Rows.IndexOf(dataGridView1.SelectedRows[0]);

                codigoSelecionado = Convert.ToInt32(dataGridView1.Rows[n].Cells[0].Value);

                try
                {

                    try { textBox2.Text = dataGridView1.Rows[n].Cells[0].Value.ToString(); }
                    catch (NullReferenceException ex) { }
                    try { textBox6.Text = dataGridView1.Rows[n].Cells[1].Value.ToString(); }
                    catch (NullReferenceException ex) { }
                    try { textBox1.Text = dataGridView1.Rows[n].Cells[2].Value.ToString(); }
                    catch (NullReferenceException ex) { }
                }
                catch (System.ArgumentOutOfRangeException ex) { }

            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (editando == false & adicionando == false)
            {
                SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
                string commandString = "DELETE FROM BRASCAMtransportadora WHERE CodigoDaTransportadora=" + codigoSelecionado;
                SqlCommand command = new SqlCommand(commandString, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão");
                    Close();
                    return;
                }

                PreencherDataGrid();
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (editando != true & adicionando != true)
            {
                textBox6.ReadOnly = false;
                textBox1.ReadOnly = false;

                textBox6.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";

                adicionando = true;
                dataGridView1.ReadOnly = true;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
            string commandString = "";

            if (adicionando == true)
            {
                commandString = "INSERT INTO BRASCAMtransportadora (Transportadora,Telefone) VALUES"
                    + "('" + textBox6.Text + "','" + textBox1.Text + "')";
            }
            else if (editando == true)
            {
                commandString = "UPDATE BRASCAMtransportadora SET Transportadora='" + textBox6.Text + "',Telefone='" + textBox1.Text
                    + "' WHERE CodigoDaTransportadora='" + codigoSelecionado + "'";

            }
            SqlCommand command = new SqlCommand(commandString, connection);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro de conexão");
                Close();
                return;
            }

            PreencherDataGrid();
            adicionando = false;
            editando = false;
            textBox6.ReadOnly = true;
            textBox1.ReadOnly = true;

            dataGridView1.ReadOnly = false;
        }
        private void button5_Click(object sender, EventArgs e)
        {

            if (editando != true & adicionando != true & dataGridView1.SelectedRows.Count > 0)
            {
                editando = true;

                textBox6.ReadOnly = false;
                textBox1.ReadOnly = false;

                dataGridView1.ReadOnly = true;
            }
        }

        //Bloquear caracteres
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsLetterOrDigit(e.KeyChar) == true | Char.IsPunctuation(e.KeyChar)
                | Char.IsWhiteSpace(e.KeyChar) | Char.IsControl(e.KeyChar)) & e.KeyChar.ToString() != "'")
            {
                e.Handled = false;
            }
            else { e.Handled = true; }
        }

    }
}
