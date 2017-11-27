using System.Drawing;
using System.Windows.Forms;
using System;
using System.IO;

namespace Codificador
{
    public class Draw
    {
        #region Attributes
        private Graphics sheet;
        private Graphics backgroundSheet;
        private PictureBox pictureBox;
        private Color background = Color.FromArgb(255, 255, 255, 255);
        private Bitmap bitmap;
        private Pen lapiz_negro_grueso = new Pen(Color.Black, 2);
        private Pen lapiz_rojo_grueso = new Pen(Color.Red, 2);
        private Pen lapiz_negro = new Pen(Color.Black, 1);
        private Color notesColor = Color.FromArgb(120, 255, 0, 0);
        #endregion

        #region Properties
        public Graphics Sheet { get => sheet; set => sheet = value; }
        public PictureBox PictureBox { get => pictureBox; set => pictureBox = value; }
        public Color Background { get => background; set => background = value; }
        public Pen Lapiz_negro_grueso { get => lapiz_negro_grueso; set => lapiz_negro_grueso = value; }
        public Pen Lapiz_rojo_grueso { get => lapiz_rojo_grueso; set => lapiz_rojo_grueso = value; }
        public Pen Lapiz_negro { get => lapiz_negro; set => lapiz_negro = value; }
        public Bitmap Bitmap { get => bitmap; }
        public Color NotesColor { get => notesColor; set => notesColor = value; }
        public Graphics BackgroundSheet { get => backgroundSheet; set => backgroundSheet = value; }
        #endregion

        #region Constructors
        public Draw()
        {

        }

        public Draw(PictureBox pictureBox)
        {
            PictureBox = pictureBox;
            Set();
        }

        public Draw(PictureBox pictureBox, Color background)
        {
            PictureBox = pictureBox;
            Background = background;
            Set();
        }
        #endregion

        #region Methods
        public void Set()
        {

            bitmap = new Bitmap(PictureBox.Width, PictureBox.Height);
            PictureBox.Image = bitmap;
            Sheet = Graphics.FromImage(PictureBox.Image);
            Clean();
            bitmap = new Bitmap(PictureBox.Width, PictureBox.Height);
            PictureBox.BackgroundImage = bitmap;
            BackgroundSheet = Graphics.FromImage(PictureBox.BackgroundImage);
            BackgroundSheet.Clear(Background);
        }

        public void Clean()
        {
           Sheet.Clear(Color.Transparent);
        }

        public void Box()
        {
            BackgroundSheet.DrawLine(Lapiz_negro_grueso, 1, 0, 1, PictureBox.Height);
            BackgroundSheet.DrawLine(Lapiz_negro_grueso, 0, (PictureBox.Height - 1), PictureBox.Width,(PictureBox.Height - 1));
            BackgroundSheet.DrawLine(Lapiz_negro_grueso, 0, 1, PictureBox.Width, 1);
            BackgroundSheet.DrawLine(Lapiz_negro_grueso, PictureBox.Width - 1, 0, PictureBox.Width -1, PictureBox.Height);
            PictureBox.Refresh();
        }

        /*public void Grid()
        {
            for (int i = (PictureBox.Width / 127); i < PictureBox.Width; i += (PictureBox.Width / 127))
            {
                BackgroundSheet.DrawLine(Lapiz_negro, i, 0, i, PictureBox.Height);
            }
        }*/

        public void Grid()
        {
            int buffer;
            SolidBrush Pincel = new SolidBrush(Color.Black);
            for (int i = (PictureBox.Width / 72); i < PictureBox.Width; i += (PictureBox.Width / 72))
            {
                BackgroundSheet.DrawLine(Lapiz_negro, i, 0, i, PictureBox.Height);
                buffer = (i / (PictureBox.Width / 72)) - 7 * (int)((i / (PictureBox.Width / 72)) / 7);
                if(buffer != 3 && buffer != 0)
                    BackgroundSheet.FillRectangle(Pincel, (i - (PictureBox.Width / 216)), 0, (PictureBox.Width / 108), (PictureBox.Height * 2 / 3));
            }
        }

