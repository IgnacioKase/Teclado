using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;

namespace Codificador
{
    public class MySR : ToolStripSystemRenderer
    {
        public MySR() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
        }
    }

    class Formato
    {
        #region Attributs
        public Color Primario { get; set; }
        public Color Secundario { get; set; }
        public Color Terciario { get; set; }
        public Color Controls { get; set; }
        public Color Words { get; set; }

        #endregion

        #region Constructores

        public Formato() { }

        public Formato(Color primario, Color secundario, Color terciario, Color controls, Color words)
        {
            Primario = primario;
            Secundario = secundario;
            Terciario = terciario;
            Controls = controls;
            Words = words;
        }
        public Formato(Color primario, Color secundario, Color terciario, Color controls)
        {
            Primario = primario;
            Secundario = secundario;
            Terciario = terciario;
            Controls = controls;
        }
        public Formato(Color primario, Color secundario, Color terciario)
        {
            Primario = primario;
            Secundario = secundario;
            Terciario = terciario;
        }
        public Formato(Color primario, Color secundario)
        {
            Primario = primario;
            Secundario = secundario;
        }
        public Formato(Color primario)
        {
            Primario = primario;
        }
        #endregion

        #region Methods
        public void Inicio(Control.ControlCollection controles, Form formulario)
        {
            bool hasSetColors = true;
            Color[] listaDeColores = new Color[] { Primario, Secundario, Terciario, Controls, Words};
            foreach(Color _x in listaDeColores)
            {
                if (_x.IsEmpty)
                {
                    hasSetColors = false;
                    break;
                }
            }
            if (hasSetColors)
            {
                setForm(controles, formulario);
                setControls(controles);
            }    
        }

        public void inicioDesdeXml(Control.ControlCollection controles, Form formulario, ThemesAppearance tema)
        {
            Primario = tema.Primario;
            Secundario = tema.Secundario;
            Terciario = tema.Terciario;
            Controls = tema.Controls;
            Words = tema.Words;
            Inicio(controles, formulario);
        }

        public List<ThemesAppearance> loadThemesFromXml ()
        {
            int[] rgbInNumber;
            Color primario, secundario, terciario, controls, words;
            String name;
            List<ThemesAppearance> listaDeTemas = new List<ThemesAppearance>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Appearance.xml");

                foreach (XmlNode _x in doc.DocumentElement)
                {
                    name = _x.Attributes[0].Value;

                    rgbInNumber = stringArrayToIntArray(_x["Primario"].InnerText);
                    primario = Color.FromArgb(rgbInNumber[0], rgbInNumber[1], rgbInNumber[2]);
                    rgbInNumber = stringArrayToIntArray(_x["Secundario"].InnerText);
                    secundario = Color.FromArgb(rgbInNumber[0], rgbInNumber[1], rgbInNumber[2]);
                    rgbInNumber = stringArrayToIntArray(_x["Terciario"].InnerText);
                    terciario = Color.FromArgb(rgbInNumber[0], rgbInNumber[1], rgbInNumber[2]);
                    rgbInNumber = stringArrayToIntArray(_x["Controls"].InnerText);
                    controls = Color.FromArgb(rgbInNumber[0], rgbInNumber[1], rgbInNumber[2]);
                    rgbInNumber = stringArrayToIntArray(_x["Words"].InnerText);
                    words = Color.FromArgb(rgbInNumber[0], rgbInNumber[1], rgbInNumber[2]);

                    listaDeTemas.Add(new ThemesAppearance(name, primario, secundario, terciario, controls, words));
                }

            }
            catch
            {
                return new List<ThemesAppearance>();
            }
            return listaDeTemas;
        }

        private void setControls(Control.ControlCollection controles)
        {
            setButton(controles);
            setLabel(controles);
            setComboBox(controles);
            setDataGridView(controles);
            setDateTimePicker(controles);
            setGroupBox(controles);
            setPanel(controles);
            setProgressBar(controles);
            setTextBox(controles);
            setToolStrip(controles);
            setCheckBox(controles);
        }

        private int[] stringArrayToIntArray(string text)
        {
            String[] cadenaString = text.Split(',');
            int[] numberResult = new int[3];
            for (int i = 0; i < cadenaString.Length; i++)
            {
                numberResult[i] = Convert.ToInt16(cadenaString[i]);
            }

            return numberResult;
        }
        #endregion

        #region Individuals Method Sets
        private void setForm(Control.ControlCollection controles, Form formulario)
        {
            formulario.BackColor = Primario;
        }

        private void setCheckBox(Control.ControlCollection controles)
        {
            foreach (CheckBox _x in controles.OfType<CheckBox>())
            {
                _x.ForeColor = Words;
                _x.FlatStyle = FlatStyle.Flat;
                _x.FlatAppearance.BorderColor = Terciario;
                _x.FlatAppearance.BorderSize = 2;
            }
        }

        private void setButton(Control.ControlCollection controles)
        {
            foreach (Button _x in controles.OfType<Button>())
            {
                _x.ForeColor = Words;
                _x.BackColor = Controls;
                _x.FlatStyle = FlatStyle.Flat;
                _x.FlatAppearance.BorderColor = Terciario;
                _x.FlatAppearance.BorderSize = 1;
            }
        }

        private void setLabel(Control.ControlCollection controles)
        {
            foreach (Label _x in controles.OfType<Label>())
            {
                _x.ForeColor = Words;
                _x.FlatStyle = FlatStyle.Flat;
                _x.BorderStyle = BorderStyle.None;
            }
        }

        private void setTextBox(Control.ControlCollection controles)
        {
            foreach (TextBox _x in controles.OfType<TextBox>())
            {
                _x.ForeColor = Words;
                _x.BackColor = Controls;
                _x.BorderStyle = BorderStyle.None;
            }
        }

        private void setToolStrip(Control.ControlCollection controles)
        {
            foreach (ToolStrip _x in controles.OfType<ToolStrip>())
            {
                _x.BackColor = Secundario;
                _x.Renderer = new MySR();
                _x.GripStyle = ToolStripGripStyle.Hidden;
                setToolStripButton(_x);
                setToolStripDropDownButton(_x);
            }
        }

        private void setProgressBar(Control.ControlCollection controles)
        {
            foreach (ProgressBar _x in controles.OfType<ProgressBar>())
            {
                _x.BackColor = Controls;
                _x.ForeColor = Words;
            }
        }

        private void setToolStripDropDownButton(ToolStrip controles)
        {
            foreach (ToolStripDropDownButton _x in controles.Items.OfType<ToolStripDropDownButton>())
            {
                _x.ForeColor = Words;
            }
        }

        private void setToolStripButton(ToolStrip controles)
        {
            foreach (ToolStripButton _x in controles.Items.OfType<ToolStripButton>())
            {
                _x.ForeColor = Words;
            }
        }

        private void setPanel(Control.ControlCollection controles)
        {
            foreach (Panel _x in controles.OfType<Panel>())
            {
                _x.BackColor = Secundario;
                setControls(_x.Controls);
            }
        }

        private void setDataGridView(Control.ControlCollection controles)
        {
            foreach (DataGridView _x in controles.OfType<DataGridView>())
            {
                _x.BackgroundColor = Secundario;
                _x.GridColor = Terciario;
                _x.ForeColor = Words;
                _x.DefaultCellStyle.BackColor = Secundario;
                _x.RowHeadersVisible = false;
                _x.StandardTab = false;
                _x.MultiSelect = false;
            }
        }

        private void setComboBox(Control.ControlCollection controles)
        {
            foreach (ComboBox _x in controles.OfType<ComboBox>())
            {
                _x.ForeColor = Words;
                _x.BackColor = Controls;
                _x.FlatStyle = FlatStyle.Flat;
            }
        }

        private void setDateTimePicker(Control.ControlCollection controles)
        {
            foreach (DateTimePicker _x in controles.OfType<DateTimePicker>())
            {
                _x.ForeColor = Words;
                _x.BackColor = Controls;
                _x.Format = DateTimePickerFormat.Custom;
                _x.CustomFormat = "yyyy/MM/dd";
                _x.CalendarForeColor = Words;
                _x.CalendarTitleBackColor = Secundario;
                _x.CalendarMonthBackground = Secundario;
            }
        }

        private void setGroupBox(Control.ControlCollection controles)
        {
            foreach (GroupBox _x in controles.OfType<GroupBox>())
            {
                _x.BackColor = Secundario;
                _x.ForeColor = Words;
                setControls(_x.Controls);
            }
        }
        #endregion
    }

    class ThemesAppearance
    {
        #region Attributs
        public String Name { get; private set; }
        public Color Primario { get; private set; }
        public Color Secundario { get; private set; }
        public Color Terciario { get; private set; }
        public Color Controls { get; private set; }
        public Color Words { get; private set; }
        #endregion

        #region Methods
        public ThemesAppearance(String name, Color primario, Color secundario, Color terciario, Color controls, Color words)
        {
            Name = name;
            Primario = primario;
            Secundario = secundario;
            Terciario = terciario;
            Controls = controls;
            Words = words;
        }
        #endregion
    }
}