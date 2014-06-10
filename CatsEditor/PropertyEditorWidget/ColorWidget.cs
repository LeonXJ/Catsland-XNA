using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;

namespace Catsland.CatsEditor.PropertyEditorWidget {
    public partial class ColorWidget : UserControl {
        public ColorWidget() {
            InitializeComponent();
        }

        public void SetObserve(CatColor _value) {
            m_value = _value;
            UpdateShowValue();
        }

        public void UpdateShowValue() {
            if (m_value != null) {
                colorBox.BackColor = Color.FromArgb(
                    255,
                    (int)(255.0f * m_value.m_value.X),
                    (int)(255.0f * m_value.m_value.Y),
                    (int)(255.0f * m_value.m_value.Z));
                alphaBox.Value = new System.Decimal(m_value.m_value.W);
            }
        }

        private void colorBox_Click(object sender, EventArgs e) {
            // show color box
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.SolidColorOnly = false;
            
            colorDialog.Color = Color.FromArgb(
                    255,
                    (int)(255.0f * m_value.m_value.X),
                    (int)(255.0f * m_value.m_value.Y),
                    (int)(255.0f * m_value.m_value.Z));

            if (colorDialog.ShowDialog(this) == DialogResult.OK) {
                m_value.m_value.X = colorDialog.Color.R/255.0f;
                m_value.m_value.Y = colorDialog.Color.G/255.0f;
                m_value.m_value.Z = colorDialog.Color.B/255.0f;
                UpdateShowValue();
            }            
        }

        private void alphaBox_ValueChanged(object sender, EventArgs e) {
            m_value.m_value.W = float.Parse(alphaBox.Value.ToString());
        }

        CatColor m_value;
    }
}