        protected int CalculateBlackNotes(int nota)
        {
            int cantidad = 0;
            int escala = (int)(nota / 12);
            int notaEscalada = nota - 12 * (int)(nota / 12);
            if (notaEscalada < 4 && notaEscalada >= 2)
                cantidad = 1;
            if (notaEscalada < 7 && notaEscalada >= 4)
                cantidad = 2;
            if (notaEscalada < 9 && notaEscalada >= 7)
                cantidad = 3;
            if (notaEscalada < 10 && notaEscalada >= 9)
                cantidad = 4;
            if (notaEscalada < 12 && notaEscalada >= 10)
                cantidad = 5;
            cantidad += escala * 5;
            return cantidad;
        }

        protected int CalculateWhiteNotes(int nota)
        {
            int cantidad = 0;
            int escala = (int)(nota / 12);
            int notaEscalada = nota - 12 * (int)(nota / 12);
            if (notaEscalada == 1)
                cantidad = 1;
            if (notaEscalada == 3)
                cantidad = 2;
            if (notaEscalada == 6)
                cantidad = 4;
            if (notaEscalada == 8)
                cantidad = 5;
            if (notaEscalada == 10)
                cantidad = 6;
            cantidad += escala * 7;
            return cantidad;
        }
        public void DrawNotes(int nota, bool hidden)
        {
            int buffer;
            SolidBrush Pincel = new SolidBrush(NotesColor);
            if (hidden) Pincel = new SolidBrush(Background);
            buffer = nota - 12 * (int)(nota / 12);
            bool esBlanca = buffer != 1 && buffer != 3 && buffer != 6 && buffer != 8 && buffer != 10;
            if (esBlanca) nota -= CalculateBlackNotes(nota);
            if (!esBlanca)
                nota = CalculateWhiteNotes(nota);
            if (hidden && !esBlanca) Pincel = new SolidBrush(Color.Black);
            if (esBlanca)
            {
                if(buffer == 4 || buffer == 11)
                    Sheet.FillRectangle(Pincel, (((int)PictureBox.Width / 72) * nota + 1 + (PictureBox.Width / 216)), 2, (PictureBox.Width / 108), (PictureBox.Height * 2 / 3) - 2);
                if(buffer == 5 || buffer == 0)
                    Sheet.FillRectangle(Pincel, (((int)PictureBox.Width / 72) * nota + 1), 2, (PictureBox.Width / 108), (PictureBox.Height * 2 / 3) - 2);
                if ((buffer != 5 && buffer != 0) && (buffer != 4 && buffer != 11))
                    Sheet.FillRectangle(Pincel, (((int)PictureBox.Width / 72) * nota + (PictureBox.Width / 216)), 2, (PictureBox.Width / 216) + 2, (PictureBox.Height * 2 / 3) - 2);
                Sheet.FillRectangle(Pincel, ((int)PictureBox.Width / 72) * nota + 1, (PictureBox.Height * 2 / 3), ((int)PictureBox.Width / 72) -1, (PictureBox.Height / 3) - 1);
                
            }
            else
            {
                Sheet.FillRectangle(Pincel, ((int)PictureBox.Width / 72) * nota + 1 - ((int)PictureBox.Width / 216), 2, ((int)PictureBox.Width / 108) - 1, (PictureBox.Height * 2 / 3) - 4);
                //Sheet.FillRectangle(Pincel, ((int)PictureBox.Width / 216) * nota + 1, 2, ((int)PictureBox.Width / 108) - 1, (PictureBox.Height * 2 / 3) - 4); 
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Bitmap.Dispose();
                Lapiz_negro.Dispose();
                Lapiz_negro_grueso.Dispose();
                Lapiz_rojo_grueso.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
