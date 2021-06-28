using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;

namespace Programa_BRASCAM
{
    public partial class FormPrincipal : Form
    {
        bool iniciando;
        int posiçãoX, posiçãoY;
        bool arrastar = false; 
        bool loading; 
        public List<int> NotasSelecionadas = new List<int>();
        string dataFilter;
        DateTime data;

        FormCadastroResponsavel cadastroResponsavel = new FormCadastroResponsavel();
               
        public FormPrincipal()
        {            
            iniciando = true;
            
            Opacity = 0;

            InitializeComponent();

            FormLogin login = new FormLogin();
            login.ShowDialog();

            if (Program.result == DialogResult.Cancel) 
            {
                this.Close();
                Environment.Exit(0);
                
            }
            else if (Program.result == DialogResult.Retry) 
            {
                Application.Restart();
                this.Close(); 
            }

            Program.Colunas1 = new List<string>();
            
            foreach(DataGridViewColumn col in dataGridView1.Columns)
            {
                Program.Colunas1.Add(col.Name);
            }

            Program.Colunas2 = new List<string>();

            foreach (DataGridViewColumn col in dataGridView2.Columns)
            {
                Program.Colunas2.Add(col.Name);
            }

            ControlBox = true;

            loading = true;

            tabControl1.Location = new Point(-5, 46);            

            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();             

            CarregarClientes();


            int dist = 10;
            int x = label2.Location.X + label2.Size.Width + dist;            
            comboBox1.Location = new Point(x, 20);
            x = comboBox1.Location.X + comboBox1.Size.Width + dist;
            comboBox2.Location = new Point(x, 20);
            x = comboBox2.Location.X + comboBox2.Size.Width + dist;
            dateTimePicker1.Location = new Point(x, 31);
            label5.Location = new Point(x, 15);
            x= dateTimePicker1.Location.X + dateTimePicker1.Size.Width + dist;
            dateTimePicker2.Location = new Point(x, 31);
            label6.Location = new Point(x, 15);
            x = dateTimePicker2.Location.X + dateTimePicker2.Size.Width + dist;
            textBox1.Location = new Point(x, 26);
            x = textBox1.Location.X + textBox1.Size.Width + dist;
            button7.Location = new Point(x, 26);

            x= label2.Location.X + label2.Size.Width + dist;
            comboBox4.Location = new Point(x, 20);
            x = comboBox4.Location.X + comboBox4.Size.Width + dist;
            comboBox3.Location = new Point(x, 20);
            x = comboBox3.Location.X + comboBox3.Size.Width + dist;
            textBox2.Location = new Point(x, 26);
            x = textBox2.Location.X + textBox2.Size.Width + dist;
            button3.Location = new Point(x, 26);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            data = DateTime.Today.AddDays(-60);
            dataFilter = data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");
            
            UpdateNotas();
            UpdatePedidos();
            UpdatePedidosDeEntrega();
            CarregarClientes();

            tabControl1.SelectedTab = tabControl1.TabPages[0];

            Visible = true;
            ShowInTaskbar = true;
            Opacity = 100f;

            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox4.Text = comboBox4.Items[0].ToString();            
            comboBox3.Text = comboBox3.Items[0].ToString();            
            comboBox2.Text = comboBox2.Items[0].ToString();           

            Opacity = 100;

            if (Programa_BRASCAM.Properties.Settings.Default.Permissão == 3)
            {
                button2.Enabled = false;
                button4.Enabled = false;
                dataGridView2.Columns[7].ReadOnly = true;
                dataGridView1.Columns[7].ReadOnly = true;
                dataGridView1.Columns[6].ReadOnly = true;
            }

            inicioDoTimer = DateTime.Now;
            timer1.Start();
            timer2.Start();           

            AutoUpdateNotas();
            AutoUpdatePedidos();
            AutoUpdatePedidosDeEntrega();

            dateTimePicker1.Value = data;
            dateTimePicker2.Value = DateTime.Today;

            Program.EndLoading();
        }

