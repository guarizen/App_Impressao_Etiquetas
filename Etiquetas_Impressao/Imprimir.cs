using Impressao_Etiquetas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Etiquetas_Impressao.Properties;

namespace Etiquetas_Impressao
{
    public partial class ImpressaoEtiquetas : Form
    {
        public ImpressaoEtiquetas()
        {
            InitializeComponent();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && ((textBox2.Text) != "") && (textBox1.Text.Length <= 9) && (textBox2.Text.Length <= 9))
            {
                string cod_filial = (textBox1.Text);
                string documento = (textBox2.Text);

                string End = Properties.Settings.Default.EnderecoIP;
                string Port = Properties.Settings.Default.Porta;
                string Hash64 = Properties.Settings.Default.Authorization;

                var enderecoapi = $"http://{End}:{Port}/api/millenium!pillow/movimentacao/lista_recebimento?romaneio=" + $"{documento}" + "&cod_filial=" + $"{cod_filial}" + "&$format=json";

                try
                {
                    var requisicaoWeb = WebRequest.CreateHttp($"{enderecoapi}");
                    requisicaoWeb.Method = "GET";
                    requisicaoWeb.Headers.Add("Authorization", $"Basic {Hash64}");
                    requisicaoWeb.UserAgent = "Consulta API - Recebimento Etiquetas";
                    requisicaoWeb.Timeout = 130000;

                    using (var resposta = requisicaoWeb.GetResponse())
                    {
                        var streamDados = resposta.GetResponseStream();
                        StreamReader reader = new StreamReader(streamDados);
                        object objResponse = reader.ReadToEnd();

                        var statusCodigo = ((System.Net.HttpWebResponse)resposta).StatusCode;

                        ListaRecebimento Receb = JsonConvert.DeserializeObject<ListaRecebimento>(objResponse.ToString());

                        foreach (var reb in Receb.Value)
                        {
                            string s = "^XA" +
                                       "^FWR" +
                                       "^MMT^MNY^EF^FS" +
                                       "^DFFORM1^FS^LH00,00^FS^MUM" +
                                       "^FO000,012^XGIMAGEM^FS" +
                                       "^FO017,002^A0,6,4.5^FN1^FS" +
                                       "^FO040,010^A0,16,24^FB440,1,,R^FN2^FS" +
                                       "^FO010,008^A0,3,4.5^FN3^FS" +
                                       "^FO012,086^A0,3,3^FN4^FS" +
                                       "^FO007,095^A0,3,3^FN5^FS" +
                                       "^FO003,006^A1,2,1^BC25,7,19^FN6^FS" +
                                       "^MUD" +
                                       "^XZ" +
                                       "^XA" +
                                       "^XFFORM1^FS" +
                                       $"^FN1^FD{reb.CodProduto}^FS" +
                                       "^FN2^FD^FS" +
                                       $"^FN3^FD{reb.Descricao1}^FS" +
                                       $"^FN4^FD{reb.Cor}^FS" +
                                       $"^FN5^FD{reb.Tamanho}^FS" +
                                       $"^FN6^FD{reb.Ean13}^FS" +
                                       "^XB" +
                                       "^XZ";



                            /*
                           "^XA" +
                           "^CF0,30" +
                           "^FO10,40^FDPILLOWTEX IND. COM. TEXTIL - LTDA^FS" +
                           "^CFA,30" +
                           "^CF0,20" +
                           $"^FO40,220^FDCodigo: {reb.CodProduto} ^FS" +
                           $"^FO40,250^FDCor: {reb.Cor}^FS" +
                           $"^FO40,280^FDEstampa: {reb.Estampa}^FS" +
                           $"^FO40,310^FDTam: {reb.Tamanho}^FS" +
                           "^CF0,20" +
                           $"^FO26,130^FD{reb.Descricao1}^FS" +
                           "^CFA,15" +
                           "^BY2,1,100^BEN,100,Y,N" +
                           $"^FO80,360^BC^FD{reb.Ean13}^FS" +
                           "^XZ";
                           */

                            for (int i = 1; reb.Quantidade >= i; i++)
                            {
                                PrintDialog pd = new PrintDialog();
                                pd.PrinterSettings = new PrinterSettings();
                                RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, s);
                            }
                        }
                        if (Receb.OdataCount == 0)
                        {
                            MessageBox.Show("Não Encontrada movimentação para realizar a Impressão.");
                        }
                        else
                        {
                            MessageBox.Show("Processo Concluído!");
                        }
                    }
                }
                catch (WebException e1)
                {
                    if (e1.Status == WebExceptionStatus.ProtocolError)
                    {
                        var respostaErr = (HttpWebResponse)e1.Response;

                        using (Stream dataStreamErr = respostaErr.GetResponseStream())
                        {
                            StreamReader readerErr = new StreamReader(dataStreamErr);
                            string responseFromServer = readerErr.ReadToEnd();

                            string responseServer1 = responseFromServer.Replace("\"}}}", "");
                            string[] responseServerEnd = responseServer1.Split();
                            MessageBox.Show("Retorno Millennium: " + " " + responseServerEnd[6] + " " + responseServerEnd[7] + " " + responseServerEnd[8] + " " + responseServerEnd[9]);

                        }
                    }
                    else
                    {
                        MessageBox.Show(e1.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Dados Incorretos, verifique os campos informados.");
            }
        }

        private void ImpressaoEtiquetas_Load(object sender, EventArgs e)
        {

        }

        private void FecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ConfiguraçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuracao telaconf = new Configuracao();
            telaconf.ShowDialog();
        }
    }
}
