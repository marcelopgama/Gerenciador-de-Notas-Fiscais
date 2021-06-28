using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Programa_BRASCAM
{
    public partial class FormCadastroUsuario : Form
    {
        bool arrastar, editando = false, carregando;
        public bool adicionando = false;
        int posiçãoX, posiçãoY;
        string usuarioSelecionado;
        int permissão;
        List<Usuários> usuários;

        string senhaTemp;

        public FormCadastroUsuario()
        {
            InitializeComponent();

            permissão = Properties.Settings.Default.Permissão;

            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;



        }
        private void FormCadastroUsuario_Load(object sender, EventArgs e)
        {
            PreencherDataGrid();

            

        }
        void PreencherDataGrid()
        {
            carregando = true;

            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
            string command = "SELECT ID,Permissao,Senha FROM BRASCAMUsuarios";
            
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
                dataGridView1.Rows[n].Cells[2].Value = dataTable.Rows[n][2].ToString();               

                if (Convert.ToInt32(dataTable.Rows[n][1].ToString()) == 1)
                {
                    dataGridView1.Rows[n].Cells[1].Value = "Editor";
                }
                else if (Convert.ToInt32(dataTable.Rows[n][1].ToString()) == 2)
                {
                    dataGridView1.Rows[n].Cells[1].Value = "Administrador";
                }
                else if (Convert.ToInt32(dataTable.Rows[n][1].ToString()) == 3)
                {
                    dataGridView1.Rows[n].Cells[1].Value = "Visualizador";
                }

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
            this.Close();
            editando = false;adicionando = false;
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
                int n = dataGridView1.SelectedRows[0].Index;
                usuarioSelecionado = dataGridView1.Rows[n].Cells[0].Value.ToString();

                textBox6.Text = dataGridView1.Rows[n].Cells[0].Value.ToString();

                if (dataGridView1.SelectedRows[0].Cells[1].Value.ToString() == "Editor")
                {
                    checkBox1.CheckState = CheckState.Checked;
                    checkBox2.CheckState = CheckState.Unchecked;
                    checkBox3.CheckState = CheckState.Unchecked;
                }
                else if (dataGridView1.Rows[n].Cells[1].Value.ToString() == "Administrador")
                {
                    checkBox1.CheckState = CheckState.Unchecked;
                    checkBox2.CheckState = CheckState.Checked;
                    checkBox3.CheckState = CheckState.Unchecked;
                }
                else if (dataGridView1.Rows[n].Cells[1].Value.ToString() == "Visualizador")
                {
                    checkBox1.CheckState = CheckState.Unchecked;
                    checkBox2.CheckState = CheckState.Unchecked;
                    checkBox3.CheckState = CheckState.Checked;
                }

                textBox1.Text= Decrypt(dataGridView1.Rows[n].Cells[2].Value.ToString());
                Console.WriteLine(Decrypt(dataGridView1.Rows[n].Cells[2].Value.ToString()));
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (editando == false & adicionando == false & permissão==2)
            {
                SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
                string commandString = "DELETE FROM BRASCAMusuarios WHERE ID='" + usuarioSelecionado+"'";
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
        private void button2_Click(object sender, EventArgs e)
        {
            if (editando != true & adicionando != true & permissão==2)
            {
                adicionando = true;
                textBox6.ReadOnly = false;
                textBox1.ReadOnly = false;
                textBox6.Text = "";
                textBox1.Text = "";
                checkBox1.Checked = true;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                textBox1.UseSystemPasswordChar = false;                
                dataGridView1.ReadOnly = true;

                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (editando == true | adicionando == true)
            {

                string editarUsuario = usuarioSelecionado;

                int Permissão = 0;

                if (checkBox1.Checked == true) { Permissão = 1; }
                else if (checkBox2.Checked == true) { Permissão = 2; }
                else if (checkBox3.Checked == true) { Permissão = 3; }

                if(textBox1.Text=="" || textBox1.Text == String.Empty) 
                {
                    MessageBox.Show("A senha não pode ser nula");
                    return;
                }                

                string senhaCriptografada = Encrypt(textBox1.Text);
                Console.WriteLine(senhaCriptografada);

                SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
                string commandString = "";

                if (adicionando == true)
                {
                    commandString = "INSERT INTO BRASCAMusuarios (ID,Senha,Permissao) VALUES"
                       + "('" + textBox6.Text + "','" + senhaCriptografada + "'," + Permissão + ")";
                }
                else if (editando == true)
                {

                    commandString = "UPDATE BRASCAMusuarios SET ID='" + textBox6.Text + "',Senha='" + senhaCriptografada
                   + "',Permissao=" + Permissão + " WHERE ID='" + usuarioSelecionado + "'";
                }


                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(commandString, connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                    string commandString2 = "UPDATE BRASCAMpedidos SET UsuarioEmissor='" + textBox6.Text + "' WHERE " +
                            "UsuarioEmissor='" + editarUsuario + "'";

                    SqlCommand command2 = new SqlCommand(commandString2, connection);
                    connection.Open();
                    command2.ExecuteNonQuery();
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
                textBox1.ReadOnly = true;
                textBox6.ReadOnly = true;
                textBox1.UseSystemPasswordChar = true;
                dataGridView1.ReadOnly = false;

                if (button5.Text == "Cancelar") { button5.Text = "Editar"; }

                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {        

            //Editar como administrador
            if (editando != true & adicionando != true & dataGridView1.SelectedRows.Count > 0 & permissão==2)
            {
                editando = true;                
                textBox6.ReadOnly = false;                
                textBox1.ReadOnly = false;
                dataGridView1.ReadOnly = true;
                textBox1.UseSystemPasswordChar = false;
                senhaTemp = textBox1.Text;
                textBox1.Text = "";
                if (button5.Text == "Editar") { button5.Text = "Cancelar"; }

                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;

            }
            //Editar como outros usuários
            else if(editando != true & adicionando != true & dataGridView1.SelectedRows.Count > 0 & permissão != 2)
                  {
                if (dataGridView1.SelectedRows[0].Cells[0].Value.ToString() == Properties.Settings.Default.Usuário)
                {

                    editando = true;
                    textBox6.ReadOnly = false;
                    textBox1.ReadOnly = false;
                    dataGridView1.ReadOnly = true;
                    textBox1.UseSystemPasswordChar = false;
                    senhaTemp = textBox1.Text;
                    textBox1.Text = "";
                    if (button5.Text == "Editar") { button5.Text = "Cancelar"; }

                    if(permissão==1)
                    {
                        checkBox1.Enabled = true;
                        checkBox2.Enabled = true;
                        checkBox3.Enabled = true;
                    }
                    else
                    {
                        checkBox1.Enabled = false;
                        checkBox2.Enabled = false;
                        checkBox3.Enabled = false;
                    }
                }               
            }
            else if (editando==true) 
            {
                editando = false;
                textBox6.ReadOnly = true;
                textBox1.ReadOnly = true;
                dataGridView1.ReadOnly = false;
                textBox1.UseSystemPasswordChar = true;
                textBox1.Text = senhaTemp;
                if (button5.Text == "Cancelar") { button5.Text = "Editar"; }

                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
            }
        }

        //Criptografia
        string Encrypt(string senha)
        {
            string novaSenha = "";

            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            List<char> charList = caracteres.ToList();

            string adicional = "j35Doy9";
            senha = senha + adicional;
            char[] senhaArray = senha.ToCharArray();


            for (int i = 0; i < senhaArray.Count(); i++)
            {
                int posição = i + charList.IndexOf(senhaArray[i]) + 7;
                if (posição >= charList.Count) { posição = posição - charList.Count; }
                senhaArray[i] = charList[posição];
            }
            novaSenha = String.Concat(senhaArray);
            return novaSenha;
        }
        string Decrypt(string senhaEncryptada)
        {
            string senha = "";
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            List<char> charList = caracteres.ToList();

            List<char> senhaEncryptArray = senhaEncryptada.ToCharArray().ToList();

            for (int i = 0; i < senhaEncryptArray.Count(); i++)
            {
                int posição = charList.IndexOf(senhaEncryptArray[i]) - i - 7;
                if (posição < 0) { posição = posição + charList.Count; }
                senhaEncryptArray[i] = charList[posição];
            }
            senhaEncryptArray.RemoveRange(senhaEncryptArray.Count() - 7, 7);
            senha = String.Concat(senhaEncryptArray);
            return senha;
        }

        //Permissão       
        public void CheckClick(object sender, MouseEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (editando == true | adicionando == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox.Checked = true;
            }
        }

        //Bloquear caracteres
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            string permitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] charPermitidos = permitidos.ToCharArray();

            if (charPermitidos.Contains(e.KeyChar) == true | Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else { e.Handled = true; }
        }
    }
}
