using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO.Ports;
using System.Threading;

namespace Codificador
{
    public partial class Form1 : Form
    {
        #region Properties
        Formato inicializacionFormato = new Formato();
        List<ThemesAppearance> temas = new List<ThemesAppearance>();
        short escala = 40;
        Color claro = Color.FromArgb(255, 240, 240, 240);
        Draw canvas = new Draw();
        Midi midiFile = new Midi();
        int countTest;
        #endregion

        #region Attributes
        internal Formato InicializacionFormato { get => inicializacionFormato; set => inicializacionFormato = value; }
        internal List<ThemesAppearance> Temas { get => temas; set => temas = value; }
        public short Escala { get => escala; set => escala = value; }
        public Color Claro { get => claro; set => claro = value; }
        public Draw Canvas { get => canvas; set => canvas = value; }
        public Midi MidiFile { get => midiFile; set => midiFile = value; }
        public int CountTest { get => countTest; set => countTest = value; }
        #endregion

        public Form1()
        {
            InitializeComponent();
            SetCanvas();
            SetThemes();
            
        }

        #region MidiMethods
        private List<NotaMidi> AgruparNotas(List<NotaMidi> listaNotas, Midi midi)
        {
            while (listaNotas.Last().Tiempo == 0)
            {
                listaNotas.Add(midi.NextNote());
                if (listaNotas.Last() == null) break;
            }
            return listaNotas;
        }

        private List<byte> ProcesarNotas(List<NotaMidi> listaNotas, List<byte> serialArray)
        {
            serialArray.Add(0xff);
            serialArray.Add((byte)(listaNotas.Count * 2));
            foreach (NotaMidi _x in listaNotas)
            {
                if (_x != null)
                {
                    for (int i = 0; i < _x.Nota.Count; i++)
                    {
                        serialArray.Add((byte)(Math.Abs(_x.Nota[i] - Escala)));
                        serialArray.Add(Convert.ToByte(_x.Velocidad[i] > 0));
                    }
                }
            }
            serialArray.Add(0xfe);
            return serialArray;
        }

        private void CalcularTiempo(List<NotaMidi> listaNotas)
        {
            ulong tiempo;
            if (listaNotas.Last() != null)
            {
                MidiFile.NextTempo(tempoScaler.Value);
                tiempo = (ulong)(listaNotas.Last().Tiempo * (int)(MidiFile.TempoQN / MidiFile.TicksQN) / 1000) / (ulong)tempoScaler.Value;
                if (tiempo == 0) tiempo = 1;
                timer1.Interval = (int)tiempo;
                MidiFile.TotalTiempoSong += (ulong)timer1.Interval;
                timer1.Enabled = true;
            }
        }

        private void ProcesarDibujo(List<NotaMidi> listaNotas, PictureBox picture)
        {
            foreach (NotaMidi _x in listaNotas)
            {
                if (_x != null)
                {
                    for (int i = 0; i < _x.Nota.Count; i++)
                    {
                        Canvas.DrawNotes(_x.Nota[i], (_x.Velocidad[i] == 0));
                        picture.Refresh();
                    }
                }
            }
        }

        private void ProcesarMonitores(List<NotaMidi> listaNotas, List<byte> serialArray, TextBox textBinario, TextBox textDecimal, TextBox textSerial)
        {
            foreach (NotaMidi _x in listaNotas)
            {
                if (_x != null)
                {
                    for (int i = 0; i < _x.Nota.Count; i++)
                    {
                        textBinario.AppendText(Convert.ToString(_x.Nota[i], 2) + " " + Convert.ToByte(_x.Velocidad[i] > 0) + " ");
                        textDecimal.AppendText(Convert.ToString(_x.Nota[i]) + " " + Convert.ToByte(_x.Velocidad[i] > 0) + " ");
                    }
                }
            }
            textBinario.AppendText(Environment.NewLine);
            textDecimal.AppendText(Environment.NewLine);
            if (serialPort1.IsOpen)
            {
                foreach (byte instruccion in serialArray)
                {
                    textSerial.AppendText(Convert.ToString(instruccion) + " ");
                }
                textSerial.AppendText(Environment.NewLine);
            }
        }

        private void ReproducirNotas(List<NotaMidi> listaNotas)
        {
            foreach (NotaMidi _x in listaNotas)
            {
                if (_x != null)
                {
                    for (int i = 0; i < _x.Nota.Count; i++)
                    {
                        if (_x.Velocidad[i] > 0)
                        {
                            int NotaActual = _x.Nota[i];
                            new Thread(() =>
                            {
                                double frecuencia = ((double)(NotaActual - 69)) / 12.00;
                                if (NotaActual > 27)
                                    Console.Beep((int)(Math.Pow(2, frecuencia) * 440), 250);
                            }).Start();
                        }
                    }
                }
            }
        }

