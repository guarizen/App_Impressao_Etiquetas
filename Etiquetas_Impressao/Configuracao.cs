using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Etiquetas_Impressao.Properties;

namespace Etiquetas_Impressao
{
    public partial class Configuracao : Form
    {
        public Configuracao()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnderecoIP = textBox1.Text;
            Properties.Settings.Default.Porta = textBox2.Text;
            Properties.Settings.Default.Authorization = textBox3.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Configurações Salvas com Sucesso!");
        }
        public void GetSettings()
        {
            textBox1.Text = Properties.Settings.Default.EnderecoIP;
            textBox2.Text = Properties.Settings.Default.Porta;
            textBox3.Text = Properties.Settings.Default.Authorization;
        }

        private void Configuracao_Load(object sender, EventArgs e)
        {
            GetSettings();
        }
    }
}
