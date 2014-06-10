using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Catsland.Core;

namespace Catsland.CatsEditor.PropertyEditorWidget {
    public partial class VectorWidget : UserControl {
        public VectorWidget() {
            InitializeComponent();
        }

        public void SetObserve(CatVector4 _value) {
            m_v4Value = _value;
            // set other to zero
            // TODO
            UpdateShowValue();
        }

        public void UpdateShowValue() {
            // v2
            float vx = 0.0f, vy = 0.0f, vz = 0.0f, vw = 0.0f;

            // v3
            zLabel.Visible = true;
            zValue.Visible = true;
            

            // v4
            if (m_v4Value != null) {
                wLabel.Visible = true;
                wValue.Visible = true;
                // set value
                vx = m_v4Value.X;
                vy = m_v4Value.Y;
                vz = m_v4Value.Z;
                vw = m_v4Value.W;
            }
            // set value
            xValue.Value = new System.Decimal(vx);
            yValue.Value = new System.Decimal(vy);
            zValue.Value = new System.Decimal(vz);
            wValue.Value = new System.Decimal(vw);
        }

        private void Value_ValueChanged(object sender, EventArgs e) {
            NumericUpDown dimenSender = (NumericUpDown)sender;
            float value = float.Parse(dimenSender.Value.ToString());
            // decide receiver
            if (m_v4Value != null) {
                // vector4
                // decide dimension
                if (dimenSender.Name == "xValue") {
                    m_v4Value.X = value;
                }
                else if (dimenSender.Name == "yValue") {
                    m_v4Value.Y = value;
                }
                else if (dimenSender.Name == "zValue") {
                    m_v4Value.Z = value;
                }
                else if (dimenSender.Name == "wValue") {
                    m_v4Value.W = value;
                }
            }
        }

        private CatVector4 m_v4Value;

        
    }
}