        private void CodificarMidi()
        {
            List<NotaMidi> notas = new List<NotaMidi>() { MidiFile.NextNote() };
            List<byte> envio = new List<byte>();
            if (notas.Last() != null)
            {
                notas = AgruparNotas(notas, MidiFile);
                envio = ProcesarNotas(notas, envio);
                EnviarTrama(envio);
                ProcesarDibujo(notas, pictureBox1);
                ProcesarMonitores(notas, envio, textBox1, textBox2, textBox3);
                ReproducirNotas(notas);
                envio.Clear();
                CalcularTiempo(notas);
            }
            if (notas.Last() == null) Canvas.Clean();
        }
        #endregion

        #region Eventos Varios
        private void SetCanvas()
        {
            Canvas = new Draw(pictureBox1, Claro);
            Canvas.Set();
            Canvas.Box();
            Canvas.Grid();
        }

        private void SetThemes()
        {
            Temas = InicializacionFormato.loadThemesFromXml();
            foreach (ThemesAppearance _x in Temas)
            {
                if (_x.Name == "Dark")
                {
                    InicializacionFormato.inicioDesdeXml(Controls, this, _x);
                    toolStripComboBox1.Text = "Dark";
                }
                toolStripComboBox1.Items.Add(_x.Name);
            }
        }

        private void PictureBox1_Resize(object sender, EventArgs e)
        {
            Canvas.Set();
            Canvas.Box();
            Canvas.Grid();
            Refresh();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            CodificarMidi();
        }

        private void ResetQs_Click(object sender, EventArgs e)
        {
            SendReset();
        }

        private void Jog_Click(object sender, EventArgs e)
        {
            CountTest = 0;
            SendTest(CountTest, 1);
        }

        private void TimerJob_Tick(object sender, EventArgs e)
        {
            timerTest.Enabled = false;
            if (CountTest < 64)
                SendTest(CountTest, 0);
            CountTest++;
            if(CountTest < 64)
                SendTest(CountTest, 1);
        }

        private void comandoSerial_Click(object sender, EventArgs e)
        {
            byte[] trama = new byte[]
            {
                0xff,
                (byte)notaTextBox.Text.Split(',').Length,
                Byte.Parse(notaTextBox.Text.Split(',')[0]),
                (byte)(Byte.Parse(notaTextBox.Text.Split(',')[1]) & 1),
                0xfe
            };
            List<NotaMidi> notas = new List<NotaMidi>()
            {
                new NotaMidi(0, new List<int>(){Int16.Parse(notaTextBox.Text.Split(',')[0]) }, new List<int>(){ Int16.Parse(notaTextBox.Text.Split(',')[1]) & 1 } )
            };
            EnviarTrama(trama.ToList());
            ProcesarMonitores(notas, trama.ToList(), textBox1, textBox2, textBox3);
            ProcesarDibujo(notas, pictureBox1);
            ReproducirNotas(notas);
        }
        #endregion

        #region ToolStrip
        private void ToolStripComboBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (ThemesAppearance _x in Temas)
            {
                if (_x.Name == toolStripComboBox1.Text)
                {
                    InicializacionFormato.inicioDesdeXml(Controls, this, _x);
                }
            }
        }

        private void TextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            bool midiAbierto = MidiFile.OpenFile(openFileDialog1.FileName);
            if (midiAbierto)
            {
                MidiFile.TotalTiempoSong = 0;
                Pause.Enabled = true;
                Stop.Enabled = true;
                CodificarMidi();
            }     
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            MidiFile.Stop();
            timer1.Enabled = false;
            Canvas.Clean();
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
                MidiFile.OpenFile(openFileDialog1.FileName);
                MidiFile.TotalTiempoSong = 0;
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

        private void Button1_Click(object sender, EventArgs e)
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

        private void SendTest(int nota, byte estado)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    List<byte> trama = new List<byte>() { 0xff, 2, (byte)nota, estado, 0xfe };
                    serialPort1.Write(trama.ToArray(), 0, 5);
                    ProcesarMonitores(new List<NotaMidi>() { new NotaMidi(0, new List<int>() { nota }, new List<int>() { estado }) }, trama, textBox1, textBox2, textBox3);
                    ProcesarDibujo(new List<NotaMidi>() { new NotaMidi(0, new List<int>() { nota }, new List<int>() { estado }) }, pictureBox1);
                    ReproducirNotas(new List<NotaMidi>() { new NotaMidi(0, new List<int>() { nota }, new List<int>() { estado }) });
                    timerTest.Enabled = true;
                }
            }
            catch
            {
                return;
            }
        }

        private void SendReset()
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    List<byte> trama = new List<byte>();
                    List<NotaMidi> notas = new List<NotaMidi>();
                    trama.Add(0xff);
                    trama.Add(128);
                    for (byte i = 0; i < 64; i++)
                    {
                        trama.Add(i);
                        trama.Add(0);
                        notas.Add(new NotaMidi(0, new List<int>() { i }, new List<int>() { 0 }));
                    }
                    trama.Add(0xfe);
                    serialPort1.Write(trama.ToArray(), 0, trama.Count);
                    ProcesarMonitores(notas, trama, textBox1, textBox2, textBox3);
                }
            }
            catch
            {
                return;
            }
        }

        private void EnviarTrama(List<byte> serialArray)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(serialArray.ToArray(), 0, serialArray.Count);
            }
        }
        #endregion
    }
}
