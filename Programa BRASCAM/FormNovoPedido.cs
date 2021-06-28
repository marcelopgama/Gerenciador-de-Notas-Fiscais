using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Programa_BRASCAM
{
    public partial class FormNovoPedido : Form
    {
        bool arrastar = false;
        public bool Visualizando;
        int posiçãoX, posiçãoY;
        public Pedido Pedido = new Pedido();        
        List<NotaFiscal> notasDoPedido;
        List<int> notasSelecionadas;
        List<int> codigoDasNotas=new List<int>();        
        List<NotaFiscal> notasRemovidas = new List<NotaFiscal>();
        int inícioDasPedidosCriados = 0;
        string protocolo = "";
        string novasNotas;
        bool atualizando = false;
        double frete;
        bool carregando = true;

        DateTime data = DateTime.Today.AddDays(-60);
        string dataFilter = "";


        internal List<NotaFiscal> NotasDoPedido { get => notasDoPedido; set => notasDoPedido = value; }
        public List<int> NotasSelecionadas { get => notasSelecionadas; set => notasSelecionadas = value; }
        public string NovasNotas { get => novasNotas; set => novasNotas = value; }

        public FormNovoPedido()
        {         

            InitializeComponent();                     
            notasDoPedido = new List<NotaFiscal>();

            dataFilter=data.Year.ToString("0000") + "-" + data.Month.ToString("00") + "-" + data.Day.ToString("00");

            if (Programa_BRASCAM.Properties.Settings.Default.Permissão == 3)
            {
                maskedTextBox1.ReadOnly = true;
                maskedTextBox2.ReadOnly = true;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                textBox6.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox1.ReadOnly = true;
                button8.Enabled = false;
                button3.Enabled = false;
                button2.Enabled = false;
                button9.Enabled = false;
                button10.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button7.Enabled = false;
                dataGridView1.Columns[4].ReadOnly = true;
                dataGridView1.Columns[5].ReadOnly = true;
                dataGridView1.Columns[6].ReadOnly = true;
                dataGridView1.Columns[7].ReadOnly = true;
            }
           
        }
        private void FormNovoPedido_Load(object sender, EventArgs e)
        {           

            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet10.BRASCAMtransportadora'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMtransportadoraTableAdapter.Fill(this.rSYS2003DataSet10.BRASCAMtransportadora);
            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet3.BRASCAMresponsavel'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMresponsavelTableAdapter.Fill(this.rSYS2003DataSet3.BRASCAMresponsavel);
            // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet2.BRASCAMveiculos'. Você pode movê-la ou removê-la conforme necessário.
            this.bRASCAMveiculosTableAdapter.Fill(this.rSYS2003DataSet2.BRASCAMveiculos);

            if (Visualizando == true)
            {               
                
                PreencherCampos();
                CarregarNotasDoPedido();
                AtualizarDataGrid();
                inícioDasPedidosCriados = NotasDoPedido.Count - 1;

                DataGridViewComboBoxColumn boxColumn = dataGridView1.Columns[5] as DataGridViewComboBoxColumn;
                boxColumn.Items[0] = "NF retornou";
            }
            else
            {
                DataGridViewComboBoxColumn boxColumn = dataGridView1.Columns[5] as DataGridViewComboBoxColumn;
                

                textBox2.Text = frete.ToString();

                comboBox1.Text = String.Empty;
                comboBox2.Text = String.Empty;
                comboBox3.Text = String.Empty;

                textBox3.Text = Programa_BRASCAM.Properties.Settings.Default.Usuário;
                maskedTextBox1.Text = DateTime.Today.ToShortDateString();
                maskedTextBox2.Text = DateTime.Today.ToShortDateString();               

                CarregarNotasDoPedido();
                AtualizarDataGrid();
            }
            
            codigoDasNotas = new List<int>();
            carregando = false;
        }
        void PreencherCampos()
        {
            protocolo = Pedido.Protocolo.ToString();

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
         
            SqlCommand command = new SqlCommand("SELECT Protocolo,UsuarioEmissor," +
             "DataDoPedido,ResponsavelPelaEntrega,Tipo,DataDeEntrega," +
             "StatusDoPedido,Veiculo,Quilometragem,Frete,Transportadora," +
             "Observacao,QuilometragemFinal FROM BRASCAMPedidos " +
             "WHERE Protocolo="+protocolo, connection);

            try
            {

                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    try { Pedido.Protocolo = dataReader.GetValue(0); } catch (NullReferenceException ex) { }
                    try { Pedido.Usuário = dataReader.GetValue(1); } catch (NullReferenceException ex) { }
                    try { Pedido.DataDoPedido = dataReader.GetValue(2); } catch (NullReferenceException ex) { }
                    try { Pedido.ResponsávelPelaEntrega = dataReader.GetValue(3); } catch (NullReferenceException ex) { }
                    try { Pedido.Tipo = dataReader.GetValue(4); } catch (NullReferenceException ex) { }
                    try { Pedido.DataDeEntrega = dataReader.GetValue(5); } catch (NullReferenceException ex) { }
                    try { Pedido.Status = dataReader.GetValue(6); } catch (NullReferenceException ex) { }
                    try { Pedido.Veículo = dataReader.GetValue(7); } catch (NullReferenceException ex) { }
                    try { Pedido.Quilometragem = dataReader.GetValue(8); } catch (NullReferenceException ex) { }
                    try { Pedido.Frete = dataReader.GetValue(9); } catch (NullReferenceException ex) { }
                    try { Pedido.Transportadora = dataReader.GetValue(10); } catch (NullReferenceException ex) { }
                    try { Pedido.Observação = dataReader.GetValue(11); } catch (NullReferenceException ex) { }
                    try { Pedido.QuilometragemFinal = dataReader.GetValue(12); } catch (NullReferenceException ex) { }

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro de conexão");
                Close();
                return;
            }


        try { textBox3.Text = Pedido.Usuário.ToString();}catch (NullReferenceException ex) { }
        try { comboBox1.Text = Pedido.ResponsávelPelaEntrega.ToString();}catch (NullReferenceException ex) { }
        try{ comboBox2.Text = Pedido.Veículo.ToString(); }catch (NullReferenceException ex) { }
        try{ textBox6.Text = Pedido.Quilometragem.ToString().Replace('.',','); }catch (NullReferenceException ex) { }
        try { textBox4.Text = Pedido.QuilometragemFinal.ToString().Replace('.', ','); } catch (NullReferenceException ex) { }
        try { textBox2.Text = Pedido.Frete.ToString().Replace('.', ','); }catch (NullReferenceException ex) { }
        try{ comboBox3.Text = Pedido.Transportadora.ToString(); } catch (NullReferenceException ex) { }
        try{ maskedTextBox1.Text = Pedido.DataDoPedido.ToString();}catch (NullReferenceException ex) { }
        try{ maskedTextBox2.Text = Pedido.DataDeEntrega.ToString(); }catch (NullReferenceException ex) { }
        try { textBox7.Text = Convert.ToInt32(Pedido.Protocolo).ToString("0000"); } catch (NullReferenceException ex) { }
        try { textBox1.Text = Pedido.Observação.ToString(); } catch (NullReferenceException ex) { }
        try { comboBox4.SelectedItem = comboBox4.Items[Convert.ToInt32(Pedido.Tipo)-1]; } catch{ }
            
        }
        void CarregarNotasDoPedido()
        {
            if (Visualizando == true)
            {

                protocolo = Pedido.Protocolo.ToString();
                SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
                
                 try
                {
                    //Notas
                    SqlCommand command = new SqlCommand("SELECT NumDaNota,NumDoPedido,DataDaNota,CodDoCliente," +
                    "StatusDoPedido,Tipo,ReceberDinheiro,CodigoPedidoDeEntrega,CodigoDaNota,Frete," +
                    "ModoDeEntrega,TipoDeEntrega FROM BRASCAMnotas WHERE CodigoPedidoDeEntrega=" + protocolo, connection);

                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();

                    notasDoPedido = new List<NotaFiscal>();
                    while (dataReader.Read())
                    {
                        NotaFiscal novaNota = new NotaFiscal();
                        novaNota.CodDoCliente = dataReader.GetValue(3);
                        novaNota.RedecerDinheiro = dataReader.GetValue(6);
                        novaNota.NumDoPedido = dataReader.GetValue(1);
                        novaNota.NumDaNota = dataReader.GetValue(0);
                        novaNota.Tipo = dataReader.GetValue(5);
                        novaNota.StatusDoPedido = dataReader.GetValue(4);
                        novaNota.DataDaNota = dataReader.GetValue(2);
                        novaNota.CodigoDaNota = dataReader.GetValue(8);
                        novaNota.NumPedidoDeEntrega = dataReader.GetValue(7);
                        novaNota.Frete = dataReader.GetValue(9);
                        novaNota.ModoDeEntrega = dataReader.GetValue(10);
                        novaNota.TipoDeEntrega = dataReader.GetValue(11);
                        notasDoPedido.Add(novaNota);
                    }

                    connection.Close();

                    try
                    {
                        frete = Convert.ToDouble(Pedido.Frete);
                        textBox2.Text = frete.ToString();

                    }
                    catch { };

                    //Pedidos de entrega (sem nota)
                    SqlCommand command2 = new SqlCommand("SELECT NumDaNota,NumDoPedido,DataDaNota,CodDoCliente," +
                    "StatusDoPedido,Tipo,ReceberDinheiro,CodigoPedidoDeEntrega,CodigoDaNota,Frete," +
                    "ModoDeEntrega,TipoDeEntrega FROM BRASCAMpedDeEntrega WHERE CodigoPedidoDeEntrega=" + protocolo, connection);

                    connection.Open();
                    SqlDataReader dataReader2 = command2.ExecuteReader();
                                        
                    while (dataReader2.Read())
                    {
                        NotaFiscal novaNota = new NotaFiscal();
                        novaNota.CodDoCliente = dataReader2.GetValue(3);
                        novaNota.RedecerDinheiro = dataReader2.GetValue(6);
                        novaNota.NumDoPedido = dataReader2.GetValue(1);
                        novaNota.NumDaNota = dataReader2.GetValue(0);
                        novaNota.Tipo = dataReader2.GetValue(5);
                        novaNota.StatusDoPedido = dataReader2.GetValue(4);
                        novaNota.DataDaNota = dataReader2.GetValue(2);
                        novaNota.CodigoDaNota = dataReader2.GetValue(8);
                        novaNota.NumPedidoDeEntrega = dataReader2.GetValue(7);
                        novaNota.Frete = dataReader2.GetValue(9);
                        novaNota.ModoDeEntrega = dataReader2.GetValue(10);
                        novaNota.TipoDeEntrega = dataReader2.GetValue(11);
                        notasDoPedido.Add(novaNota);
                    }

                    connection.Close();

                    try
                    {
                        frete = Convert.ToDouble(Pedido.Frete);
                        textBox2.Text = frete.ToString();

                    }
                    catch { };
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão");
                    Close();
                    return;
                }

            }
            else
            {
                frete = 0;                

                notasDoPedido = new List<NotaFiscal>();

                NovasNotas = String.Join(",", NotasSelecionadas);

                SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);

                string connectionString = "SELECT NumDaNota,NumDoPedido,DataDaNota,CodDoCliente," +
                    "StatusDoPedido,Tipo,ReceberDinheiro,CodigoPedidoDeEntrega,CodigoDaNota,Frete," +
                    "ModoDeEntrega,TipoDeEntrega FROM BRASCAMnotas";

                if (String.IsNullOrEmpty(NovasNotas) == false)
                {
                    connectionString += " WHERE CodigoDaNota IN (" + NovasNotas+")";

                    SqlCommand command = new SqlCommand(connectionString, connection);
                    try
                    {

                        connection.Open();
                        SqlDataReader dataReader = command.ExecuteReader();


                        while (dataReader.Read())
                        {
                            NotaFiscal novaNota = new NotaFiscal();
                            novaNota.CodDoCliente = dataReader.GetValue(3);
                            novaNota.RedecerDinheiro = dataReader.GetValue(6);
                            novaNota.NumDoPedido = dataReader.GetValue(1);
                            novaNota.NumDaNota = dataReader.GetValue(0);
                            novaNota.Tipo = dataReader.GetValue(5);
                            novaNota.StatusDoPedido = 2;
                            novaNota.DataDaNota = dataReader.GetValue(2);
                            novaNota.CodigoDaNota = dataReader.GetValue(8);
                            novaNota.NumPedidoDeEntrega = dataReader.GetValue(7);
                            novaNota.Frete = dataReader.GetValue(9);                            
                            novaNota.ModoDeEntrega = dataReader.GetValue(10);
                            novaNota.TipoDeEntrega = dataReader.GetValue(11);
                            notasDoPedido.Add(novaNota);

                           try
                           {
                                frete += Convert.ToDouble(dataReader.GetValue(9));
                                textBox2.Text = frete.ToString();
                           }
                            catch { };
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
        }
        void AtualizarDataGrid()
        {
            atualizando = true;
                       

            dataGridView1.Rows.Clear();
            for (int n = 0; n < notasDoPedido.Count; n++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = notasDoPedido[n].NumDaNota;
                dataGridView1.Rows[n].Cells[1].Value = notasDoPedido[n].NumDoPedido;
                dataGridView1.Rows[n].Cells[2].Value = Convert.ToDateTime(notasDoPedido[n].DataDaNota).ToShortDateString();
                dataGridView1.Rows[n].Cells[3].Value = notasDoPedido[n].CodDoCliente;                
                dataGridView1.Rows[n].Cells[7].Value = notasDoPedido[n].CodigoDaNota;

                DataGridViewComboBoxCell comboBoxCellTipo = new DataGridViewComboBoxCell();
                comboBoxCellTipo = dataGridView1.Rows[n].Cells[4] as DataGridViewComboBoxCell;
                comboBoxCellTipo.Value = comboBoxCellTipo.Items[Convert.ToInt32(NotasDoPedido[n].TipoDeEntrega) - 1];

                DataGridViewComboBoxCell comboBoxCellStatus = new DataGridViewComboBoxCell();
                comboBoxCellStatus = dataGridView1.Rows[n].Cells[5] as DataGridViewComboBoxCell;               
                comboBoxCellStatus.Value = comboBoxCellStatus.Items[Convert.ToInt32(NotasDoPedido[n].StatusDoPedido) - 1];

                DataGridViewComboBoxCell comboBoxCellReceber = new DataGridViewComboBoxCell();
                comboBoxCellReceber = dataGridView1.Rows[n].Cells[6] as DataGridViewComboBoxCell;
                comboBoxCellReceber.Value = comboBoxCellReceber.Items[Convert.ToInt32(notasDoPedido[n].RedecerDinheiro)];

                DataGridViewComboBoxCell comboBoxCellModo = new DataGridViewComboBoxCell();
                comboBoxCellModo = dataGridView1.Rows[n].Cells[8] as DataGridViewComboBoxCell;
                comboBoxCellModo.Value = comboBoxCellModo.Items[Convert.ToInt32(notasDoPedido[n].ModoDeEntrega)];
               
            }
            atualizando = false;
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
                    SolidBrush brush = new SolidBrush(txt.BackColor);
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1+7);
                    e.Graphics.DrawRectangle(caneta, rect);
                    e.Graphics.FillRectangle(brush, txt.Location.X, txt.Location.Y+txt.Height, txt.Width, 7);
                }
                else if (controle.Name.Contains("maskedTextBox"))
                {
                    MaskedTextBox txt = (MaskedTextBox)controle;
                    SolidBrush brush = new SolidBrush(txt.BackColor);
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1 + 7);
                    e.Graphics.DrawRectangle(caneta, rect);
                    e.Graphics.FillRectangle(brush, txt.Location.X, txt.Location.Y + txt.Height, txt.Width, 7);
                }
                else if (controle.Name.Contains("comboBox"))
                {
                    ComboBox txt = (ComboBox)controle;
                    SolidBrush brush = new SolidBrush(txt.BackColor);
                    Rectangle rect = new Rectangle(controle.Location.X, controle.Location.Y, controle.Width - 1, controle.Height - 1);
                    e.Graphics.DrawRectangle(caneta, rect);                   
                }

            }
        }
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb(100, 170, 170, 170));

            Rectangle rect = new Rectangle(0, 0, dataGridView1.Width, 22);
            e.Graphics.FillRectangle(brush, rect);

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
            DialogResult = DialogResult.Cancel;
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
        private void button5_Click(object sender, EventArgs e)
        {     

            //Criando o pedido a ser salvo
            if(textBox7.Text!=String.Empty)
            {
                Pedido.Protocolo = textBox7.Text;
            }

            Pedido.Usuário = "'"+textBox3.Text.Replace(" ",String.Empty)+"'";
            Pedido.ResponsávelPelaEntrega = "'"+comboBox1.Text.Replace(" ", String.Empty) + "'";
            Pedido.Veículo = "'"+comboBox2.Text.Replace(" ", String.Empty)+"'";
            Pedido.Quilometragem = "0" + textBox6.Text.Replace(',', '.');
            Pedido.QuilometragemFinal="0"+textBox4.Text.Replace(',', '.');
            Pedido.Frete = "0"+textBox2.Text.Replace(',','.');
            Pedido.Transportadora = "'"+comboBox3.Text.Replace(" ", String.Empty) + "'";            
            Pedido.Observação = "'"+textBox1.Text + "'"; 

            int l = 0;
            foreach(DataGridViewRow row in dataGridView1.Rows) 
            {
                DataGridViewComboBoxCell boxCell = row.Cells[5] as DataGridViewComboBoxCell;

                if (boxCell.Value.ToString() == boxCell.Items[3].ToString()) 
                {
                    l++;
                }            
            }
            if (l == dataGridView1.Rows.Count) 
            {
                Pedido.Status = 2;
            }
            else { Pedido.Status = 1; }
            

            if (comboBox4.SelectedItem!=null) 
            {
                Pedido.Tipo = comboBox4.SelectedIndex + 1;
            }
            else {
                MessageBox.Show("Escolha o tipo de pedido de entrega.");
                return;
            }
            

            //Verificando as datas
            try
            {
                Pedido.DataDoPedido = "'"+DateTime.Parse(maskedTextBox1.Text).Year.ToString("0000") + DateTime.Parse(maskedTextBox1.Text).Month.ToString("00") + DateTime.Parse(maskedTextBox1.Text).Day.ToString("00")+ "'" ;
                Pedido.DataDeEntrega = "'" + DateTime.Parse(maskedTextBox2.Text).Year.ToString("0000") + DateTime.Parse(maskedTextBox2.Text).Month.ToString("00") + DateTime.Parse(maskedTextBox2.Text).Day.ToString("00")+ "'";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Digite as datas corretamente.");
                return;
            }

            //Verificando se há um pedido existente com o mesmo entregador e carro
            string text = comboBox1.Text;
            string parâmetro = "ResponsavelPelaEntrega";

            if (text != String.Empty && carregando == false)
            {
                VerificarSeExiste(dataFilter, text, parâmetro);
            }
            text = comboBox2.Text;
            parâmetro = "Veiculo";
            if (text != String.Empty && carregando == false)
            {
                VerificarSeExiste(dataFilter, text, parâmetro);
            }

            VerificarQuilometragem();

            string commandString="";
            
            //Gerando string de update
            if (Visualizando == true)
            {

                commandString = "UPDATE BRASCAMpedidos SET UsuarioEmissor =" + Pedido.Usuário;
                if(Pedido.DataDoPedido.ToString()!="" & Pedido.DataDoPedido.ToString() != String.Empty)
                {
                    commandString += ",DataDoPedido =" + Pedido.DataDoPedido;
                }
                if (Pedido.ResponsávelPelaEntrega.ToString() != "" & Pedido.ResponsávelPelaEntrega.ToString() != String.Empty)
                {
                    commandString += ",ResponsavelPelaEntrega =" + Pedido.ResponsávelPelaEntrega;
                }
                if (Pedido.DataDeEntrega.ToString() != "" & Pedido.DataDeEntrega.ToString() != String.Empty)
                {
                    commandString += ",DataDeEntrega =" + Pedido.DataDeEntrega;
                }
                if (Pedido.Tipo.ToString() != "" & Pedido.Tipo.ToString() != String.Empty)
                {
                    commandString += ",Tipo =" + Pedido.Tipo;
                }
                if (Pedido.Status.ToString() != "" & Pedido.Status.ToString() != String.Empty)
                {
                    commandString += ",StatusDoPedido =" + Pedido.Status;
                }
                if (Pedido.Veículo.ToString() != "" & Pedido.Veículo.ToString() != String.Empty)
                {
                    commandString += ",Veiculo =" + Pedido.Veículo;
                }
                if (Pedido.Quilometragem.ToString() != "" & Pedido.Quilometragem.ToString() != String.Empty)
                {
                    commandString += ",Quilometragem =" + Pedido.Quilometragem;
                }
                if (Pedido.QuilometragemFinal.ToString() != "" & Pedido.QuilometragemFinal.ToString() != String.Empty)
                {
                    commandString += ",QuilometragemFinal =" + Pedido.QuilometragemFinal;
                }
                if (Pedido.Frete.ToString() != "" & Pedido.Frete.ToString() != String.Empty)
                {
                    commandString += ",Frete =" + Pedido.Frete;
                }
                if (Pedido.Transportadora.ToString() != "" & Pedido.Transportadora.ToString() != String.Empty)
                {
                    commandString += ",Transportadora =" + Pedido.Transportadora;
                }
                if (Pedido.Observação.ToString() != "" & Pedido.Observação.ToString() != String.Empty)
                {
                    commandString += ",Observacao =" + Pedido.Observação;
                }
                commandString+= " WHERE Protocolo=" + Pedido.Protocolo;

            }
            else//Gerando string de insert
            {
                commandString = "INSERT INTO BRASCAMpedidos (UsuarioEmissor";
                string colunas = "";
                string values = " VALUES ("+Pedido.Usuário;
                
                if (Pedido.DataDoPedido.ToString() != null & Pedido.DataDoPedido.ToString() != String.Empty)
                {
                    colunas += ",DataDoPedido";
                    values+= ","+Pedido.DataDoPedido;
                }
                if (Pedido.ResponsávelPelaEntrega.ToString() != null & Pedido.ResponsávelPelaEntrega.ToString() != String.Empty)
                {
                    colunas += ",ResponsavelPelaEntrega";
                    values+= "," + Pedido.ResponsávelPelaEntrega ;
                }
                if (Pedido.DataDeEntrega.ToString() != null & Pedido.DataDeEntrega.ToString() != String.Empty)
                {
                    colunas += ",DataDeEntrega";
                    values+= "," + Pedido.DataDeEntrega;
                }
                if (Pedido.Tipo.ToString() != null & Pedido.Tipo.ToString() != String.Empty)
                {
                    colunas += ",Tipo";
                    values+= "," +  Pedido.Tipo;
                }
                
                    colunas += ",StatusDoPedido";
                    values+= ","+Pedido.Status;
                
                if (Pedido.Veículo.ToString() != "" & Pedido.Veículo.ToString() != String.Empty)
                {
                    colunas += ",Veiculo";
                    values += "," + Pedido.Veículo;
                }
                if (Pedido.Quilometragem.ToString() != "" & Pedido.Quilometragem.ToString() != String.Empty)
                {
                    colunas += ",Quilometragem";
                    values+= "," + Pedido.Quilometragem;
                }
                if (Pedido.QuilometragemFinal.ToString() != "" & Pedido.QuilometragemFinal.ToString() != String.Empty)
                {
                    colunas += ",QuilometragemFinal";
                    values += "," + Pedido.QuilometragemFinal;
                }
                if (Pedido.Frete.ToString() != "" & Pedido.Frete.ToString() != String.Empty)
                {
                    colunas += ",Frete";
                    values+= "," + Pedido.Frete;
                }
                if (Pedido.Transportadora.ToString() != "" & Pedido.Transportadora.ToString() != String.Empty)
                {
                    colunas += ",Transportadora";
                    values+= "," + Pedido.Transportadora;
                }
                if (Pedido.Observação.ToString() != "" & Pedido.Observação.ToString() != String.Empty)
                {
                    colunas += ",Observacao";
                    values+= "," + Pedido.Observação;
                }

                colunas += ")";values += ")";
                commandString += colunas + values;
            }           
            

            SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
            SqlCommand command = new SqlCommand(commandString, connection);

            //Criando/atualizando o pedido
            try
            {

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro de conexão. Verifique se o pedido foi criado/atualizado corretamente.");
                Close();
                return;
            }

            //N° de protocolo
            if (Visualizando==false)
            {
                try
                {
                    string commandString2 = "SELECT MAX(Protocolo) FROM BRASCAMpedidos";
                    SqlConnection connection2 = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
                    SqlCommand command2 = new SqlCommand(commandString2, connection2);
                    connection2.Open();
                    SqlDataReader dataReader = command2.ExecuteReader();

                    while (dataReader.Read())
                    {
                        try
                        {
                            Pedido.Protocolo = dataReader.GetInt32(0);
                            textBox7.Text = Convert.ToInt32(Pedido.Protocolo).ToString("0000");
                        }
                        catch { }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão. Verifique se o pedido foi criado/atualizado corretamente.");
                    return;
                }
            }

            Visualizando = true;

            //Atualizando quilometragem do carro
            if (Pedido.QuilometragemFinal.ToString() != "" & Pedido.QuilometragemFinal.ToString() != String.Empty)
            {
                try
                {                    
                    string commandStringKm = "UPDATE BRASCAMveiculos SET Quilometragem =" + Pedido.QuilometragemFinal
                    +" WHERE Placa="+Pedido.Veículo;
                    SqlCommand commandKm = new SqlCommand(commandStringKm, connection);                   
                    
                    connection.Open();
                    commandKm.ExecuteNonQuery();
                    connection.Close();
               }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão. O pedido foi criado mas a quilometragem do carro e as notas não foram atualizados corretamente.");                    
                    return;
                }
            }
            
           //Atualizando notas do pedido
            try
                {
                foreach (NotaFiscal nota in notasRemovidas)
                {
                    string commandString2 = "";

                    if (nota.NumDaNota != null)
                    {
                        commandString2 = "UPDATE BRASCAMnotas SET CodigoPedidoDeEntrega=NULL, " +
                            "DataDeEntrega=NULL,Responsavel=null, StatusDopedido=1 WHERE CodigoDaNota=" + nota.CodigoDaNota;
                    }
                    else
                    {
                        commandString2 = "UPDATE BRASCAMpedDeEntrega SET CodigoPedidoDeEntrega=NULL, " +
                           "DataDeEntrega=NULL,Responsavel=null, StatusDopedido=1 WHERE CodigoDaNota=" + nota.CodigoDaNota;
                    }

                    SqlCommand command2 = new SqlCommand(commandString2, connection);
                    connection.Open();
                    command2.ExecuteNonQuery();
                    connection.Close();                    
                }

                notasRemovidas.Clear();

                if (NotasDoPedido.Count > 0)
                    {
                        foreach (NotaFiscal nota in NotasDoPedido)
                        {

                        
                        if(nota.NumDaNota==null || nota.NumDaNota.ToString()=="")
                        {

                            Console.WriteLine("Executando");

                            string commandString2 = "UPDATE BRASCAMpedDeEntrega SET StatusDoPedido=" + nota.StatusDoPedido +
                                      ", ReceberDinheiro=" + nota.RedecerDinheiro + ", CodigoPedidoDeEntrega=" + Pedido.Protocolo
                                      + ",DataDeEntrega=" + Pedido.DataDeEntrega + ",Responsavel=" + Pedido.ResponsávelPelaEntrega
                                      + ", ModoDeEntrega=" + nota.ModoDeEntrega + ",TipoDeEntrega=" + nota.TipoDeEntrega + " WHERE CodigoDaNota=" + nota.CodigoDaNota;

                            SqlCommand command2 = new SqlCommand(commandString2, connection);
                            connection.Open();
                            command2.ExecuteNonQuery();
                            connection.Close();
                        }
                        else
                        {
                            Console.WriteLine("Nota:" + nota.NumDaNota);

                            string commandString2 = "UPDATE BRASCAMnotas SET StatusDoPedido=" + nota.StatusDoPedido +
                                      ", ReceberDinheiro=" + nota.RedecerDinheiro + ", CodigoPedidoDeEntrega=" + Pedido.Protocolo
                                      + ",DataDeEntrega=" + Pedido.DataDeEntrega + ",Responsavel=" + Pedido.ResponsávelPelaEntrega
                                      + ", ModoDeEntrega=" + nota.ModoDeEntrega + ",TipoDeEntrega=" + nota.TipoDeEntrega + " WHERE CodigoDaNota=" + nota.CodigoDaNota;

                            SqlCommand command2 = new SqlCommand(commandString2, connection);
                            connection.Open();
                            command2.ExecuteNonQuery();
                            connection.Close();
                        }
                       

                    }
                    }
                    
                    Visualizando = true;

                    if(MessageBox.Show("Configurações salvas! Deseja fechar a janela?", "", MessageBoxButtons.YesNo) == DialogResult.Yes) 
                    {
                        DialogResult = DialogResult.OK;
                    };
                    
                }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro de conexão. O pedido foi criado/atualizado. Verifique se as notas foram atualizadas corretamente.");                
                return;
            }

        }

        //Editando ComboBoxes
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int receber = 0;
            int status = 0;
            int modo = 0;
            int tipo = 0;

            if (atualizando == false)
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewComboBoxCell comboBoxCellReceber = new DataGridViewComboBoxCell();
                    comboBoxCellReceber = dataGridView1.Rows[e.RowIndex].Cells[6] as DataGridViewComboBoxCell;

                    if (comboBoxCellReceber.Value != null)
                    {
                        receber = comboBoxCellReceber.Items.IndexOf(comboBoxCellReceber.Value.ToString());
                    }

                    DataGridViewComboBoxCell comboBoxCellStatus = new DataGridViewComboBoxCell();
                    comboBoxCellStatus = dataGridView1.Rows[e.RowIndex].Cells[5] as DataGridViewComboBoxCell;

                    if (comboBoxCellStatus.Value != null)
                    {
                        status = comboBoxCellStatus.Items.IndexOf(comboBoxCellStatus.Value)+1;                       
                    }

                    DataGridViewComboBoxCell comboBoxCellModo = new DataGridViewComboBoxCell();
                    comboBoxCellModo = dataGridView1.Rows[e.RowIndex].Cells[8] as DataGridViewComboBoxCell;

                    if (comboBoxCellModo.Value != null)
                    {
                        modo = comboBoxCellModo.Items.IndexOf(comboBoxCellModo.Value);
                    }

                    DataGridViewComboBoxCell comboBoxCellTipo = new DataGridViewComboBoxCell();
                    comboBoxCellTipo = dataGridView1.Rows[e.RowIndex].Cells[4] as DataGridViewComboBoxCell;

                    if (comboBoxCellTipo.Value != null)
                    {
                        tipo = comboBoxCellTipo.Items.IndexOf(comboBoxCellTipo.Value.ToString())+1;
                    }

                    notasDoPedido[comboBoxCellReceber.RowIndex].RedecerDinheiro = receber;
                    notasDoPedido[comboBoxCellStatus.RowIndex].StatusDoPedido = status;
                    notasDoPedido[comboBoxCellModo.RowIndex].ModoDeEntrega = modo;
                    notasDoPedido[comboBoxCellTipo.RowIndex].TipoDeEntrega = tipo;
                }
            }
        }

        //Adicionar Pedido
        private void button8_Click(object sender, EventArgs e)
        {
            List<int> duplicada = new List<int>();

            foreach (NotaFiscal nota in notasDoPedido)
            {
                if (nota.CodigoDaNota != null)
                {
                    duplicada.Add(Convert.ToInt32(nota.CodigoDaNota));
                }
            }

            FormAdicionarPedido formAdicionarPedido = new FormAdicionarPedido();
            formAdicionarPedido.ShowDialog();           

            if (formAdicionarPedido.DialogResult == DialogResult.OK)
            {
                
                for (int i = 0; i < formAdicionarPedido.Notas; i++) 
                {
                    carregando = true;

                    NotaFiscal novaNota = new NotaFiscal();
                    novaNota.CodDoCliente = formAdicionarPedido.CodCliente;
                    novaNota.RedecerDinheiro = Convert.ToInt32(formAdicionarPedido.ReceberDinheiro);
                    novaNota.NumDaNota = null;
                    novaNota.ModoDeEntrega = 0;
                    novaNota.TipoDeEntrega = Convert.ToInt32(formAdicionarPedido.Tipo);
                    novaNota.StatusDoPedido = Convert.ToInt32(formAdicionarPedido.Status);
                    novaNota.DataDaNota = formAdicionarPedido.DataDaNota;
                    novaNota.NumDoPedido = formAdicionarPedido.NumeroDoPedido;
                    novaNota.Frete = formAdicionarPedido.Frete;
                    novaNota.CodigoDaNota = formAdicionarPedido.CodigoDaNota;

                    try
                    {
                        frete += Convert.ToDouble(novaNota.Frete);
                        textBox2.Text = frete.ToString();
                    }
                    catch { };

                    if (duplicada.Contains(Convert.ToInt32(novaNota.CodigoDaNota)) == false)
                    {
                        notasDoPedido.Add(novaNota);
                        AtualizarDataGrid();
                        carregando = false;
                    }

                }
              
            }


        }
        //Adicionar Nota
        private void button3_Click(object sender, EventArgs e)
        {
            List<int> duplicada = new List<int>();

            foreach(NotaFiscal nota in notasDoPedido)
            {
                if (nota.CodigoDaNota !=null)
                {
                    duplicada.Add(Convert.ToInt32(nota.CodigoDaNota));
                }
            }

            FormAdicionarNF formAdicionarPedido = new FormAdicionarNF();
            formAdicionarPedido.ShowDialog();

            if (formAdicionarPedido.DialogResult == DialogResult.OK)
            {
                carregando = true;
                NotaFiscal novaNota = new NotaFiscal();
                novaNota.CodDoCliente = formAdicionarPedido.CodCliente;
                novaNota.RedecerDinheiro = Convert.ToInt32(formAdicionarPedido.ReceberDinheiro);
                novaNota.NumDaNota = formAdicionarPedido.NumeroDaNota;
                novaNota.CodigoDaNota = formAdicionarPedido.CodigoDaNota;
                novaNota.TipoDeEntrega = Convert.ToInt32(formAdicionarPedido.Tipo);
                novaNota.StatusDoPedido = Convert.ToInt32(formAdicionarPedido.Status);
                novaNota.DataDaNota = formAdicionarPedido.DataDaNota;
                novaNota.ModoDeEntrega = 0;
                novaNota.NumDoPedido = formAdicionarPedido.NumeroDoPedido;
                novaNota.Frete = formAdicionarPedido.Frete;

                Console.WriteLine("Frete:" + novaNota.Frete);

                try
                {
                    frete += Convert.ToDouble(novaNota.Frete);
                    textBox2.Text = frete.ToString();
                }
                catch { };


                if (duplicada.Contains(Convert.ToInt32(novaNota.CodigoDaNota)) == false) 
                {
                    notasDoPedido.Add(novaNota);
                    AtualizarDataGrid();
                    carregando = false;
                }
                
            }


        }

        //Excluir Pedido
        private void button7_Click(object sender, EventArgs e)
        {

         if(Visualizando ==true)
            {
                if (MessageBox.Show("Deseja excluir o pedido?", "Alerta de exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string commandString = "DELETE FROM BRASCAMpedidos WHERE Protocolo=" + protocolo;
                    SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);
                    SqlCommand command = new SqlCommand(commandString, connection);
                    Console.WriteLine(commandString);

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

                    DialogResult = DialogResult.Abort;
                    Close();
                }
            }
        }

        //Excluir Nota/Pedido
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
               
                if (MessageBox.Show("Deseja excluir as notas e Pedidos selecionados?","Aviso de exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                   foreach(DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        notasRemovidas.Add(notasDoPedido[row.Index]);
                        try
                        {
                            frete -= Convert.ToDouble(notasDoPedido[row.Index].Frete);
                        }
                        catch { }
                        textBox2.Text = frete.ToString();
                        notasDoPedido.RemoveAt(row.Index);
                    }
                    carregando = true;
                    AtualizarDataGrid();
                    carregando = false;
                }
            }
        }
       
        //Bloquear Caracteres
        private void BloquearCharEspeciais_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            
            if (Char.IsLetterOrDigit(e.KeyChar) | Char.IsControl(e.KeyChar) | Char.IsPunctuation(e.KeyChar)
                | Char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = false;
            }
            else { e.Handled = true; }
        }
        private void BloquearCharEspeciais2_KeyPress(object sender, KeyPressEventArgs e)
        {
            ComboBox textBox = (ComboBox)sender;

            if (Char.IsLetterOrDigit(e.KeyChar) | Char.IsControl(e.KeyChar) | Char.IsPunctuation(e.KeyChar)
                | Char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = false;
            }
            else { e.Handled = true; }
        }
        private void BloquearLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string permitidos = "0123456789";

            if (permitidos.Contains(e.KeyChar.ToString()) == true | Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }else { e.Handled = true; }
        }
        private void permitirDecimals_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string permitidos = "0123456789,";

            if (e.KeyChar.ToString() == "," & textBox.Text.Contains(",") == true )
            {
                e.Handled = true;
            }            
            else if (permitidos.Contains(e.KeyChar.ToString()) == false & Char.IsControl(e.KeyChar)==false)
            {                
                e.Handled = true;
            }           
            
        }
        
        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Convert.ToString(Pedido.Protocolo)) == false)
            {
                FormReportDoPedido rel = new FormReportDoPedido();
                rel.protocolo = Convert.ToString(Pedido.Protocolo);
                rel.ShowDialog();
            }
        }
       
        //cadastros
        private void button4_Click(object sender, EventArgs e)
        {
            FormCadastroResponsavel formCadastro = new FormCadastroResponsavel();

            //if(Properties.Settings.Default.Permissão==2)
            //{               
                formCadastro.ShowDialog();
                DialogResult = DialogResult.None;

                if (formCadastro.DialogResult == DialogResult.OK)
                {
                    // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet3.BRASCAMresponsavel'. Você pode movê-la ou removê-la conforme necessário.
                    this.bRASCAMresponsavelTableAdapter.Fill(this.rSYS2003DataSet3.BRASCAMresponsavel);
                    comboBox1.Text = String.Empty;
                }
            //}
            //else { MessageBox.Show("Apenas administradores podem adicionar usuários.", "Permissão negada!"); }
           
        } 
        private void button9_Click(object sender, EventArgs e)
        {
            FormCadastroVeiculos formCadastro = new FormCadastroVeiculos();            
            formCadastro.ShowDialog();
            DialogResult = DialogResult.None;

            if (formCadastro.DialogResult== DialogResult.OK)
            {
                // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet2.BRASCAMveiculos'. Você pode movê-la ou removê-la conforme necessário.
                this.bRASCAMveiculosTableAdapter.Fill(this.rSYS2003DataSet2.BRASCAMveiculos);
                comboBox2.Text = String.Empty;
            }
        }  
        private void button10_Click(object sender, EventArgs e)
        {
            FormCadastroTransportadora formCadastro = new FormCadastroTransportadora();
            
            formCadastro.ShowDialog();
            DialogResult = DialogResult.None;

            if (formCadastro.DialogResult == DialogResult.OK)
            {
                // TODO: esta linha de código carrega dados na tabela 'rSYS2003DataSet10.BRASCAMtransportadora'. Você pode movê-la ou removê-la conforme necessário.
                this.bRASCAMtransportadoraTableAdapter.Fill(this.rSYS2003DataSet10.BRASCAMtransportadora);
                comboBox3.Text = String.Empty;
            }
        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }        

        //Verificar se já existe pedido de entrega com entregado e carro sem estar entregue
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = comboBox1.Text;
            string parâmetro = "ResponsavelPelaEntrega";

            if (text != String.Empty && carregando==false)
            {               
                VerificarSeExiste(dataFilter, text,parâmetro);
            }
            
        }
        private void comboBox2_SelectedIndexChanged(object sender,EventArgs e)
        {
            string text = comboBox2.Text;
            string parâmetro = "Veiculo";

            if (text != String.Empty && carregando == false)
            {
                VerificarSeExiste(dataFilter, text, parâmetro);

                try
                {
                    SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);

                    string commandStringKm = "SELECT Quilometragem FROM BRASCAMveiculos WHERE Placa='" + comboBox2.Text + "'";
                    SqlCommand commandKm = new SqlCommand(commandStringKm, connection);
                    connection.Open();
                    SqlDataReader readerKm = commandKm.ExecuteReader();
                    object kmFinal = null;
                    while (readerKm.Read()) { kmFinal = readerKm.GetValue(0); }
                    connection.Close();
                    if (kmFinal != null)
                    {
                        textBox6.Text = kmFinal.ToString().Replace('.', ',');
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão");
                    comboBox2.SelectedItem = null;
                    return;
                }
                catch(System.NullReferenceException) 
                { 
                
                }
            }          
        }
        void VerificarSeExiste(string data,string texto,string parâmetro)
        {
            DataTable table = Program.Pedidos.Table;
            DataView view = table.AsDataView();
            view.RowFilter = "StatusDoPedido =1";
            table = view.ToTable();
            DataRow[] dataRows = table.Select(parâmetro + "='" + texto+"'");

            try
            {
                int protocolo = (int)dataRows[0][0];

                if (protocolo != Convert.ToInt32(Pedido.Protocolo))
                {
                    if (MessageBox.Show("Já existe um pedido com o mesmo responsável de entrega ou veículo. Deseja visualizá-lo?", "Pedido existente!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FormNovoPedido novoPedidoForm = new FormNovoPedido();

                        novoPedidoForm.Pedido = new Pedido();
                        novoPedidoForm.Visualizando = true;
                        novoPedidoForm.Pedido.Protocolo = protocolo;
                        novoPedidoForm.FormClosed += NovoPedidoForm_FormClosed;
                        novoPedidoForm.ShowDialog();
                    }
                    else 
                    { 
                        if (parâmetro == "Veiculo") 
                        {
                            comboBox2.SelectedItem = null;
                        }
                        else if (parâmetro == "ResponsavelPelaEntrega")
                        {
                            comboBox1.SelectedItem = null;
                        }
                    }
                }
            }
            catch { }

        }
        private void NovoPedidoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell checkBoxCell = row.Cells[0] as DataGridViewCheckBoxCell;
                checkBoxCell.Value = false;
            }
            for (int n = 0; n < Program.Notas.Count; n++)
            {
                Program.Notas[n]["CheckColumn"] = false;
            }
            NotasSelecionadas.Clear();

        }

        //Notificar sobre quilometragem diferente
        private void textBox4_Leave(object sender, EventArgs e)
        {
           
        }

        private void comboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        void VerificarQuilometragem()
        {
            if (textBox4.Text != String.Empty && comboBox2.Text!=String.Empty && Visualizando==false)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(Programa_BRASCAM.Properties.Settings.Default.ConnectionString);

                    string commandStringKm = "SELECT Quilometragem FROM BRASCAMveiculos WHERE Placa=" + Pedido.Veículo;
                    SqlCommand commandKm = new SqlCommand(commandStringKm, connection);
                    connection.Open();
                    SqlDataReader readerKm = commandKm.ExecuteReader();
                    object kmFinal = null;
                    while (readerKm.Read()) { kmFinal = readerKm.GetValue(0); }
                    connection.Close();

                    if (Pedido.Quilometragem != kmFinal) { MessageBox.Show("A quilometragem está diferente da cadastrada anteriormente"); }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro de conexão");
                    Close();
                    return;
                }
            }
        }
    }


}
