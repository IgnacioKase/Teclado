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
            rows = datos.Split(',');
            palabras = Palabra.ParseData(rows);
            comandos = Palabra.Codificar(palabras, escala);
            foreach (long _x in comandos)
            {
                textBox1.AppendText(Convert.ToString(_x, 2));
                textBox1.AppendText(Environment.NewLine);
            }
            
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
            saveFileDialog1.ShowDialog();
            saveFileDialog1.DefaultExt = ".txt";
            Palabra.ParseDataByFile(openFileDialog1.FileName, saveFileDialog1.FileName);
            Palabra.CodificarByFile(palabras, escala);
            textBox1.Text = "";
            foreach (long _x in comandos)
            {
                textBox1.AppendText(Convert.ToString(_x, 2));
                textBox1.AppendText(Environment.NewLine);
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
        #region Métodos
        static public List<Palabra> ParseData (string[] rows)
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

        static public void ParseDataByFile(string openFile, string saveFile)
        {
            StreamReader lectura = new StreamReader(openFile);
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
            }
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
    }
}
