using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Codificador
{
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

        /*static public List<Int64> Codificar(List<Palabra> palabras, short escala)
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
        }*/

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
