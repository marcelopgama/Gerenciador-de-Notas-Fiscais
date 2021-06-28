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
    public partial class FormCadastroResponsavel : Form
    {
        bool arrastar = false, adicionando, editando=false,carregando;
        int posiçãoX, posiçãoY;
        int codigoSelecionado;
        string editarResponsavel;

        public bool Adicionando { get => adicionando; set => adicionando = value; }

        public FormCadastroResponsavel()
        {            
            InitializeComponent();
           
        }
        private void FormCadastroResponsavel_Load(object sender, EventArgs e)
        {
            PreencherDataGrid();
            if (adicionando == true)
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox6.ReadOnly = false;
                dataGridView1.ReadOnly = true;
            }
        }
        void PreencherDataGrid()
        {
            carregando = true;

            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
            string command = "SELECT CodigoDoResponsavel,Nome,Empresa,CNH,Telefone FROM BRASCAMresponsavel WHERE CodigoDoResponsavel IS NOT NULL";

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
            if (numeroDelinhas > 0) { dataGridView1.Rows.Add(numeroDelinhas); }

            for (int n = 0; n < numeroDelinhas; n++)
            {
                dataGridView1.Rows[n].Cells[0].Value = dataTable.Rows[n][0];
                dataGridView1.Rows[n].Cells[1].Value = dataTable.Rows[n][1];
                dataGridView1.Rows[n].Cells[2].Value = dataTable.Rows[n][2];
                dataGridView1.Rows[n].Cells[3].Value = dataTable.Rows[n][3];
                dataGridView1.Rows[n].Cells[4].Value = dataTable.Rows[n][4];
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

        //Fechar
        private void Close_button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            editando = false;
            adicionando = false;
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox6.ReadOnly = true;
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
            if (editando == false & adicionando == false & carregando==false)
            {
                int n = dataGridView1.Rows.IndexOf(dataGridView1.SelectedRows[0]);
                codigoSelecionado = Convert.ToInt32(dataGridView1.Rows[n].Cells[0].Value);

                textBox6.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                textBox1.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            }
            
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (editando == false & adicionando == false)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("Deseja excluir os usuários selecionados?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
                        string commandString = "DELETE FROM BRASCAMresponsavel WHERE CodigoDoResponsavel=" + codigoSelecionado;
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
                    }

                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (editando == false & adicionando==false)
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox6.ReadOnly = false;

                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox6.Text = "";

                adicionando = true;
                dataGridView1.ReadOnly = true;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {   
            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
            string commandString = "";

            if (adicionando == true)
            {
                commandString = "INSERT INTO BRASCAMresponsavel (Nome,Empresa,CNH,Telefone) VALUES"
                    + "('" + textBox6.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox1.Text + "')";
            }
            else if (editando == true)
            {
                commandString = "UPDATE BRASCAMresponsavel SET Nome='" + textBox6.Text + "',Empresa='" + textBox2.Text
                    + "',CNH='" + textBox3.Text + "',Telefone='" + textBox1.Text + "' WHERE CodigoDoResponsavel='" + codigoSelecionado+"'";

            }

            SqlCommand command = new SqlCommand(commandString, connection);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                if (editando == true)
                {
                    commandString = "UPDATE BRASCAMpedidos SET ResponsavelPelaEntrega='" + textBox6.Text + "' WHERE " +
                        "ResponsavelPelaEntrega='" + editarResponsavel + "'";

                    SqlCommand command2 = new SqlCommand(commandString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
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

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox6.ReadOnly = true;

            dataGridView1.ReadOnly = false;


        }
        private void button5_Click(object sender, EventArgs e)
        {

            if (editando != true & adicionando != true & dataGridView1.SelectedRows.Count>0 )
            {
                editando = true;

                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox6.ReadOnly = false;

                editarResponsavel = textBox6.Text;

                dataGridView1.ReadOnly = true;
            }
        }

        //Bloquer caracteres
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            string permitidos = ".,abcdefghijklmnoprstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890- ";

            if (permitidos.Contains(e.KeyChar.ToString()) == true | Char.IsControl(e.KeyChar)) { e.Handled = false; }
            else { e.Handled = true; }
        }
    }
    


}
