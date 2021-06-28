using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Programa_BRASCAM
{
    public partial class FormLogin : Form
    {
        string id, senha;
        bool arrastar = false;
        int posiçãoX, posiçãoY, posiçãoFX, posiçãoFY;
        

        public string Id { get => id; set => id = value; }
        public string Senha { get => senha; set => senha = value; }

        public FormLogin()
        {
            InitializeComponent();            
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            
        }

        //fechar
        private void button2_Click(object sender, EventArgs e)
        {
            Program.result = DialogResult.Cancel;
        }

        //Conectar
        private void button1_Click(object sender, EventArgs e)
        {
            id = textBox1.Text;
            senha = Encrypt(textBox2.Text);            
            Conectar();

        }
        void Conectar()
        {
            Program.StartLoading();

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            string commandString = "Select * from BRASCAMusuarios where ID='" + textBox1.Text + "' AND Senha='" + senha+"'";
            SqlCommand command = new SqlCommand(commandString, connection);
            SqlDataReader dataReader;
            
            try
            {
                connection.Open();
                dataReader = command.ExecuteReader();               
                Opacity = 0.0f;
                Enabled = false;                

                if (dataReader.HasRows == false)
                {
                    Program.EndLoading();                    
                    MessageBox.Show("Usuário ou senha inválidos");
                    Program.result = DialogResult.Retry;
                }
                else
                {
                    Programa_BRASCAM.Properties.Settings.Default.Usuário = textBox1.Text;
                    Programa_BRASCAM.Properties.Settings.Default.Senha = textBox2.Text;

                    while (dataReader.Read())
                    {
                        Programa_BRASCAM.Properties.Settings.Default.Permissão = dataReader.GetInt32(1);
                    }

                    Programa_BRASCAM.Properties.Settings.Default.Save();

                    Program.result = DialogResult.OK;
                    ShowInTaskbar = false;
                }

            } 
            catch (SqlException ex)
            {
                Program.EndLoading();                
                MessageBox.Show("Não foi possível se conectar ao servidor");
                Program.result = DialogResult.Retry;

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

            
            for(int i=0;i<senhaArray.Count();i++)
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
                       
           for(int i=0;i<senhaEncryptArray.Count();i++)
            {
                int posição = charList.IndexOf(senhaEncryptArray[i]) - i - 7;
                if (posição < 0) { posição = posição + charList.Count; }
                senhaEncryptArray[i] = charList[posição];                
            }
            senhaEncryptArray.RemoveRange(senhaEncryptArray.Count() - 7, 7);
            senha = String.Concat(senhaEncryptArray);
            return senha;
        }

        //Clicar e arrastar
        private void Arrastar_MouseDown(object sender, MouseEventArgs e)
        {
            arrastar = true;
            posiçãoX = e.X;
            posiçãoY = e.Y;
        }
        private void Arrastar_MouseUp(object sender, MouseEventArgs e)
        {
            arrastar = false;
        }
        private void Arrastar_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastar == true)
            {
                Location = new Point(MousePosition.X - posiçãoX, MousePosition.Y - posiçãoY);
            }
        }

        //VerSenha
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.PasswordChar != '*')
            {
                textBox2.PasswordChar = '*';
            }
            else { textBox2.PasswordChar='\0'; }
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



    }
}
