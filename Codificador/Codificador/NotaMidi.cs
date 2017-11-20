using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codificador
{
    public class NotaMidi
    {
        #region Attributes
        private List<int> nota;
        private List<int> velocidad;
        private int tiempo;
        private static int cantidad;
        #endregion

        #region Properties
        public List<int> Nota { get => nota; set => nota = value; }
        public List<int> Velocidad { get => velocidad; set => velocidad = value; }
        public int Tiempo { get => tiempo; set => tiempo = value; }
        protected static int Cantidad { get => cantidad; set => cantidad = value; }
        #endregion

        #region Constructors
        public NotaMidi()
        {
            Cantidad++;
        }

        public NotaMidi(int tiempo, List<int> nota, List<int> velocidad)
        {
            Nota = nota;
            Tiempo = tiempo;
            Velocidad = velocidad;
            Cantidad++;
        }
        #endregion
    }
}
