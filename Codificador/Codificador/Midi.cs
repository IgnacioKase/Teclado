using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Codificador
{
    public class Midi
    {
        #region Attributes
        private List<int> nota;
        private List<int> velocidad;
        private int tiempo;
        static private int cantidad;
        static StreamReader lectura;
        static StreamWriter registros;
        static private ulong tempoQN;
        static private ulong ticksQN;
        static TempoMap[] tempoMap;
        static ulong totalTiempoMap;
        static ulong totalTiempoSong;
        static uint cuenta = 0;
        #endregion

        #region Properties
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
        static public ulong TicksQN { get => ticksQN; set => ticksQN = value; }
        public static ulong TempoQN { get => tempoQN; set => tempoQN = value; }
        internal static TempoMap[] TempoMap { get => tempoMap; set => tempoMap = value; }
        public static ulong TotalTiempoSong { get => totalTiempoSong; set => totalTiempoSong = value; }
        public static ulong TotalTiempoMap { get => totalTiempoMap; set => totalTiempoMap = value; }
        public static uint Cuenta { get => cuenta; set => cuenta = value; }
        #endregion

        #region Constructores
        public Midi()
        {
            cantidad++;
        }

        public Midi(int tiempo, List<int> nota, List<int> velocidad)
        {
            Nota = nota;
            Tiempo = tiempo;
            Velocidad = velocidad;
            cantidad++;
        }
        #endregion

        #region Methods

        static public bool OpenFile(string openFile, string saveFile)
        {
            Lectura = new StreamReader(openFile);
            //Registros = new StreamWriter(saveFile);
            string dato = Lectura.ReadLine();
            string[] rows = dato.Split();
            List<TempoMap> tempoMapList = new List<TempoMap>();
            while(dato != "TrkEnd")
            {
                if( rows.Length > 1)
                {
                    switch (rows[1])
                    {
                        case "TimeSig":
                            TicksQN = ((ulong)Int32.Parse(rows[2].Split('/')[0]) / (ulong)Int32.Parse(rows[2].Split('/')[1])) * 4 * (ulong)Int32.Parse(rows[3]);
                            break;
                        case "Tempo":
                            tempoMapList.Add(new TempoMap((ulong)Int32.Parse(rows[0]), (ulong)Int32.Parse(rows[2])));
                            break;
                    }
                }
                dato = Lectura.ReadLine();
                rows = dato.Split();
            }
            TempoMap = tempoMapList.ToArray();
            TempoQN = TempoMap[Cuenta].Velocidad;
            if(TempoMap.Length > 1)
            {
                TotalTiempoMap = TempoMap[Cuenta + 1].Tiempo;
            }
            else
            {
                TotalTiempoMap = 0;
            }
            return true;
        }

        static public Midi ReadFile()
        {
            try
            {
                string dato = Lectura.ReadLine();
                string[] rows = dato.Split();
                Midi palabra = new Midi();
                while (dato != "TrkEnd")
                {
                    if(rows.Length > 1)
                    {
                        if (rows[1] == "On")
                        {
                            palabra = new Midi()
                            {
                                tiempo = Int16.Parse(dato.Split()[0]),
                                nota = new List<int>() { Int16.Parse(dato.Split()[3].Replace("n=", string.Empty)) },
                                velocidad = new List<int>() { Int16.Parse(dato.Split()[4].Replace("v=", string.Empty)) }
                            };
                            return palabra;

                        }
                    }
                    dato = Lectura.ReadLine();
                    rows = dato.Split();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static void NextTempo()
        {
            Cuenta++;
            if(Cuenta <= TempoMap.Length )
            {
                TotalTiempoMap = (ulong)(TempoMap[Cuenta + 1].Tiempo * (ulong)(Midi.TempoQN / Midi.TicksQN) / 1000);
                TempoQN = TempoMap[Cuenta].Velocidad;
            }
        }
        #endregion

        public static void Stop()
        {
            if( Lectura != null)
            {
                Lectura.Close();
                Cuenta = 0;
                TempoQN = 0;
                TicksQN = 0;
                TotalTiempoMap = 0;
                TotalTiempoSong = 0;
            }
        }
    }
}
