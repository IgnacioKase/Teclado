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
        string[] rows = new string[] { };
        List<Midi> palabras = new List<Midi>();
        List<int> comandos = new List<int>();
        List<ThemesAppearance> temas = new List<ThemesAppearance>();
        short escala = 40;
        Color claro = Color.FromArgb(255, 240, 240, 240);
        Draw canvas = new Draw();

        List<byte> envio = new List<byte>();
        string[] puertos = new string[] { };
        ulong tiempo;
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
            List<Midi> palabras = new List<Midi>() { Midi.ReadFile() };
            if (palabras.Last() != null)
            {
                while (palabras.Last().Tiempo == 0)
                {
                    palabras.Add(Midi.ReadFile());
                    if (palabras.Last() == null) break;
                }
                envio.Add(0xff);
                envio.Add((byte)palabras.Count);
                foreach (Midi _x in palabras)
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
                envio.Add(0);
                textBox1.AppendText(Environment.NewLine);
                textBox2.AppendText(Environment.NewLine);
                if (serialPort1.IsOpen)
                {
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
                    if ((Midi.TotalTiempoSong >= Midi.TotalTiempoMap) && (Midi.Cuenta <= Midi.TempoMap.Length)) Midi.NextTempo();
                    tiempo = (ulong)(palabras.Last().Tiempo * (int)(Midi.TempoQN / Midi.TicksQN) / 1000);
                    timer1.Interval = (int)tiempo;        
                    Midi.TotalTiempoSong += (ulong)timer1.Interval;
                    timer1.Enabled = true;
                    Text = Convert.ToString(Midi.TempoQN);
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
            Midi.OpenFile(openFileDialog1.FileName, saveFileDialog1.FileName);
            Midi.TotalTiempoSong = 0;
            Pause.Enabled = true;
            Stop.Enabled = true;
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

        private void Stop_Click(object sender, EventArgs e)
        {
            Midi.Stop();
            timer1.Enabled = false;
            canvas.Clean();
            Pause.Enabled = false;
            Stop.Enabled = false;
            Continue.Enabled = true;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            Refresh();
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Pause.Enabled = false;
            Continue.Enabled = true;
        }

        private void Continue_Click(object sender, EventArgs e)
        { 
            if (Stop.Enabled)
            {
                timer1.Enabled = true;
            }
            else
            {
                Midi.OpenFile(openFileDialog1.FileName, saveFileDialog1.FileName);
                Midi.TotalTiempoSong = 0;
                CodificarMidi();
            }
            Continue.Enabled = false;
            Pause.Enabled = true;
            Stop.Enabled = true;
        }
    }
}
