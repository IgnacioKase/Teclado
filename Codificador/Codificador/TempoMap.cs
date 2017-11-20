using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codificador
{
    public class TempoMap
    {
        #region Properties
        private ulong tiempo;
        private ulong velocidad;
        #endregion

        #region Attributes
        public ulong Tiempo { get => tiempo; set => tiempo = value; }
        public ulong Velocidad { get => velocidad; set => velocidad = value; }
        #endregion

        #region Constructors
        public TempoMap()
        {

        }

        public TempoMap(ulong tiempo, ulong velocidad)
        {
            Tiempo = tiempo;
            Velocidad = velocidad;
        }
        #endregion
    }
}
