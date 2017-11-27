using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Codificador
{
    public class Midi
    {
        #region Attributes 
        private StreamReader lectura;
        private StreamWriter registros;
        private ulong tempoQN = 500000;
        private ulong ticksQN;
        private TempoMap[] tempoMap;
        private ulong totalTiempoMap;
        private ulong totalTiempoSong;
        private uint cuenta = 0;
        #endregion

        #region Properties
        public StreamReader Lectura { get => lectura; set => lectura = value; }
        public StreamWriter Registros { get => registros; set => registros = value; }
        public ulong TicksQN { get => ticksQN; set => ticksQN = value; }
        public ulong TempoQN { get => tempoQN; set => tempoQN = value; }
        public TempoMap[] TempoMap { get => tempoMap; set => tempoMap = value; }
        public ulong TotalTiempoSong { get => totalTiempoSong; set => totalTiempoSong = value; }
        public ulong TotalTiempoMap { get => totalTiempoMap; set => totalTiempoMap = value; }
        public uint Cuenta { get => cuenta; set => cuenta = value; }
        #endregion

        #region Constructores
        
        #endregion

        #region Methods

        public bool OpenFile(string openFile)
        {
            if (openFile == "openFileDialog1") return false;
            Lectura = new StreamReader(openFile);
            string dato = Lectura.ReadLine();
            string[] rows = dato.Split();
            Int32 Btiempo = 0;
            Int32 tiempo = 0;
            List<TempoMap> tempoMapList = new List<TempoMap>();
            while(dato != null)
            {
                if( rows.Length > 1)
                {
                    Int32.TryParse(rows[0], out Btiempo);
                    tiempo += Btiempo;
                    switch (rows[1])
                    {
                        case "TimeSig":
                            TicksQN = ((ulong)Int32.Parse(rows[2].Split('/')[0]) * 4 / (ulong)Int32.Parse(rows[2].Split('/')[1])) * (ulong)Int32.Parse(rows[3]);
                            break;
                        case "Tempo":
                            tempoMapList.Add(new TempoMap((ulong)tiempo, (ulong)Int32.Parse(rows[2]) / 10));
                            break;
                    }
                }
                dato = Lectura.ReadLine();
                if(dato != null) rows = dato.Split();
            }
            TempoMap = tempoMapList.ToArray();
            if (TempoMap.Length > 0)
            {
                if (TempoMap[Cuenta].Tiempo == 0)
                {
                    TempoQN = TempoMap[Cuenta].Velocidad;
                    Cuenta++;
                }  
            }
            if (TempoMap.Length > 1)
            {
                TotalTiempoMap = TempoMap[Cuenta + 1].Tiempo;
            }
            else
            {
                TotalTiempoMap = 0;
            }
            Lectura.Close();
            Lectura = new StreamReader(openFile);
            return true;
        }

        public NotaMidi NextNote()
        {
            try
            {
                string dato = Lectura.ReadLine();
                string[] rows = dato.Split();
                NotaMidi palabra = new NotaMidi();
                while (dato != null)
                {
                    if(rows.Length > 1)
                    {
                        if (rows[1] == "On")
                        {
                            palabra = new NotaMidi()
                            {
                                Tiempo = Int16.Parse(dato.Split()[0]),
                                Nota = new List<int>() { Int16.Parse(dato.Split()[3].Replace("n=", string.Empty)) },
                                Velocidad = new List<int>() { Int16.Parse(dato.Split()[4].Replace("v=", string.Empty)) }
                            };
                            return palabra;

                        }
                    }
                    dato = Lectura.ReadLine();
                    if (dato != null) rows = dato.Split();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void NextTempo(int escaler)
        {
            if ((TotalTiempoSong >= TotalTiempoMap) && ((Cuenta + 1) < (TempoMap.Length - 1)))
            {
                TotalTiempoMap += (ulong)((TempoMap[Cuenta + 1].Tiempo * (ulong)(TempoQN / TicksQN) / 1000) / (ulong)escaler);
                TempoQN = TempoMap[Cuenta].Velocidad;
                Cuenta++;
            }
        }

        public void Stop()
        {
            if (Lectura != null)
            {
                Lectura.Close();
                Cuenta = 0;
                TempoQN = 0;
                TicksQN = 0;
                TotalTiempoMap = 0;
                TotalTiempoSong = 0;
            }
        }
        #endregion  
    }
}