        //Carregar Banco de Dados       
        void StatusDaNota()
        {
            this.tabControl1.Invoke((MethodInvoker)delegate
            {
                tabControl1.SelectedIndex = 0;
            });

            this.dataGridView1.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {

                    if (Convert.ToInt32(row.Cells[8].Value) == 1)
                    {
                        Program.Notas[row.Index]["StatusColumn"] = "Pendente de envio";
                        row.Cells[6].Style.BackColor = Color.Red;
                    }
                    else if (Convert.ToInt32(row.Cells[8].Value) == 2)
                    {
                        Program.Notas[row.Index]["StatusColumn"] = "Saiu p/ entrega";
                        row.Cells[6].Style.BackColor = Color.Orange;
                    }
                    else if (Convert.ToInt32(row.Cells[8].Value) == 3)
                    {
                        Program.Notas[row.Index]["StatusColumn"] = "Canhoto pendente";
                        row.Cells[6].Style.BackColor = Color.Yellow;
                    }
                    else if (Convert.ToInt32(row.Cells[8].Value) == 4)
                    {
                        Program.Notas[row.Index]["StatusColumn"] = "Entrega finalizada";
                        row.Cells[6].Style.BackColor = Color.Green;
                    }
                }

            });

            
        }
        void StatusDoPedido()
        {
            this.tabControl1.Invoke((MethodInvoker)delegate
            {
                tabControl1.SelectedIndex = 1;
            });

            this.dataGridView2.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    //status-------------               
                    if (Convert.ToInt32(row.Cells[9].Value) == 1)
                    {
                        Program.Pedidos[row.Index]["StatusColumn"] = "Pedido realizado";
                        row.Cells[7].Style.BackColor = Color.Yellow;
                    }
                    else if (Convert.ToInt32(row.Cells[9].Value) == 2)
                    {
                        Program.Pedidos[row.Index]["StatusColumn"] = "Pedido entregue";
                        row.Cells[7].Style.BackColor = Color.Green;
                    }

                    //Tipo---------------------
                    if (Convert.ToInt32(row.Cells[10].Value) == 1)
                    {
                        Program.Pedidos[row.Index]["TipoColumn"] = "Coleta";
                    }
                    else if (Convert.ToInt32(row.Cells[10].Value) == 2)
                    {
                        Program.Pedidos[row.Index]["TipoColumn"] = "Entrega";
                    }

                }
            });

        }        
        void UpdateNotas()
        {
            loading = true;

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            SqlCommand command = new SqlCommand("SELECT CodigoDaNota, NumDaNota, NumDoPedido, " +
                "CodigoPedidoDeEntrega, DataDaNota, DataDeEntrega, CodDoCliente, StatusDoPedido," +
                " Tipo, ReceberDinheiro, ValorDaNota, Cliente, Responsavel,ModoDeEntrega,TipoDeEntrega,Frete FROM BRASCAMnotas WHERE " +
                "DataDaNota>='" + dataFilter + "' ORDER BY StatusDoPedido, DataDaNota, NumDaNota DESC", connection );

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);
            dataTable.Columns.Add("CheckColumn", typeof(bool));
            dataTable.Columns.Add("StatusColumn", typeof(string));

            Program.Notas = new DataView(dataTable); 
            
            dataGridView1.DataSource = Program.Notas;
            dataGridView1.Columns[0].DataPropertyName = "CheckColumn";
            dataGridView1.Columns[6].DataPropertyName = "StatusColumn";

            StatusDaNota();

            loading = false;
        }
        void UpdatePedidos()
        {
            loading = true;           
            tabControl1.SelectedTab = tabControl1.TabPages[1];

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            SqlCommand command = new SqlCommand("SELECT Protocolo, UsuarioEmissor, " +
                "DataDoPedido, ResponsavelPelaEntrega, Tipo, DataDeEntrega, StatusDoPedido, Veiculo, " +
                "Quilometragem, Frete, Transportadora, Observacao FROM dbo.BRASCAMpedidos WHERE " +
                "DataDoPedido>='" + dataFilter + "' ORDER BY StatusDoPedido, DataDoPedido, Protocolo DESC", connection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);
            
            dataTable.Columns.Add("TipoColumn", typeof(string));
            dataTable.Columns.Add("StatusColumn", typeof(string));

            Program.Pedidos = new DataView(dataTable);

            dataGridView2.DataSource = Program.Pedidos;
                        
            dataGridView2.Columns[4].DataPropertyName = "TipoColumn";
            dataGridView2.Columns[7].DataPropertyName = "StatusColumn";

            StatusDoPedido();

            loading = false;
        }
        void UpdatePedidosDeEntrega()
        {
            loading = true;

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);

            SqlCommand command = new SqlCommand("SELECT CodigoDaNota, NumDaNota, NumDoPedido, " +
                "CodigoPedidoDeEntrega, DataDaNota, DataDeEntrega, CodDoCliente, StatusDoPedido," +
                " Tipo, ReceberDinheiro, ValorDaNota, Cliente, Responsavel,ModoDeEntrega,TipoDeEntrega,Frete FROM BRASCAMpedDeEntrega WHERE " +
                "DataDaNota>='" + dataFilter + "' ORDER BY StatusDoPedido, DataDaNota, NumDoPedido DESC", connection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);
            dataTable.Columns.Add("CheckColumn", typeof(bool));
            dataTable.Columns.Add("StatusColumn", typeof(string));

            Program.PedidosDeEntrega = new DataView(dataTable);

            //dataGridView1.DataSource = Program.Notas;
            //dataGridView1.Columns[0].DataPropertyName = "CheckColumn";
            //dataGridView1.Columns[6].DataPropertyName = "StatusColumn";

            //StatusDaNota();

            loading = false;
        }
        void CarregarClientes()
        {
            Program.CodClientes = new List<string>();
            Program.Clientes = new List<string>();

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
            SqlCommand command = new SqlCommand("SELECT Codigo,RazaoSocial FROM Entidades WHERE RazaoSocial<>''", connection);

            DataTable dataTable;
            try
            {
                connection.Open();
                dataTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                sqlDataAdapter.Fill(dataTable);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {                
                MessageBox.Show("Erro de conexão");
                if (iniciando == true) { Close(); }

                return;
            }

            foreach (DataRow dataRow in dataTable.Rows)
            {
                Program.CodClientes.Add(dataRow[0].ToString());
                Program.Clientes.Add(dataRow[1].ToString());
            }

        }
        
        //ComboBox
        private void comboBoxTextChanged(object sender,EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.SelectionLength = 0;
        }       

        //Paint
        private void BordaCustomizada(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;

            Pen caneta = new Pen(Color.FromArgb(208, 208, 209));
            caneta.Width = 3;

            foreach (Control controle in panel.Controls)
            {
                if (controle.Name.Contains("textBox"))
                {
                    
                    TextBox txt = (TextBox)controle;
                    
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);
                }
                else if (controle.Name.Contains("comboBox"))
                {
                    
                    ComboBox txt = (ComboBox)controle;                        
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);                                  

                }
                else if(controle.Name.Contains("dateTimePicker"))
                {   
                    DateTimePicker picker = (DateTimePicker)controle;
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);

                }

            }
        }
        private void FormPrincipal_Paint(object sender, PaintEventArgs e)
        {
           
        }
        private void BordaCustomizada2(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;

            Pen caneta = new Pen(Color.FromArgb(208, 208, 209));
            caneta.Width = 3;

            foreach (Control controle in panel.Controls)
            {
                
                if (controle.Name.Contains("comboBox"))
                {

                    ComboBox txt = (ComboBox)controle;
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);

                }
                
            }
        }

        //Redimensionando o Form
        private void FormPrincipal_SizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine(Size.Width);

            if (Width > 1245 & (checkBox1.Parent == panel1 | checkBox1.Parent == panel8))
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    panel1.Controls.Remove(checkBox1);
                    panel1.Controls.Remove(pictureBox2);
                    panel1.Controls.Remove(maskedTextBox1);
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    panel8.Controls.Remove(checkBox1);
                    panel8.Controls.Remove(pictureBox2);
                    panel8.Controls.Remove(maskedTextBox1);
                }

                checkBox1.Parent = panel3;
                pictureBox2.Parent = panel3;
                maskedTextBox1.Parent = panel3;

                checkBox1.ForeColor = Color.Black;
                pictureBox2.BackgroundImage = Programa_BRASCAM.Properties.Resources.cadiado;

                if (tabControl1.SelectedIndex == 0)
                {
                    checkBox1.Location = new Point(panel1.Width - 201, 30);
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    checkBox1.Location = new Point(panel8.Width - 201, 30);
                }
                pictureBox2.Location = new Point(checkBox1.Location.X + 114, checkBox1.Location.Y - 7);
                maskedTextBox1.Location = new Point(checkBox1.Location.X-55, checkBox1.Location.Y -3);
            }

            else if (Width <= 1245 & checkBox1.Parent == panel3)
            {
                panel3.Controls.Remove(checkBox1);
                panel3.Controls.Remove(pictureBox2);
                panel3.Controls.Remove(maskedTextBox1);

                if (tabControl1.SelectedIndex == 0)
                {
                    checkBox1.Parent = panel1;
                    pictureBox2.Parent = panel1;
                    maskedTextBox1.Parent = panel1;
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    checkBox1.Parent = panel8;
                    pictureBox2.Parent = panel8;
                    maskedTextBox1.Parent = panel8;
                }

                checkBox1.ForeColor = Color.White;
                pictureBox2.BackgroundImage = Programa_BRASCAM.Properties.Resources.cadiado_branco;
                                
                checkBox1.Location = new Point(panel1.Width - 200, 30);
                pictureBox2.Location = new Point(checkBox1.Location.X + 114, checkBox1.Location.Y - 7);
                maskedTextBox1.Location = new Point(checkBox1.Location.X - 55, checkBox1.Location.Y - 3);
            }


            if (Width <= 1007 & Width >= 980)
            {
                pictureBox1.Location = new Point(Width - 1300 + 344, pictureBox1.Location.Y);
                NFPendentesButton.Location = new Point(Width - 1084 + 344, NFPendentesButton.Location.Y);
                SolicitacoesButton.Location = new Point(Width - 919 + 344, SolicitacoesButton.Location.Y);
                RelatorioButton.Location = new Point(Width - 687 + 344, RelatorioButton.Location.Y);
                CadastrosButton.Location = new Point(Width - 540 + 344, CadastrosButton.Location.Y);

            }
            else if (Width > 1007)

            {
                pictureBox1.Location = new Point(58, pictureBox1.Location.Y);
                NFPendentesButton.Location = new Point(270, NFPendentesButton.Location.Y);
                SolicitacoesButton.Location = new Point(435, SolicitacoesButton.Location.Y);
                RelatorioButton.Location = new Point(667, RelatorioButton.Location.Y);
                CadastrosButton.Location = new Point(814, CadastrosButton.Location.Y);

            }
        }
        private void FormPrincipal_ResizeBegin(object sender, EventArgs e)
        {
            checkBox1.Visible = false;
            pictureBox2.Visible = false;
            maskedTextBox1.Visible = false;
        }
        private void FormPrincipal_ResizeEnd(object sender, EventArgs e)
        {
            checkBox1.Visible = true;
            pictureBox2.Visible = true;
            maskedTextBox1.Visible = true;
        }

        //Barra Superior
        private void CloseWindowButton_Click(object sender, EventArgs e)
        {            
            Application.Exit();
        }   
        private void MaximizeWindowButton_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
            else 
            {
                WindowState = FormWindowState.Maximized;
            }
        }
        private void MinimizeWindowButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }       

        //Menu
        private void NFPendentesButton_Paint(object sender, PaintEventArgs e)
        {
            Pen caneta = new Pen(Color.FromArgb(179, 179, 179));
            caneta.Width = 4;
            e.Graphics.DrawLine(caneta,NFPendentesButton.Width-1,0, NFPendentesButton.Width-1, NFPendentesButton.Height);
            e.Graphics.DrawLine(caneta, 1, 1, 1, NFPendentesButton.Height);
        }
        private void SolicitacoesButton_Paint(object sender, PaintEventArgs e)
        {
            Pen caneta = new Pen(Color.FromArgb(179, 179, 179));
            caneta.Width = 4;
            e.Graphics.DrawLine(caneta, SolicitacoesButton.Width - 1, 0, SolicitacoesButton.Width - 1, SolicitacoesButton.Height);
        }
        private void RelatorioButton_Paint(object sender, PaintEventArgs e)
        {
            Pen caneta = new Pen(Color.FromArgb(179, 179, 179));
            caneta.Width = 4;
            e.Graphics.DrawLine(caneta, RelatorioButton.Width - 1, 0, RelatorioButton.Width - 1, RelatorioButton.Height);

        }
        private void CadastrosButton_Paint(object sender, PaintEventArgs e)
        {
            Pen caneta = new Pen(Color.FromArgb(179, 179, 179));
            caneta.Width = 4;
            e.Graphics.DrawLine(caneta, CadastrosButton.Width - 1, 0, CadastrosButton.Width - 1, CadastrosButton.Height);

        }
        private void NFPendentesButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }
        private void SolicitacoesButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[1];
        }
        private void RelatorioButton_Click(object sender, EventArgs e)
        {
            RelatorioButton.ContextMenuStrip.Show(RelatorioButton, 0, RelatorioButton.Height);
        }
        private void CadastrosButton_Click(object sender, EventArgs e)
        {
            CadastrosButton.ContextMenuStrip.Show(CadastrosButton, 9, CadastrosButton.Height);
        }       
        private void mouseEntraCor(object sender, EventArgs e)
        {
            Button botão = (Button)sender;
            botão.BackColor = Color.FromArgb(180, 180, 180);

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Width < 1186)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    checkBox1.Parent = panel1;
                    pictureBox2.Parent = panel1;
                    maskedTextBox1.Parent = panel1;
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    checkBox1.Parent = panel8;
                    pictureBox2.Parent = panel8;
                    maskedTextBox1.Parent = panel8;
                }
            }
        }

        //Submenu Relatórios
        private void contextMenuStripRelatorios_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStripRelatorios.Show(RelatorioButton, 0, RelatorioButton.Height);
        }
        private void contextMenuStripRelatorios_MouseLeave(object sender, EventArgs e)
        {
            contextMenuStripRelatorios.Close();
        }
        private void notaFiscalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRelatorioNotas relatorioNotas = new FormRelatorioNotas();
            relatorioNotas.Notas = new DataView();            
            relatorioNotas.Notas= Program.Notas;
            relatorioNotas.ShowDialog();
        }
        private void pedidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRelatorioPedidos relatorioPedidos = new FormRelatorioPedidos();
            relatorioPedidos.Pedidos = new DataView();
            relatorioPedidos.Pedidos = Program.Pedidos;
            relatorioPedidos.ShowDialog();
        }
        
        //Submenu Cadastros
        private void contextMenuStripCadastros_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStripCadastros.Show(CadastrosButton, 9, CadastrosButton.Height);
        }
        private void contextMenuStripCadastros_MouseLeave(object sender, EventArgs e)
        {
            contextMenuStripCadastros.Close();
        }
        private void responsávelToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            cadastroResponsavel.ShowDialog();
        }
        private void usuáriosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCadastroUsuario cadastroUsuario = new FormCadastroUsuario();            
            cadastroUsuario.ShowDialog();
        }
        private void veículosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCadastroVeiculos cadastroVeiculos = new FormCadastroVeiculos();           
            cadastroVeiculos.ShowDialog();
        }

        //Tabela Primeira Tela   
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
            
            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();            

            if (e.RowIndex >= 0 & Program.Colunas1[e.ColumnIndex]=="ExcluirNota"  & Properties.Settings.Default.Permissão!=3)
            {
                if (MessageBox.Show("Deseja excluir a nota n° " + dataGridView1.Rows[e.RowIndex].Cells["NumDaNota"].Value + " de " + Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Data"].Value).ToShortDateString() + "?", "Aviso de exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                    
                    SqlConnection connection2 = new SqlConnection(Properties.Settings.Default.RSYS2003ConnectionString);
                    string command2 = "DELETE FROM BRASCAMnotas WHERE CodigoDaNota="+ dataGridView1.Rows[e.RowIndex].Cells["CodigoDaNota"].Value;

                    SqlCommand sqlCommand = new SqlCommand(command2, connection2);
                    connection2.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection2.Close();
                }
            }
            else if (e.RowIndex >= 0 & Program.Colunas1[e.ColumnIndex] == "SelecionarNota" & Properties.Settings.Default.Permissão != 3)
            {
                checkCell = dataGridView1.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;

                
                if (Program.Notas[e.RowIndex]["CheckColumn"] != DBNull.Value && Program.Notas[e.RowIndex]["CheckColumn"] !=null)
                {

                    if (Convert.ToBoolean(Program.Notas[e.RowIndex]["CheckColumn"]) == true)
                    {
                        Program.Notas[e.RowIndex]["CheckColumn"] = false;
                        NotasSelecionadas.Remove(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[9].Value));

                    }
                    else if (Convert.ToBoolean(Program.Notas[e.RowIndex]["CheckColumn"]) == false)
                    {
                        Program.Notas[e.RowIndex]["CheckColumn"] = true;
                        NotasSelecionadas.Add(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[9].Value));
                    }
                }
                else
                {
                    Program.Notas[e.RowIndex]["CheckColumn"] = true;
                    NotasSelecionadas.Add(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[9].Value));
                }
            }
            else if (e.RowIndex >= 0 & Program.Colunas1[e.ColumnIndex] == "VerPedido" & Properties.Settings.Default.Permissão != 3)
            {
                try
                {
                    int protocolo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Protocolo"].Value);

                    SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                    SqlCommand command = new SqlCommand("SELECT Protocolo FROM BRASCAMPedidos " +
                     "WHERE Protocolo=" + protocolo, connection);

                    int PROTOCOLO = -1;

                    //Verificando se o pedido existe
                    try
                    {

                        connection.Open();
                        SqlDataReader dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {                            
                            try 
                            {                                
                                PROTOCOLO = dataReader.GetInt32(0); 
                            } catch { }
                        }
                    }

                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erro de conexão");
                        Close();
                        return;
                    }

                    if (PROTOCOLO!=-1)
                    {
                        FormNovoPedido novoPedidoForm = new FormNovoPedido();
                        novoPedidoForm.Pedido = new Pedido();
                        novoPedidoForm.Visualizando = true;
                        novoPedidoForm.Pedido.Protocolo = PROTOCOLO;
                        novoPedidoForm.FormClosed += NovoPedidoForm_FormClosed1;
                        novoPedidoForm.ShowDialog();
                    }
                   
                }
                catch { }
            }

        }       
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int n = e.RowIndex;
                int item = 0;

                if (n >= 0)
                {
                    DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                    comboBoxCell = dataGridView1.Rows[n].Cells[6] as DataGridViewComboBoxCell;

                    item = comboBoxCell.Items.IndexOf(comboBoxCell.Value) + 1;

                    if (e.ColumnIndex == 6 & loading == false)
                    {
                        SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
                        string commandString = "";

                        if (item != 1)
                        {
                            commandString = "UPDATE BRASCAMnotas SET StatusDoPedido=" + item + " WHERE CodigoDaNota=" + dataGridView1.Rows[n].Cells[9].Value;
                        }
                        else
                        {
                            commandString = "UPDATE BRASCAMnotas SET StatusDoPedido=" + item + ",CodigoPedidoDeEntrega=null,DataDeEntrega=null,Responsavel=null WHERE CodigoDaNota=" + dataGridView1.Rows[n].Cells[9].Value;
                        }

                        SqlCommand command = new SqlCommand(commandString, connection);

                        Program.Notas[n]["StatusDopedido"] = item;

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("Erro de conexão");
                            return;
                        }
                    }

                    if (item == 1)
                    {
                        dataGridView1.Rows[n].Cells[6].Style.BackColor = Color.Red;
                    }
                    else if (item == 2)
                    {
                        dataGridView1.Rows[n].Cells[6].Style.BackColor = Color.Orange;
                    }
                    else if (item == 3)
                    {
                        dataGridView1.Rows[n].Cells[6].Style.BackColor = Color.Yellow;
                    }
                    else if (item == 4)
                    {
                        dataGridView1.Rows[n].Cells[6].Style.BackColor = Color.Green;
                    }
                }
            }
            
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
        
        //tabela Segunda tela       
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 & e.ColumnIndex == 8)
            {
                FormNovoPedido novoPedidoForm = new FormNovoPedido();

                novoPedidoForm.Pedido = new Pedido();
                novoPedidoForm.Visualizando = true;
                novoPedidoForm.Pedido.Protocolo = dataGridView2.Rows[e.RowIndex].Cells[0].Value;
                novoPedidoForm.FormClosed += NovoPedidoForm_FormClosed1;
                novoPedidoForm.ShowDialog();

                if (novoPedidoForm.DialogResult == DialogResult.Abort)
                {
                   
                }
            }

           
        }
        private void NovoPedidoForm_FormClosed1(object sender, FormClosedEventArgs e)
        {
            FechandoFormNovoPedido();
        }
        
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            

            if (dataGridView2.Rows.Count > 0) { 

            int n = e.RowIndex;
            int item = 0;

                if (n >= 0)
                {
                    DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                    comboBoxCell = dataGridView2.Rows[n].Cells[7] as DataGridViewComboBoxCell;

                    item = comboBoxCell.Items.IndexOf(comboBoxCell.Value) + 1;

                    if (e.ColumnIndex == 7 & loading == false)
                    {

                        SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
                        string commandString = "";

                        commandString = "UPDATE BRASCAMpedidos SET StatusDoPedido=" + item + " WHERE Protocolo=" + dataGridView2.Rows[n].Cells[0].Value;
                        SqlCommand command = new SqlCommand(commandString, connection);

                        Program.Pedidos[n]["StatusDoPedido"] = item;

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("Erro de conexão. O pedido de entrega não foi atualizad.");
                            return;
                        }
                    }

                    if (item == 1)
                    {
                        comboBoxCell.Style.BackColor = Color.Yellow;
                    }
                    else if (item == 2)
                    {
                        comboBoxCell.Style.BackColor = Color.Green;
                    }
                    

                    SqlConnection connection2 = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.RSYS2003ConnectionString);
                    string commandString2 = "";
                    string commandString3 = "";

                    if (item == 2)
                        {

                        commandString2 = "UPDATE BRASCAMnotas SET StatusDoPedido=" + 4 + " WHERE CodigoPedidoDeEntrega=" + dataGridView2.Rows[n].Cells[0].Value;
                        commandString3 = "UPDATE BRASCAMpedDeEntrega SET StatusDoPedido=" + 4 + " WHERE CodigoPedidoDeEntrega=" + dataGridView2.Rows[n].Cells[0].Value;

                        SqlCommand command2 = new SqlCommand(commandString2, connection2);
                        SqlCommand command3 = new SqlCommand(commandString3, connection2);


                        //Program.Notas[n]["StatusDopedido"] = 4;

                        try
                        {
                            connection2.Open();
                            command2.ExecuteNonQuery();
                            command3.ExecuteNonQuery();
                            connection2.Close();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("Erro de conexão. As notas e os pedidos não foram atualizados.");
                            return;
                        }

                    }

                }
            }

        }
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
        }
        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

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

        //Novo pedido
        private void button2_Click(object sender, EventArgs e)
        {
            NovoPedido();
           
        }
        private void button4_Click(object sender, EventArgs e)
        {
            NovoPedido();            
            
        }
        void NovoPedido()
        {
            Pedido novoPedido = new Pedido();
            FormNovoPedido novoPedidoForm = new FormNovoPedido();            
            novoPedidoForm.NotasSelecionadas = NotasSelecionadas;
            novoPedidoForm.Visualizando = false;               
            novoPedidoForm.FormClosed += NovoPedidoForm_FormClosed;
            novoPedidoForm.ShowDialog();           
        }
        private void NovoPedidoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FechandoFormNovoPedido();
           
        }
        void FechandoFormNovoPedido()
        {  

            for (int n = 0; n < Program.Notas.Count; n++)
            {
                Program.Notas[n]["CheckColumn"] = false;
            }
            NotasSelecionadas.Clear();         

        }

        //Atualizar Banco e Tabela
        private void button1_Click(object sender, EventArgs e)
        {
            loading = true;

            CarregarClientes();
            UpdateNotas();
            UpdatePedidos();
            UpdatePedidosDeEntrega();

            tabControl1.SelectedTab = tabControl1.TabPages[0];

            loading = false;

            FiltroTabela1();
            FiltroTabela2();

        }

        //Filtros        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltroTabela1();
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltroTabela2();
        }
        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortTabela1();  
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortTabela2();
        }
        void SortTabela1()
        {
            if (loading == false)
            {
                int item = 0; ;
                try
                {
                    item = comboBox2.Items.IndexOf(comboBox2.Text);
                }
                catch { }

                if (item == 1) { Program.Notas.Sort = "StatusDoPedido ASC, DataDaNota ASC, NumDaNota DESC"; }
                else if (item == 0) { Program.Notas.Sort = "StatusDoPedido ASC, DataDaNota DESC, NumDaNota DESC"; }

                StatusDaNota();
            }
        }
        void SortTabela2() {
            if (loading == false)
            {    
                int item = 0; ;
                try
                {
                    item = comboBox3.Items.IndexOf(comboBox3.Text);
                }
                catch { }

                if (item == 1) { Program.Pedidos.Sort = "StatusDoPedido ASC, DataDoPedido ASC, Protocolo DESC"; }
                else if (item == 0) { Program.Pedidos.Sort = "StatusDoPedido ASC, DataDoPedido DESC, Protocolo DESC"; }

                StatusDoPedido();
            }
        }
        void FiltroTabela1()
        {
            if (loading == false)
            {

                int item = 0; ;
                try
                {
                    item = comboBox1.Items.IndexOf(comboBox1.Text);
                }
                catch { }

                string data1 = dateTimePicker1.Value.ToString("dd/MM/yyyy");
                string data2 = dateTimePicker2.Value.ToString("dd/MM/yyyy");

                if (item != 0)
                {
                    Program.Notas.RowFilter = "StatusDoPedido=" + item + " AND DataDaNota>='"+data1+"' AND DataDaNota<='"+data2+"'";
                }
                else
                {
                    Program.Notas.RowFilter = "StatusDoPedido IN (1,2,3,4)" + " AND DataDaNota>='" + data1 + "' AND DataDaNota<='" + data2 + "'";
                }
                SortTabela1();
                StatusDaNota();
            }
        }
        void FiltroTabela2() {
            if (loading == false)
            {
                int item = 0; ;
                try
                {
                    item = comboBox4.Items.IndexOf(comboBox4.Text);
                }
                catch { }

                if (item != 0)
                {
                    Program.Pedidos.RowFilter = "StatusDoPedido=" + item;

                }
                else
                {
                    Program.Pedidos.RowFilter = "StatusDoPedido IN (1,2)";
                }
                SortTabela2();
                StatusDoPedido();
            }
        }      
        private void FiltrarPorData(object sender, EventArgs e)
        {
            if(loading==false)
            {
                var datePicker = sender as DateTimePicker;
                var novaData = datePicker.Value;

                if(novaData<data)
                {
                    data = novaData;
                    dataFilter = novaData.Year.ToString("0000") + "-" + novaData.Month.ToString("00") + "-" + novaData.Day.ToString("00");

                    UpdateNotas();
                    CarregarClientes();
                    StatusDaNota();
                }

                FiltroTabela1();
            }
            
        }

        //Pesquisar
        private void button7_Click(object sender, EventArgs e)
        {

            List<string> filtro = new List<string>();

            Program.Notas.RowFilter = "";

            if (textBox1.Text != String.Empty)
            {               

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 1; j <= 5; j++)
                    {
                        try
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value.ToString().Contains(textBox1.Text) == true)
                            {
                                filtro.Add(dataGridView1.Rows[i].Cells[2].Value.ToString());
                            }
                        }
                        catch { }

                    }
                }

                if (filtro.Count > 0)
                {                    
                    Program.Notas.RowFilter = "NumDoPedido IN(" + String.Join(",", filtro) + ")";
                    StatusDaNota();
                }
                else if(filtro.Count==0 & textBox1.Text != String.Empty)
                {
                    Program.Notas.RowFilter = "NumDoPedido =-1";
                    StatusDaNota();
                }
                               

            }
            else { Program.Notas.RowFilter = ""; StatusDaNota(); }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            List<string> filtro = new List<string>();

            Program.Pedidos.RowFilter = "";

            if (textBox2.Text != String.Empty)
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    for (int j = 0; j <= 7; j++)
                    {
                        try
                        {
                            if (dataGridView2.Rows[i].Cells[j].Value.ToString().Contains(textBox2.Text) == true)
                            {
                                filtro.Add(dataGridView2.Rows[i].Cells[0].Value.ToString());
                            }
                        }
                        catch { }

                    }
                }

                if (filtro.Count > 0)
                {
                    Program.Pedidos.RowFilter = "Protocolo IN(" + String.Join(",", filtro) + ")";
                    StatusDoPedido();
                }
                if (filtro.Count == 0 & textBox2.Text!=String.Empty)
                {
                    Program.Pedidos.RowFilter = "Protocolo =-1";
                    StatusDoPedido();
                }

            }
            else { Program.Pedidos.RowFilter = ""; StatusDoPedido(); }
        }

        //Clicar e arrastar
        private void Arrastar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y < 22)
            {
                arrastar = true;
                posiçãoX = e.X;
                posiçãoY = e.Y;
            }
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

        //fechando form
        private void FormPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "Temporário.doc"));
            }
            catch { }
            try
            {
                File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "Temporário.pdf"));
            }
            catch { }          

            Application.Exit();
        }

        //manter conectado
        DateTime inicioDoTimer;
        TimeSpan deltaTime;

        Point cursor;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                inicioDoTimer = DateTime.Now;
                timer1.Start();
                cursor = Cursor.Position;
            }
            else { timer1.Stop(); maskedTextBox1.Clear(); }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            deltaTime = DateTime.Now - inicioDoTimer;                    

            int minutos = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Win32.GetIdleTime())/60000));
            int segundos = (Convert.ToInt32(Win32.GetIdleTime())-minutos*60000)/1000;
            maskedTextBox1.Text = minutos.ToString("00") + segundos.ToString("00");
            //120000
            if (Win32.GetIdleTime() >= 120000) { timer1.Stop(); Application.Restart(); }

        }
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        public class Win32
        {
            [DllImport("User32.dll")]
            public static extern bool LockWorkStation();

            [DllImport("User32.dll")]
            private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

            [DllImport("Kernel32.dll")]
            private static extern uint GetLastError();

            public static uint GetIdleTime()
            {
                LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                GetLastInputInfo(ref lastInPut);

                return ((uint)Environment.TickCount - lastInPut.dwTime);
            }

            public static long GetLastInputTime()
            {
                LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                if (!GetLastInputInfo(ref lastInPut))
                {
                    throw new Exception(GetLastError().ToString());
                }

                return lastInPut.dwTime;
            }
        }

        //Atualizar Notas
        class Nota 
        {
            public string CodigoDaNota { get; set; }
            public string NumDaNota { get; set; }
            public string NumDoPedido { get; set; }
            public string CodigoPedidoDeEntrega { get; set; }
            public string StatusDoPedido { get; set; }
            public string Tipo { get; set; }
            public string ReceberDinheiro { get; set; }
            public string TemNota { get; set; }
            public string ModoDeEntrega { get; set; }
            public string TipoDeEntrega { get; set; }
            public string DataDaNota { get; set; }
            public string DataDeEntrega { get; set; }
            public string CodDoCliente { get; set; }
            public string Responsavel { get; set; }
            public string Cliente { get; set; }
            public string ValorDaNota { get; set; }
            public string Frete { get; set; }

        }
        //Usar esse comando no banco de dados
        //ALTER DATABASE Database_Name SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;
               
        bool atualizandoNotas = false;
        DateTime horaInicial;
        TimeSpan tempoPassado;
        SqlTableDependency<Nota> dep;
        void AutoUpdateNotas()
        {
            dep = new SqlTableDependency<Nota>(Properties.Settings.Default.ConnectionString, "BRASCAMnotas");
            dep.OnChanged += Dep_OnChanged;            
            dep.Start();            
        }
        private void Dep_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Nota> e)
        {

            if (e.ChangeType != ChangeType.None)
            {
                object[] parametros = new object[16];
                parametros[0] = e.Entity.CodigoDaNota;
                parametros[1] = e.Entity.NumDaNota;
                parametros[2] = e.Entity.NumDoPedido;
                parametros[3] = e.Entity.CodigoPedidoDeEntrega;
                parametros[4] = e.Entity.DataDaNota;
                parametros[5] = e.Entity.DataDeEntrega;
                parametros[6] = e.Entity.CodDoCliente;
                parametros[7] = e.Entity.StatusDoPedido;
                parametros[8] = e.Entity.Tipo;
                parametros[9] = e.Entity.ReceberDinheiro;
                parametros[10] = e.Entity.ValorDaNota;
                parametros[11] = e.Entity.Cliente;
                parametros[12] = e.Entity.Responsavel;
                parametros[13] = e.Entity.ModoDeEntrega;
                parametros[14] = e.Entity.TipoDeEntrega;
                parametros[15] = e.Entity.Frete;
                
                loading = true;

                if (e.ChangeType == ChangeType.Insert)
                {
                    Invoke(new Action(() =>
                    {
                        dataGridView1.DataSource = null;
                    }));

                    Program.Notas.Table.Rows.Add(parametros);

                    try
                    {
                        if (parametros[6].ToString() != "" && Program.CodClientes.Contains(parametros[6].ToString()) == false)
                        {
                            Program.CodClientes.Add(parametros[6].ToString());
                            Program.Clientes.Add(parametros[11].ToString());

                            Invoke(new Action(() =>
                            {
                                dataGridView1.DataSource = Program.Notas;
                            }));

                            StatusDaNota();

                        }
                    }
                    catch { }



                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    if (Program.Notas.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList().Count > 0)
                    {
                        Invoke(new Action(() =>
                        {
                            dataGridView1.DataSource = null;
                        }));


                        var NotaAntiga = Program.Notas.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList()[0];
                        Program.Notas.Table.Rows.Remove(NotaAntiga);

                        Invoke(new Action(() =>
                        {
                            dataGridView1.DataSource = Program.Notas;
                        }));

                        StatusDaNota();
                    }
                   
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    if(Program.Notas.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList().Count>0)
                    {
                        dep.Stop();

                        Invoke(new Action(() =>
                        {
                            UpdateNotas();
                            CarregarClientes();
                            FiltroTabela1();
                            StatusDaNota();
                        }));
                        

                        dep.Start();                        
                    }

                }

               

                loading = false;

                #region Atualizar pedido de entrega

                try
                {
                    if (Convert.ToInt32(parametros[7]) != 4)
                    {
                        string commandString = "UPDATE BRASCAMpedidos SET StatusDoPedido=1 WHERE Protocolo=" + Convert.ToInt32(parametros[3]);
                        SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                        SqlCommand command = new SqlCommand(commandString, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();                       

                    }
                    else if(Convert.ToInt32(parametros[7]) == 4)
                    {
                        bool finalizado = true;

                        string commandString = "SELECT StatusDoPedido " +
                            "FROM BRASCAMnotas WHERE CodigoPedidoDeEntrega=" + Convert.ToInt32(parametros[3]);
                        SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                        SqlCommand command = new SqlCommand(commandString, connection);                        

                        connection.Open();
                        SqlDataReader dataReader = command.ExecuteReader();                        

                        while(dataReader.Read())
                        {
                            if(dataReader.GetInt32(0)!=4)
                            {
                                finalizado = false;
                            }
                        }

                        connection.Close();

                        if (finalizado == true)
                        {
                            commandString = "SELECT StatusDoPedido " +
                                                        "FROM BRASCAMpedDeEntrega WHERE CodigoPedidoDeEntrega=" + Convert.ToInt32(parametros[3]);
                            SqlConnection connection2 = new SqlConnection(Properties.Settings.Default.ConnectionString);
                            SqlCommand command2 = new SqlCommand(commandString, connection2);

                            connection2.Open();
                            SqlDataReader dataReader2 = command2.ExecuteReader();

                            while (dataReader2.Read())
                            {
                                if (dataReader2.GetInt32(0) != 4)
                                {
                                    finalizado = false;
                                }
                            }

                            connection2.Close();

                        }

                        if (finalizado==false)
                        {
                            string commandString3 = "UPDATE BRASCAMpedidos SET StatusDoPedido=1 WHERE Protocolo=" + Convert.ToInt32(parametros[3]);
                            SqlConnection connection3 = new SqlConnection(Properties.Settings.Default.ConnectionString);
                            SqlCommand command3 = new SqlCommand(commandString3, connection3);
                            connection3.Open();
                            command3.ExecuteNonQuery();
                            connection3.Close();

                        }
                        else
                        {
                            string commandString3 = "UPDATE BRASCAMpedidos SET StatusDoPedido=2 WHERE Protocolo=" + Convert.ToInt32(parametros[3]);
                            SqlConnection connection3 = new SqlConnection(Properties.Settings.Default.ConnectionString);
                            SqlCommand command3 = new SqlCommand(commandString3, connection3);
                            connection3.Open();
                            command3.ExecuteNonQuery();
                            connection3.Close();
                        }
                    }
                }
                catch { }
                #endregion


            }
           
        }
        private void TimerDeUpdate_Tick(object sender, EventArgs e)
        {            
            tempoPassado = DateTime.Now - horaInicial;
            
            if (tempoPassado.TotalSeconds>=2)//Não está mais atualizando as notas
            {  
                Invoke(new Action(() =>
                {
                    dataGridView1.DataSource = Program.Notas;
                }));

                StatusDaNota();
                atualizandoNotas = false;
                timer3.Stop();
            }
        }

        //Atualizar pedidos de entrega
        void AutoUpdatePedidosDeEntrega()
        {
            var dep = new SqlTableDependency<Nota>(Properties.Settings.Default.ConnectionString, "BRASCAMpedDeEntrega");
            dep.OnChanged += Dep_OnChanged3;
            dep.Start();
        }
        private void Dep_OnChanged3(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Nota> e)
        {

            if (e.ChangeType != ChangeType.None)
            {
                object[] parametros = new object[16];
                parametros[0] = e.Entity.CodigoDaNota;
                parametros[1] = e.Entity.NumDaNota;
                parametros[2] = e.Entity.NumDoPedido;
                parametros[3] = e.Entity.CodigoPedidoDeEntrega;
                parametros[4] = e.Entity.DataDaNota;
                parametros[5] = e.Entity.DataDeEntrega;
                parametros[6] = e.Entity.CodDoCliente;
                parametros[7] = e.Entity.StatusDoPedido;
                parametros[8] = e.Entity.Tipo;
                parametros[9] = e.Entity.ReceberDinheiro;
                parametros[10] = e.Entity.ValorDaNota;
                parametros[11] = e.Entity.Cliente;
                parametros[12] = e.Entity.Responsavel;
                parametros[13] = e.Entity.ModoDeEntrega;
                parametros[14] = e.Entity.TipoDeEntrega;
                parametros[15] = e.Entity.Frete;

                loading = true;

                Invoke(new Action(() =>
                {
                    //dataGridView1.DataSource = null;
                }));

                if (e.ChangeType == ChangeType.Insert)
                {
                    Program.PedidosDeEntrega.Table.Rows.Add(parametros);

                    try
                    {
                        if (parametros[6].ToString() != "" && Program.CodClientes.Contains(parametros[6].ToString()) == false)
                        {
                            Program.CodClientes.Add(parametros[6].ToString());
                            Program.Clientes.Add(parametros[11].ToString());
                        }
                    }
                    catch { }


                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    if (Program.PedidosDeEntrega.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList().Count > 0)
                    {
                        var NotaAntiga = Program.PedidosDeEntrega.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList()[0];
                        Program.PedidosDeEntrega.Table.Rows.Remove(NotaAntiga);
                    }

                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    if(Program.PedidosDeEntrega.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList().Count>0)
                    {
                        var NotaAntiga = Program.PedidosDeEntrega.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.CodigoDaNota.ToString()).ToList()[0];
                        Program.PedidosDeEntrega.Table.Rows.Remove(NotaAntiga);
                        Program.PedidosDeEntrega.Table.Rows.Add(parametros);

                        try
                        {
                            if (parametros[6].ToString() != "" && Program.CodClientes.Contains(parametros[6].ToString()) == false)
                            {
                                Program.CodClientes.Add(parametros[6].ToString());
                                Program.Clientes.Add(parametros[11].ToString());
                            }
                        }
                        catch { }

                    }



                }

                Invoke(new Action(() =>
                {
                    //dataGridView1.DataSource = Program.Notas;
                }));               

                if (Convert.ToInt32(parametros[7]) != 4)
                {
                    string commandString = "UPDATE BRASCAMpedidos SET StatusDoPedido=1 WHERE Protocolo=" + Convert.ToInt32(parametros[3]);
                    SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                    SqlCommand command = new SqlCommand(commandString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                else if (Convert.ToInt32(parametros[7]) == 4)
                {
                    bool finalizado = true;

                    string commandString = "SELECT StatusDoPedido " +
                        "FROM BRASCAMnotas WHERE CodigoPedidoDeEntrega=" + Convert.ToInt32(parametros[3]);
                    SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                    SqlCommand command = new SqlCommand(commandString, connection);

                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        if (dataReader.GetInt32(0) != 4)
                        {
                            finalizado = false;
                        }
                    }

                    connection.Close();

                    if (finalizado == true)
                    {
                        commandString = "SELECT StatusDoPedido " +
                                                    "FROM BRASCAMpedDeEntrega WHERE CodigoPedidoDeEntrega=" + Convert.ToInt32(parametros[3]);
                        SqlConnection connection2 = new SqlConnection(Properties.Settings.Default.ConnectionString);
                        SqlCommand command2 = new SqlCommand(commandString, connection2);

                        connection2.Open();
                        SqlDataReader dataReader2 = command2.ExecuteReader();

                        while (dataReader2.Read())
                        {
                            if (dataReader2.GetInt32(0) != 4)
                            {
                                finalizado = false;
                            }
                        }

                        connection2.Close();

                    }

                    if (finalizado == false)
                    {
                        string commandString3 = "UPDATE BRASCAMpedidos SET StatusDoPedido=1 WHERE Protocolo=" + Convert.ToInt32(parametros[3]);
                        SqlConnection connection3 = new SqlConnection(Properties.Settings.Default.ConnectionString);
                        SqlCommand command3 = new SqlCommand(commandString3, connection3);
                        connection3.Open();
                        command3.ExecuteNonQuery();
                        connection3.Close();

                    }
                    else
                    {
                        string commandString3 = "UPDATE BRASCAMpedidos SET StatusDoPedido=2 WHERE Protocolo=" + Convert.ToInt32(parametros[3]);
                        SqlConnection connection3 = new SqlConnection(Properties.Settings.Default.ConnectionString);
                        SqlCommand command3 = new SqlCommand(commandString3, connection3);
                        connection3.Open();
                        command3.ExecuteNonQuery();
                        connection3.Close();
                    }
                }
          

            loading = false;

                //StatusDaNota();
            }

        }
        

        //Atualizar Pedidos
        class Pedido2
        {
            public string Protocolo { get; set; }
            public string UsuarioEmissor { get; set; }
            public string DataDoPedido { get; set; }
            public string ResponsavelPelaEntrega { get; set; }
            public string Tipo { get; set; }
            public string DataDeEntrega { get; set; }
            public string StatusDoPedido { get; set; }
            public string Veiculo { get; set; }
            public string Quilometragem { get; set; }
            public string Frete { get; set; }
            public string Transportadora { get; set; }
            public string Observacao { get; set; }           

        }
        //Usar esse comando no banco de dados
        //ALTER DATABASE Database_Name SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;
        void AutoUpdatePedidos()
        {
            var dep = new SqlTableDependency<Pedido2>(Properties.Settings.Default.ConnectionString, "BRASCAMpedidos");
            dep.OnChanged += Dep_OnChanged2;
            dep.Start();
        }
        private void Dep_OnChanged2(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Pedido2> e)
        {

            if (e.ChangeType != ChangeType.None)
            {
                object[] parametros = new object[12];
                parametros[0] = e.Entity.Protocolo;
                parametros[1] = e.Entity.UsuarioEmissor;
                parametros[2] = e.Entity.DataDoPedido;
                parametros[3] = e.Entity.ResponsavelPelaEntrega;
                parametros[4] = e.Entity.Tipo;
                parametros[5] = e.Entity.DataDeEntrega;
                parametros[6] = e.Entity.StatusDoPedido;
                parametros[7] = e.Entity.Veiculo;
                parametros[8] = e.Entity.Quilometragem;
                parametros[9] = e.Entity.Frete;
                parametros[10] = e.Entity.Transportadora;
                parametros[11] = e.Entity.Observacao;

                loading = true;

                Invoke(new Action(() =>
                {
                    dataGridView2.DataSource = null;                    
                }));

                if (e.ChangeType == ChangeType.Insert)
                {
                    Program.Pedidos.Table.Rows.Add(parametros);                    
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    var PedidoAntigo = Program.Pedidos.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.Protocolo.ToString()).ToList()[0];
                    Program.Pedidos.Table.Rows.Remove(PedidoAntigo);                   

                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var PedidoAntigo = Program.Pedidos.Table.AsEnumerable().Where(x => x[0].ToString() == e.Entity.Protocolo.ToString()).ToList()[0];
                    Program.Pedidos.Table.Rows.Remove(PedidoAntigo);
                    Program.Pedidos.Table.Rows.Add(parametros);                   
                }

                Invoke(new Action(() =>
                {
                    dataGridView2.DataSource = Program.Pedidos;
                }));

                loading = false;

                StatusDoPedido();
            }

        }
    
        //Filtar com Enter
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {

                List<string> filtro = new List<string>();

                Program.Notas.RowFilter = "";

                if (textBox1.Text != String.Empty)
                {

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            try
                            {
                                if (dataGridView1.Rows[i].Cells[j].Value.ToString().Contains(textBox1.Text) == true)
                                {
                                    filtro.Add(dataGridView1.Rows[i].Cells[2].Value.ToString());
                                }
                            }
                            catch { }

                        }
                    }

                    if (filtro.Count > 0)
                    {
                        Program.Notas.RowFilter = "NumDoPedido IN(" + String.Join(",", filtro) + ")";
                        StatusDaNota();
                    }
                    else if (filtro.Count == 0 & textBox1.Text != String.Empty)
                    {
                        Program.Notas.RowFilter = "NumDoPedido =-1";
                        StatusDaNota();
                    }


                }
                else { Program.Notas.RowFilter = ""; StatusDaNota(); }
            }

        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                List<string> filtro = new List<string>();

                Program.Pedidos.RowFilter = "";

                if (textBox2.Text != String.Empty)
                {
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            try
                            {
                                if (dataGridView2.Rows[i].Cells[j].Value.ToString().Contains(textBox2.Text) == true)
                                {
                                    filtro.Add(dataGridView2.Rows[i].Cells[0].Value.ToString());
                                }
                            }
                            catch { }

                        }
                    }

                    if (filtro.Count > 0)
                    {
                        Program.Pedidos.RowFilter = "Protocolo IN(" + String.Join(",", filtro) + ")";
                        StatusDoPedido();
                    }
                    if (filtro.Count == 0 & textBox2.Text != String.Empty)
                    {
                        Program.Pedidos.RowFilter = "Protocolo =-1";
                        StatusDoPedido();
                    }

                }
                else { Program.Pedidos.RowFilter = ""; StatusDoPedido(); }
            }
        }

         //Multi Select
        private void button5_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Permissão != 3)
            {
                dataGridView1.Visible = false;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    int index = row.Index;

                    if (Program.Notas[index]["CheckColumn"] != DBNull.Value && Program.Notas[index]["CheckColumn"] != null)
                    {

                        if (Convert.ToBoolean(Program.Notas[index]["CheckColumn"]) == true)
                        {
                            Program.Notas[index]["CheckColumn"] = false;
                            NotasSelecionadas.Remove(Convert.ToInt32(dataGridView1.Rows[index].Cells[9].Value));
                        }
                        else if (Convert.ToBoolean(Program.Notas[index]["CheckColumn"]) == false)
                        {
                            Program.Notas[index]["CheckColumn"] = true;
                            NotasSelecionadas.Add(Convert.ToInt32(dataGridView1.Rows[index].Cells[9].Value));
                        }
                    }
                    else
                    {
                        Program.Notas[index]["CheckColumn"] = true;
                        NotasSelecionadas.Add(Convert.ToInt32(dataGridView1.Rows[index].Cells[9].Value));
                    }
                }

                dataGridView1.Visible = true;
                
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;

            Program.Notas.RowFilter = "";

            for (int n = 0; n < Program.Notas.Count; n++)
            {
                Program.Notas[n]["CheckColumn"] = false;
            }
            NotasSelecionadas.Clear();          

            dataGridView1.Visible = true;

            FiltroTabela1();
        }

        //Editar várias
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = comboBox5.SelectedIndex;

            if(loading==false & Properties.Settings.Default.Permissão != 3 & NotasSelecionadas.Count>0 & item!=0)
            {
                if(MessageBox.Show("Deseja modificar todas as notas selecionadas?","Modificar Status",MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    string notas = "(";
                    int count = 0;

                    foreach(int nota in NotasSelecionadas)
                    {
                        notas += nota.ToString()+",";
                        count++;
                    }
                    notas = notas.Remove(notas.Length - 1, 1);
                    notas += ")";

                    Console.WriteLine(notas);
                    
                    SqlConnection connection = new SqlConnection(Properties.Settings.Default.RSYS2003ConnectionString);
                    string commandString = "";

                    if (item != 1)
                    {
                        commandString = "UPDATE BRASCAMnotas SET StatusDoPedido=" + item + " WHERE CodigoDaNota IN "+notas;
                    }
                    else
                    {
                        commandString = "UPDATE BRASCAMnotas SET StatusDoPedido=" + item + ",CodigoPedidoDeEntrega=null,DataDeEntrega=null,Responsavel=null WHERE CodigoDaNota IN "+notas;
                    }

                    SqlCommand command = new SqlCommand(commandString, connection);

                    //Program.Notas[n]["StatusDopedido"] = item;

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erro de conexão");
                        return;
                    }

                }

                NotasSelecionadas.Clear();
                comboBox5.SelectedIndex = 0;
                
            }
        }

    }


}
