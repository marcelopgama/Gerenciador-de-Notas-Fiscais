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
    public partial class FormSelecionarNotas : Form
    {
        bool arrastar = false;
        int posiçãoX, posiçãoY;       

        List<NotaFiscal> notas;
        List<int> notasSelecionadas = new List<int>();     
        List<int> notasDuplicadas;
        public List<int> NotasSelecionadas { get => notasSelecionadas; set => notasSelecionadas = value; }
        public List<int> NotasDuplicadas { get => notasDuplicadas; set => notasDuplicadas = value; }

        List<DataGridViewRow> Rows;
        
        public FormSelecionarNotas()
        {
            InitializeComponent();
            
            
            
        }
        private void FormSelecionarNotas_Load(object sender, EventArgs e)
        {
           // Rows = Program.linhasDataGridView1.ToList();
            //CarregarNotas(Rows);
            PreencherDataGrid();
        }
             
        void CarregarNotas(List<DataGridViewRow> Linhas) 
        {
            dataGridView1.Rows.Clear();
            int n = 0;

            foreach (DataGridViewRow row in Linhas)
            {
                dataGridView1.Rows.Add();

                dataGridView1.Rows[n].Cells[1].Value = row.Cells[1].Value;//N° da nota
                dataGridView1.Rows[n].Cells[2].Value = row.Cells[2].Value;// n° do pedido
                dataGridView1.Rows[n].Cells[3].Value = Convert.ToDateTime(row.Cells[3].Value).ToShortDateString();//data da nota
                dataGridView1.Rows[n].Cells[4].Value = row.Cells[4].Value;//cod do cliente
                dataGridView1.Rows[n].Cells[6].Value = row.Cells[8].Value;//Status int
                dataGridView1.Rows[n].Cells[7].Value = row.Cells[9].Value;//Codigo da nota                

                Console.WriteLine(row.Cells[1].Value.ToString());

                if (NotasDuplicadas.Contains(Convert.ToInt32(row.Cells[9].Value)))
                {
                    DataGridViewCheckBoxCell checkBoxCell = new DataGridViewCheckBoxCell();
                    checkBoxCell = dataGridView1.Rows[n].Cells[0] as DataGridViewCheckBoxCell;
                    checkBoxCell.Value = true;
                }

                if (notasSelecionadas.Contains(Convert.ToInt32(dataGridView1.Rows[n].Cells[7].Value)) == true) 
                {
                    dataGridView1.Rows[n].Cells[0].Value = true;

                }

                n++;
            }

        }

        void PreencherDataGrid()
        {
           
           foreach (DataGridViewColumn column in dataGridView1.Columns)
           {
               if (column.Index > 5) { column.Visible = false; }
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToInt32(row.Cells[6].Value) == 1)
                {
                    row.Cells[5].Value = "Pendente de envio";
                    row.Cells[5].Style.BackColor= Color.FromArgb(255, 37, 37);
                }
                else if (Convert.ToInt32(row.Cells[6].Value) == 2)
                {
                    row.Cells[5].Value = "Pedido realizado";
                    row.Cells[5].Style.BackColor = Color.FromArgb(253, 123, 36);
                }
                else if (Convert.ToInt32(row.Cells[6].Value) == 3)
                {
                    row.Cells[5].Value = "Concluído";
                    row.Cells[5].Style.BackColor = Color.FromArgb(37, 125, 81);
                }
            }
        }

        //Confirmar as Notas
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        //Clique na tabela
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();

            if (e.ColumnIndex==0 & e.RowIndex >= 0)                
            {
                if (NotasDuplicadas.Contains(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[7].Value)) == false) {

                    cell = dataGridView1.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;


                    if (Convert.ToBoolean(cell.Value) == true)
                    {
                        cell.Value = false;
                        notasSelecionadas.Remove(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[7].Value));
                    } else
                    {
                        cell.Value = true;
                        notasSelecionadas.Add(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[7].Value));
                    }
                }
            }
            
        }

        //Fechar
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Close_button_Click(object sender, EventArgs e){
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

        //Aumentar e diminuir botão
        private void Apmliar_button_MouseEnter(object sender, EventArgs e)
        {

        }
        private void Ampliar_button_MouseLeave(object sender, EventArgs e)
        {

        }

        //Procurar nota
        private void button7_Click(object sender, EventArgs e)
        {
            List<string> filtro = new List<string>();

            dataGridView1.Rows.Clear();


            if (textBox1.Text != String.Empty)
            {
                var lista = Rows.Where(x => x.Cells[1].Value.ToString() == textBox1.Text).ToList();

                CarregarNotas(lista);
                PreencherDataGrid();

            }
            else
            {
                CarregarNotas(Rows);
                PreencherDataGrid();
            }
        }
            //Impedir Selção na tabela
            private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

    }
}
