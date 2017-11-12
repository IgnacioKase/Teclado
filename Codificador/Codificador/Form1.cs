using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.IO.Ports;

namespace Codificador
{
    public partial class Form1 : Form 
    {
        Formato inicializacionFormato = new Formato();
        string datos = "1786 On ch=1 n=60 v=0,1792 On ch=1 n=62 v=85,1792 On ch=1 n=48 v=80,1792 On ch=1 n=58 v=79,2023 On ch=1 n=62 v=0,2048 On ch=1 n=60 v=69";
        string[] rows = new string[] { };
        List<Palabra> palabras = new List<Palabra>();
        List<int> comandos = new List<int>();
        List<ThemesAppearance> temas = new List<ThemesAppearance>();
        short escala = 40;
        Color claro = Color.FromArgb(255, 240, 240, 240);
        Draw canvas = new Draw();

        List<byte> envio = new List<byte>();
        string[] puertos = new string[] { };

        public Form1()
        {
            InitializeComponent();
            canvas = new Draw(pictureBox1, claro);
            canvas.Set();
            canvas.Box();
            canvas.Grid();

            temas = inicializacionFormato.loadThemesFromXml();
            foreach (ThemesAppearance _x in temas)
            {
                if (_x.Name == "Dark")
                {
                    inicializacionFormato.inicioDesdeXml(Controls, this, _x);
                    toolStripComboBox1.Text = "Dark";
                }
                toolStripComboBox1.Items.Add(_x.Name);
            }
        }

        private void CodificarMidi()
        {
            canvas.Clean();
            List<Palabra> palabras = new List<Palabra>() { Palabra.ReadFile() };
            if (palabras.Last() != null)
            {
                while (palabras.Last().Tiempo == 0)
                {
                    palabras.Add(Palabra.ReadFile());
                    if (palabras.Last() == null) break;
                }
                envio.Add((byte)palabras.Count);
                foreach (Palabra _x in palabras)
                {
                    if (_x != null)
                    { 
                        foreach (int nota in _x.Nota)
                        {
                            canvas.DrawNotes(nota);
                            textBox1.AppendText(Convert.ToString(nota, 2) + " ");
                            textBox2.AppendText(Convert.ToString(nota) + " ");          
                            envio.Add((byte)(Math.Abs(nota - escala)));
                            
                        }
                    }
                }
                textBox1.AppendText(Environment.NewLine);
                textBox2.AppendText(Environment.NewLine);
                if (serialPort1.IsOpen)
                {
                    serialPort1.DiscardOutBuffer();
                    serialPort1.Write(envio.ToArray(), 0, envio.Count);
                    foreach (byte instruccion in envio)
                    {
                        textBox3.AppendText(Convert.ToString(instruccion) + " ");
                    }
                    textBox3.AppendText(Environment.NewLine);
                }
                envio.Clear();

                
                if (palabras.Last() != null)
                {
                    timer1.Interval = palabras.Last().Tiempo * 5;
                    timer1.Enabled = true;
                }else
                {
                    canvas.Clean();
                }  
            }else
            {
                canvas.Clean();
            }
            pictureBox1.Refresh();
        }

        private void toolStripComboBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (ThemesAppearance _x in temas)
            {
                if (_x.Name == toolStripComboBox1.Text)
                {
                    inicializacionFormato.inicioDesdeXml(Controls, this, _x);
                }
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            //saveFileDialog1.ShowDialog();
            //saveFileDialog1.DefaultExt = ".txt";
            /*Palabra.ParseDataByFile(openFileDialog1.FileName, saveFileDialog1.FileName);
            Palabra.CodificarByFile(palabras, escala);
            textBox1.Text = "";
            foreach (long _x in comandos)
            {
                textBox1.AppendText(Convert.ToString(_x, 2));
                textBox1.AppendText(Environment.NewLine);
            }*/
            Palabra.OpenFile(openFileDialog1.FileName, saveFileDialog1.FileName);
            CodificarMidi();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            canvas.Set();
            canvas.Box();
            canvas.Grid();
            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            CodificarMidi();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                button1.Text = "Abrir";
            }
            else
            {
                serialPort1.Open();
                button1.Text = "Cerrar";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                }
                catch
                {
                    MessageBox.Show("Cierre el puerto para cambiar la dirección", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Cierre el puerto para cambiar la dirección", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            puertos = SerialPort.GetPortNames();
            for (int i = 0; i < puertos.Length; i++)
            {
                comboBox1.Items.Add(puertos[i]);
            }
        }
    }
}
