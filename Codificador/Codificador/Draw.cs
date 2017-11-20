using System.Drawing;
using System.Windows.Forms;

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

        public void Grid()
        {
            for (int i = (PictureBox.Width / 127); i < PictureBox.Width; i += (PictureBox.Width / 127))
            {
                BackgroundSheet.DrawLine(Lapiz_negro, i, 0, i, PictureBox.Height);
            }
        }

        public void DrawNotes(int nota, bool hidden)
        {
            SolidBrush Pincel = new SolidBrush(NotesColor);
            if (hidden) Pincel = new SolidBrush(Background);
            Sheet.FillRectangle(Pincel, ((int)PictureBox.Width / 127) * nota + 1, 2, ((int)PictureBox.Width / 127) - 1, PictureBox.Height - 4);
        }
        #endregion
    }
}
