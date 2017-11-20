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
        List<ThemesAppearance> temas = new List<ThemesAppearance>();
        short escala = 40;
        Color claro = Color.FromArgb(255, 240, 240, 240);
        Draw canvas = new Draw();
        Midi midiFile = new Midi();
        
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
            ulong tiempo;
            //canvas.Clean();
            List<NotaMidi> notas = new List<NotaMidi>() { midiFile.NextNote() };
            List<byte> envio = new List<byte>();
            if (notas.Last() != null)
            {
                while (notas.Last().Tiempo == 0)
                {
                    notas.Add(midiFile.NextNote());
                    if (notas.Last() == null) break;
                }
                envio.Add(0xff);
                envio.Add((byte)(notas.Count*2));
                foreach (NotaMidi _x in notas)
                {
                    if (_x != null)
                    {
                        for (int i = 0; i < _x.Nota.Count; i++)
                        {
                            canvas.DrawNotes(_x.Nota[i], (_x.Velocidad[i] == 0));
                            textBox1.AppendText(Convert.ToString(_x.Nota[i], 2) + " " + Convert.ToByte(_x.Velocidad[i] > 0) + " ");
                            textBox2.AppendText(Convert.ToString(_x.Nota[i]) + " " + Convert.ToByte(_x.Velocidad[i] > 0) + " ");
                            envio.Add((byte)(Math.Abs(_x.Nota[i] - escala)));
                            envio.Add(Convert.ToByte(_x.Velocidad[i] > 0));
                        }
                    }
                }
                pictureBox1.Refresh();
                envio.Add(0xfe);
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

                
                if (notas.Last() != null)
                {
                    if ((midiFile.TotalTiempoSong >= midiFile.TotalTiempoMap) && (midiFile.Cuenta < (midiFile.TempoMap.Length - 1))) midiFile.NextTempo();
                    tiempo = (ulong)(notas.Last().Tiempo * (int)(midiFile.TempoQN / midiFile.TicksQN) / 1000);
                    timer1.Interval = (int)tiempo;
                    midiFile.TotalTiempoSong += (ulong)timer1.Interval;
                    timer1.Enabled = true;
                    Text = Convert.ToString(midiFile.TempoQN);
                }
                else
                {
                    canvas.Clean();
                }  
            }else
            {
                canvas.Clean();
            }
        }

        #region Eventos Varios
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
        #endregion

        #region ToolStrip
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
            midiFile.OpenFile(openFileDialog1.FileName);
            midiFile.TotalTiempoSong = 0;
            midiFile.TotalTiempoMap = 0;
            Pause.Enabled = true;
            Stop.Enabled = true;
            CodificarMidi();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            midiFile.Stop();
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
                midiFile.OpenFile(openFileDialog1.FileName);
                midiFile.TotalTiempoSong = 0;
                CodificarMidi();
            }
            Continue.Enabled = false;
            Pause.Enabled = true;
            Stop.Enabled = true;
        }
        #endregion

        #region SerialPort
        private void LoadSerialPorts(object sender, EventArgs e)
        {
            string[] Puertos = new string[] { };
            ComboBox comboBox = ((ComboBox)sender);
            comboBox.Items.Clear();
            Puertos = SerialPort.GetPortNames();
            for (int i = 0; i < Puertos.Length; i++)
            {
                comboBox.Items.Add(Puertos[i]);
            }
        }

        private void ChangePort(object sender, EventArgs e)
        {
            ComboBox Combo = ((ComboBox)sender);
            if (!serialPort1.IsOpen && Combo != null)
            {
                try
                {
                    serialPort1.PortName = Combo.Text;
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
        #endregion

    }
}
