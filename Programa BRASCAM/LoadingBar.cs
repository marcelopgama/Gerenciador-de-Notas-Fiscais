using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Programa_BRASCAM
{
    public partial class LoadingBar : UserControl
    {
        public int Porcentagem;

        public LoadingBar()
        {
            InitializeComponent();
            progressBar1.BackColor = Color.White;
        }
        private void LoadingBar_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = Porcentagem.ToString() + "%";
        }

       
    }
}
