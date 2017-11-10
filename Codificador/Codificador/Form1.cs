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
using System.IO;
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
        List<Int64> comandos = new List<Int64>();
        List<ThemesAppearance> temas = new List<ThemesAppearance>();
        short escala = 40;
        Graphics papel;
        Pen lapiz_negro_grueso = new Pen(Color.Black, 2);
        Pen lapiz_rojo_grueso = new Pen(Color.Red, 2);
        Pen lapiz_negro = new Pen(Color.Black, 1);
        Color claro = Color.FromArgb(255, 240, 240, 240);
        byte[] envio = new byte[] { };
        string[] puertos = new string[] { };

        public Form1()
        {
            InitializeComponent();
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
            Setear();
            Recuadro();
            Grilla();
            /*rows = datos.Split(',');
            palabras = Palabra.ParseData(rows);
            comandos = Palabra.Codificar(palabras, escala);
            foreach (long _x in comandos)
            {
                textBox1.AppendText(Convert.ToString(_x, 2));
                textBox1.AppendText(Environment.NewLine);
            }*/
        }

        private void CodificarMidi()
        {
            Limpiar();
            Recuadro();
            Grilla();
            List<Palabra> palabras = new List<Palabra>() { Palabra.ReadFile() };
            if (palabras.Last() != null)
            {
                while (palabras.Last().Tiempo == 0)
                {
                    palabras.Add(Palabra.ReadFile());
                    if (palabras.Last() == null) break;
                }
                foreach (Palabra _x in palabras)
                {
                    if (_x != null)
                    {
                        foreach (int nota in _x.Nota)
                        {
                            papel.FillRectangle(new SolidBrush(Color.FromArgb(120, 255, 0, 0)), ((int)pictureBox1.Width / 127) * nota, 0, ((int)pictureBox1.Width / 127), pictureBox1.Height);
                        }
                    }
                }
                comandos = Palabra.Codificar(palabras, escala);
                try
                {
                    foreach (long _x in comandos)
                    {
                        textBox1.AppendText(Convert.ToString(_x, 2));
                        textBox1.AppendText(Environment.NewLine);
                        if (serialPort1.IsOpen)
                        {
                            envio = SplitNumerInBytes(_x);
                            serialPort1.Write(envio, 0, envio.Length);
                            serialPort1.DiscardOutBuffer();
                        }

                    }
                }
                catch
                {
                    return;
                }

                
                if (palabras.Last() != null)
                {
                    timer1.Interval = palabras.Last().Tiempo * 2;
                    timer1.Enabled = true;
                }else
                {
                    Limpiar();
                    Recuadro();
                    Grilla();
                }  
            }else
            {
                Limpiar();
                Recuadro();
                Grilla();
            }
            Refresh();
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

        public byte[] SplitNumerInBytes(long numero)
        {
            byte[] resultado = new byte[] { };
            List<byte> bits = new List<byte>();
            long contador = 0;
            long backup = numero;
            int i = 0;
            while (backup != 0)
            {
                backup = backup >> 1;
                contador++;
            }
            if (contador <= 8)
            {
                resultado = new byte[] { (byte)numero };
            }
            else
            {
                for (i = 0; (int)(contador - 8 * (i + 1)) > 0; i++)
                {
                    bits.Add((byte)(numero & 0xff));
                    numero = numero >> 8;
                }
                if (numero != 0) bits.Add((byte)numero);
                resultado = bits.ToArray();
            }
            return resultado;
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

        void Setear()
        {

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            papel = Graphics.FromImage(pictureBox1.Image);
            papel.Clear(claro);
        }

        void Limpiar()
        {
            papel.Clear(claro);
        }

        void Recuadro()
        {
            //Dibuja el recuadro
            #region
            papel.DrawLine(lapiz_negro_grueso, 1, 0, 1, pictureBox1.Height);
            papel.DrawLine(lapiz_negro_grueso, 0, (pictureBox1.Height - 1), pictureBox1.Width,
            (pictureBox1.Height - 1));
            papel.DrawLine(lapiz_negro_grueso, 0, 1, pictureBox1.Width, 1);
            papel.DrawLine(lapiz_negro_grueso, pictureBox1.Width - 1, 0, pictureBox1.Width -
            1, pictureBox1.Height);
            Refresh();
            #endregion
        }

        void Dibujar()
        {
            try
            {
                Refresh();
            }
            catch
            {

            }
        }

        void Grilla()
        {
            //Dibuja la cuadricula
            #region
            for (int i = (pictureBox1.Width / 127); i < pictureBox1.Width; i += (pictureBox1.Width / 127))
            {
                papel.DrawLine(lapiz_negro, i, 0, i, pictureBox1.Height);
            }
            #endregion
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            Setear();
            Recuadro();
            Grilla();
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

    public class Palabra
    {
        #region Atributos
        private List<int> nota;
        private List<int> velocidad;
        private int tiempo;
        static private int cantidad;
        static StreamReader lectura;
        static StreamWriter registros;
        #endregion
        #region Propiedades
        static public int Cantidad
        {
            get { return cantidad; }
        }

        public int Tiempo
        {
            get { return tiempo; }
            set { tiempo = value; }
        }

        public List<int> Velocidad
        {
            get { return velocidad; }
            set { velocidad = value; }
        }

        public List<int> Nota
        {
            get { return nota; }
            set { nota = value; }
        }

        public static StreamReader Lectura { get => lectura; set => lectura = value; }
        public static StreamWriter Registros { get => registros; set => registros = value; }
        #endregion
        #region Constructores
        public Palabra()
        {
            cantidad++;
        }

        public Palabra(int tiempo, List<int> nota, List<int> velocidad)
        {
            Nota = nota;
            Tiempo = tiempo;
            Velocidad = velocidad;
            cantidad++;
        }
        #endregion
        //Viejos
        #region Métodos
        static public List<Palabra> ParseData(string[] rows)
        {
            List<Palabra> palabras = new List<Palabra>();
            bool isNew = true;
            foreach (string _x in rows)
            {
                isNew = (palabras.Count == 0) || (palabras.LastOrDefault().Tiempo != (Int16.Parse(_x.Split()[0])));
                if (isNew)
                {
                    Palabra line = new Palabra
                    (
                        Int16.Parse(_x.Split()[0]),
                        new List<int>() { Int16.Parse(_x.Split()[3].Replace("n=", string.Empty)) },
                        new List<int>() { Int16.Parse(_x.Split()[4].Replace("v=", string.Empty)) }
                    );
                    palabras.Add(line);
                }
                else
                {
                    palabras.Last().Nota.Add(Int16.Parse(_x.Split()[3].Replace("n=", string.Empty)));
                    palabras.Last().Velocidad.Add(Int16.Parse(_x.Split()[4].Replace("v=", string.Empty)));
                }
            }
            return palabras;
        }

        static public List<Int64> Codificar(List<Palabra> palabras, short escala)
        {
            Int64 buffer;
            List<Int64> comandos = new List<Int64>();
            try
            {
                foreach (Palabra _x in palabras)
                {
                    buffer = 0;
                    foreach (int nota in _x.Nota)
                    {
                        if (nota >= escala)
                        {
#pragma warning disable CS0675 // Operador OR bit a bit utilizado en un operando de extensión de signo
                            buffer = buffer | (1 << (byte)(nota - escala));
#pragma warning restore CS0675 // Operador OR bit a bit utilizado en un operando de extensión de signo
                        }
                    }
                    comandos.Add(buffer);
                }
            }
            catch
            {
                return null;
            }
            return comandos;
        }

        static public void ParseDataByFile(string openFile, string saveFile)
        {
            /* StreamReader lectura = new StreamReader(openFile);
             StreamWriter registros = new StreamWriter(saveFile);
             bool isNew = true;
             Palabra palabras = new Palabra();
             string palabra = "";
             while ((palabra = lectura.ReadLine()) != null)
             {
                 isNew = (palabras.Count == 0) || (palabras.LastOrDefault().Tiempo != (Int16.Parse(_x.Split()[0])));
                 if (isNew)
                 {
                     Palabra line = new Palabra
                     (
                         Int16.Parse(_x.Split()[0]),
                         new List<int>() { Int16.Parse(_x.Split()[3].Replace("n=", string.Empty)) },
                         new List<int>() { Int16.Parse(_x.Split()[4].Replace("v=", string.Empty)) }
                     );
                     palabras.Add(line);
                 }
                 else
                 {
                     palabras.Last().Nota.Add(Int16.Parse(_x.Split()[3].Replace("n=", string.Empty)));
                     palabras.Last().Velocidad.Add(Int16.Parse(_x.Split()[4].Replace("v=", string.Empty)));
                 }
             }*/
        }

        static public List<Int64> CodificarByFile(List<Palabra> palabras, short escala)
        {
            Int64 buffer;
            List<Int64> comandos = new List<Int64>();
            foreach (Palabra _x in palabras)
            {
                buffer = 0;
                foreach (int nota in _x.Nota)
                {
                    if (nota >= escala)
                    {
#pragma warning disable CS0675 // Operador OR bit a bit utilizado en un operando de extensión de signo
                        buffer = buffer | (1 << (byte)(nota - escala));
#pragma warning restore CS0675 // Operador OR bit a bit utilizado en un operando de extensión de signo
                    }
                }
                comandos.Add(buffer);
            }
            return comandos;
        }
        #endregion
        #region Métodos nuevos
        static public bool OpenFile(string openFile, string saveFile)
        {
            Lectura = new StreamReader(openFile);
            //Registros = new StreamWriter(saveFile);
            return true;
        }

        static public Palabra ReadFile()
        {
            try
            {
                string dato = Lectura.ReadLine();
                string[] rows = dato.Split(',');
                Palabra palabra = new Palabra()
                {
                    tiempo = Int16.Parse(dato.Split()[0]),
                    nota = new List<int>() { Int16.Parse(dato.Split()[3].Replace("n=", string.Empty)) },
                    velocidad = new List<int>() { Int16.Parse(dato.Split()[4].Replace("v=", string.Empty)) }
                };
                return palabra;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
