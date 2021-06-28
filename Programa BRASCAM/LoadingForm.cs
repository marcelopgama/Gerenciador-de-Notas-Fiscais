using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Programa_BRASCAM
{
    public partial class LoadingForm : Form
    {
        int porcentagem;
        public int Porcentagem { get => porcentagem; set => porcentagem = value; }

        string id, senha;

        public string Id { get => id; set => id = value; }
        public string Senha { get => senha; set => senha = value; }

        public LoadingForm()
        {
            InitializeComponent();
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            timer1.Start();      
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label2.Text != "LOADING...")
            {
                label2.Text += ".";
            }
            else 
            {
                label2.Text = "LOADING";
            }

        }
    }
}
